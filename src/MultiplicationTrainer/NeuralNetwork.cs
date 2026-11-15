namespace MultiplicationTrainer; // اجزای محاسباتی وابسته به پایگاه داده نیستند.

public sealed class DenseLayer // وزن هر نورون یک سطر مستقل دارد.
{
    public double[][] Weights { get; set; } = []; // شکل ماتریس برابر [تعداد خروجی، تعداد ورودی] است.
    public double[] Biases { get; set; } = []; // هر نورون خروجی یک بایاس اسکالر دارد.
}

public sealed record Gradients(double[][][] Weights, double[][] Biases); // گرادیان‌ها پیش از تغییر پارامترها به‌طور کامل محاسبه می‌شوند.

public sealed class NeuralNetwork // لایه‌های مخفی tanh و لایهٔ خروجی خطی‌اند.
{
    public DenseLayer[] Layers { get; } // ترتیب لایه‌ها جهت انتشار رو به جلو را مشخص می‌کند.

    public NeuralNetwork(DenseLayer[] layers) // بارگذاری پارامترها بدون مقداردهی تصادفی دوباره انجام می‌شود.
    {
        Layers = layers; // مالکیت آرایه‌ها به مدل تازه‌ساخته‌شده واگذار می‌شود.
    }

    public static NeuralNetwork Create(Settings settings) // تنها آموزش از صفر این سازنده را به کار می‌برد.
    {
        settings.Validate(); // ابعاد پیش از تخصیص حافظه بررسی می‌شوند.
        int[] sizes = [2, .. settings.HiddenSizes, 1]; // ورودی دو عامل و خروجی یک مقدار رگرسیونی است.
        var random = new Random(settings.Seed); // تمام مقادیر اولیه از یک دنبالهٔ تکرارپذیر می‌آیند.
        var layers = new DenseLayer[sizes.Length - 1]; // بین هر دو اندازهٔ پیاپی یک لایهٔ متراکم قرار می‌گیرد.
        for (int l = 0; l < layers.Length; l++) // لایه‌ها به ترتیب ساخته می‌شوند.
        {
            double limit = Math.Sqrt(6.0 / (sizes[l] + sizes[l + 1])); // مقداردهی Xavier از بزرگ‌شدن اولیهٔ سیگنال جلوگیری می‌کند.
            var layer = new DenseLayer { Weights = new double[sizes[l + 1]][], Biases = new double[sizes[l + 1]] }; // بایاس و سطرهای وزن هم‌اندازهٔ خروجی‌اند.
            for (int j = 0; j < layer.Biases.Length; j++) // پارامترهای هر نورون به‌صورت مستقل ساخته می‌شوند.
            {
                layer.Weights[j] = new double[sizes[l]]; // هر نورون به تمام خروجی‌های لایهٔ قبلی متصل است.
                for (int k = 0; k < sizes[l]; k++) // برای هر اتصال یک وزن تصادفی تعیین می‌شود.
                    layer.Weights[j][k] = (2 * random.NextDouble() - 1) * limit; // توزیع یکنواخت حول صفر تقارن نورون‌ها را می‌شکند.
                layer.Biases[j] = (2 * random.NextDouble() - 1) * 0.01; // بایاس تصادفی کوچک شروع را نزدیک ناحیهٔ غیر اشباع نگه می‌دارد.
            }
            layers[l] = layer; // لایهٔ آماده در معماری ثبت می‌شود.
        }
        return new NeuralNetwork(layers); // شبکهٔ تازه هیچ دانشی از ضرب ندارد.
    }

    public double[][] Forward(double[] input) // خروجی همهٔ لایه‌ها برای استفاده در پس‌انتشار نگهداری می‌شود.
    {
        if (input.Length != 2 || input.Any(x => !double.IsFinite(x))) // فقط دو ورودی متناهی پذیرفته می‌شود.
            throw new ArgumentException("شبکه دو ورودی متناهی لازم دارد."); // ناسازگاری ابعاد پیش از ضرب ماتریسی گزارش می‌شود.
        var activations = new double[Layers.Length + 1][]; // خانهٔ صفر متعلق به ورودی خام نرمال‌شده است.
        activations[0] = input; // ورودی اولین لایه بدون تغییر ذخیره می‌شود.
        for (int l = 0; l < Layers.Length; l++) // خروجی هر لایه ورودی لایهٔ بعدی است.
        {
            var layer = Layers[l]; // پارامترهای لایهٔ جاری انتخاب می‌شوند.
            var output = new double[layer.Biases.Length]; // برای هر نورون یک مقدار خروجی تخصیص می‌یابد.
            for (int j = 0; j < output.Length; j++) // محاسبهٔ ضرب داخلی برای تک‌تک نورون‌ها انجام می‌شود.
            {
                double z = layer.Biases[j]; // جمع وزن‌دار با بایاس شروع می‌شود.
                for (int k = 0; k < activations[l].Length; k++) // تمام ورودی‌های نورون در مجموع شرکت می‌کنند.
                    z += layer.Weights[j][k] * activations[l][k]; // حاصل‌ضرب وزن و فعال‌سازی قبلی به پیش‌فعال‌سازی افزوده می‌شود.
                output[j] = l == Layers.Length - 1 ? z : Math.Tanh(z); // خروجی خطی، پیش‌بینی پیوسته و بدون محدودسازی تولید می‌کند.
                if (!double.IsFinite(output[j])) // واگرایی محاسبات نباید به‌عنوان نتیجهٔ معتبر نمایش داده شود.
                    throw new ArithmeticException("شبکه واگرا شد؛ نرخ یادگیری را کاهش دهید و مدل تازه بسازید."); // خطای عددی به کاربر منتقل می‌شود.
            }
            activations[l + 1] = output; // فعال‌سازی برای لایهٔ بعد و محاسبهٔ مشتق ذخیره می‌شود.
        }
        return activations; // کش محاسبات برای همین نمونه بازگردانده می‌شود.
    }

    public double Predict(double[] input) => Forward(input)[^1][0]; // پیش‌بینی فقط از پارامترهای شبکه محاسبه می‌شود.

    public static double TanhDerivative(double activation) => 1 - activation * activation; // مشتق tanh از خروجی آن به دست می‌آید.

    public Gradients ComputeGradients(double[] input, double target) // تابع هزینهٔ هر نمونه نصف مربع اختلاف است.
    {
        var activations = Forward(input); // ابتدا مقادیر لازم برای قانون زنجیره‌ای محاسبه می‌شوند.
        var weightGradients = new double[Layers.Length][][]; // شکل گرادیان وزن دقیقاً شکل پارامترهاست.
        var biasGradients = new double[Layers.Length][]; // گرادیان بایاس همان دلتا در هر لایه است.
        double[] delta = [activations[^1][0] - target]; // مشتق نصف مربع خطا و مشتق خروجی خطی ترکیب می‌شوند.
        for (int l = Layers.Length - 1; l >= 0; l--) // خطا از خروجی به سوی ورودی انتشار می‌یابد.
        {
            var layer = Layers[l]; // وزن‌های فعلی هنوز به‌روزرسانی نشده‌اند.
            biasGradients[l] = delta; // مشتق جمع نسبت به بایاس برابر یک است.
            weightGradients[l] = new double[delta.Length][]; // هر دلتا به یک سطر وزن مربوط است.
            for (int j = 0; j < delta.Length; j++) // گرادیان اتصالات ورودی هر نورون ساخته می‌شود.
            {
                weightGradients[l][j] = new double[activations[l].Length]; // تعداد گرادیان‌ها برابر تعداد ورودی‌های نورون است.
                for (int k = 0; k < activations[l].Length; k++) // قانون زنجیره‌ای برای هر وزن اعمال می‌شود.
                    weightGradients[l][j][k] = delta[j] * activations[l][k]; // مشتق نسبت به وزن برابر دلتا ضرب‌در ورودی است.
            }
            if (l == 0) // ورودی خام پارامتر قابل یادگیری یا تابع فعال‌سازی ندارد.
                continue; // محاسبهٔ دلتا برای قبل از نخستین لایه لازم نیست.
            var previousDelta = new double[activations[l].Length]; // دلتاهای لایهٔ مخفی قبلی تخصیص می‌یابند.
            for (int k = 0; k < previousDelta.Length; k++) // سهم تمام نورون‌های بعدی در هر نورون قبلی جمع می‌شود.
            {
                double propagated = 0; // جمع وزن‌دار خطا از صفر آغاز می‌شود.
                for (int j = 0; j < delta.Length; j++) // ترانهادهٔ ماتریس وزن به‌صورت حلقه پیاده می‌شود.
                    propagated += layer.Weights[j][k] * delta[j]; // وزن قدیمی سهم اتصال را به خطای قبلی منتقل می‌کند.
                previousDelta[k] = propagated * TanhDerivative(activations[l][k]); // مشتق فعال‌سازی قانون زنجیره‌ای را کامل می‌کند.
            }
            delta = previousDelta; // مرحلهٔ بعد با دلتاهای یک لایه عقب‌تر انجام می‌شود.
        }
        return new Gradients(weightGradients, biasGradients); // گرادیان همهٔ پارامترها یکجا تحویل داده می‌شود.
    }

    public void TrainSample(Sample sample, double rate) // یک گام SGD با یک نمونه انجام می‌شود.
    {
        var gradients = ComputeGradients(sample.Input, sample.Target); // همهٔ مشتق‌ها با وزن‌های قبل از گام به دست می‌آیند.
        for (int l = 0; l < Layers.Length; l++) // اکنون به‌روزرسانی مستقل هر لایه مجاز است.
        {
            for (int j = 0; j < Layers[l].Biases.Length; j++) // هر نورون بایاس و اتصالات خود را تغییر می‌دهد.
            {
                for (int k = 0; k < Layers[l].Weights[j].Length; k++) // گرادیان کاهشی در جهت مخالف مشتق حرکت می‌کند.
                    Layers[l].Weights[j][k] -= rate * gradients.Weights[l][j][k]; // اندازهٔ تغییر وزن با نرخ یادگیری کنترل می‌شود.
                Layers[l].Biases[j] -= rate * gradients.Biases[l][j]; // بایاس نیز با همان نرخ بهینه می‌شود.
            }
        }
    }
}
