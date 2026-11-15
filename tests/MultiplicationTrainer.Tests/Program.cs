using MultiplicationTrainer; // آزمون‌ها مستقیماً همان هستهٔ مورد استفادهٔ برنامه را بررسی می‌کنند.

namespace MultiplicationTrainer.Tests; // آزمون‌ها از کد برنامه جدا نگه داشته می‌شوند.

public static class Program // یک اجراکنندهٔ ساده بدون چارچوب یا بستهٔ آزمایش اضافی است.
{
    public static int Main(string[] args) // شکست آزمون با کد خروج غیرصفر مشخص می‌شود.
    {
        try // اولین شکست، جزئیات خطای قابل بررسی را گزارش می‌کند.
        {
            CheckDataAndDimensions(); // ابعاد شبکه و قرارداد مقیاس ابتدا بررسی می‌شوند.
            CheckGradients(); // مشتق تحلیلی با تفاضل محدود مستقل مقایسه می‌شود.
            CheckUpdate(); // جهت و اندازهٔ گام گرادیان کنترل می‌شود.
            CheckRoundTripAndResume(); // بازیابی پارامترها و ادامهٔ تکرارپذیر بررسی می‌شوند.
            CheckHoldoutIsolation(); // یک نمونهٔ کنارگذاشته‌شده نباید در گام‌های آموزش شرکت کند.
            CheckInvalidModels(); // مدل خراب قبل از پیش‌بینی رد می‌شود.
            if (args.Contains("--sql")) // آزمون اتصال واقعی فقط با درخواست صریح اجرا می‌شود.
                CheckSqlRoundTrip(); // این مسیر واقعاً INSERT و SELECT در SQL Server انجام می‌دهد.
            else // نبود SQL باعث ادعای موفقیت آزمون پایگاه داده نمی‌شود.
                Console.WriteLine("SKIP: SQL integration test; use --sql with SQLSERVER_CONNECTION_STRING."); // محدودیت اجرای فعلی شفاف چاپ می‌شود.
            Console.WriteLine("PASS: all requested checks."); // این پیام فقط پس از عبور همهٔ آزمون‌های درخواستی چاپ می‌شود.
            return 0; // قرارداد استاندارد موفقیت برای اسکریپت‌ها رعایت می‌شود.
        }
        catch (Exception exception) // هر خطا به شکست واقعی اجرای آزمون تبدیل می‌شود.
        {
            Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}"); // محل منطقی شکست از پیام آزمون مشخص می‌شود.
            return 1; // CI می‌تواند شکست را از کد خروج تشخیص دهد.
        }
    }

    private static void Assert(bool condition, string message) // شرط‌های آزمون در همهٔ حالت‌های ساخت فعال‌اند.
    {
        if (!condition) // برخلاف Debug.Assert، نسخهٔ Release نیز بررسی را اجرا می‌کند.
            throw new InvalidOperationException(message); // شکست بلافاصله اجرای آزمون را متوقف می‌کند.
    }

    private static void Near(double actual, double expected, double tolerance, string message) // مقایسهٔ عددی شامل کنترل اعداد نامتناهی است.
    {
        Assert(double.IsFinite(actual) && double.IsFinite(expected) && Math.Abs(actual - expected) <= tolerance, $"{message}: {actual:R} != {expected:R}"); // NaN نمی‌تواند تصادفاً از آزمون عبور کند.
    }

    private static void CheckDataAndDimensions() // قرارداد دو ورودی و یک خروجی و تمام زوج‌ها بررسی می‌شود.
    {
        var settings = new Settings { HiddenSizes = [3, 2] }; // معماری کوچک و نامتقارن اشتباه ترانهاده را آشکار می‌کند.
        var samples = DataSet.Create(settings); // دادهٔ اصلی پروژه مورد آزمایش قرار می‌گیرد.
        Assert(samples.Length == 100 && samples.Select(s => (s.A, s.B)).Distinct().Count() == 100, "All ordered pairs required"); // هیچ زوجی نباید تکرار یا حذف شود.
        Near(samples[0].Input[0], -1, 0, "Input minimum"); // کران پایین باید دقیقاً منفی یک شود.
        Near(samples[^1].Input[1], 1, 0, "Input maximum"); // کران بالا باید دقیقاً مثبت یک شود.
        Near(samples[0].Target, 0.01, 0, "Output normalization"); // هدف ۱ در مقیاس صد به ۰٫۰۱ تبدیل می‌شود.
        var activations = NeuralNetwork.Create(settings).Forward([0.2, -0.3]); // انتشار با دو ورودی نرمال‌شده انجام می‌شود.
        Assert(activations.Select(a => a.Length).SequenceEqual(new[] { 2, 3, 2, 1 }), "Layer dimensions"); // اندازهٔ همهٔ فعال‌سازی‌ها کنترل می‌شود.
        Console.WriteLine("PASS: data, normalization, matrix dimensions."); // نتیجهٔ آزمون مستقل گزارش می‌شود.
    }

    private static double Loss(NeuralNetwork network, Sample sample) // هزینهٔ مستقل برای مشتق عددی تعریف می‌شود.
    {
        double difference = network.Predict(sample.Input) - sample.Target; // خروجی فعلی با هدف ثابت مقایسه می‌شود.
        return 0.5 * difference * difference; // همان قرارداد نصف مربع خطا استفاده می‌شود.
    }

    private static void CheckGradients() // تمام وزن‌ها و بایاس‌های دو معماری بررسی می‌شوند.
    {
        foreach (int[] hidden in new[] { new[] { 3 }, new[] { 3, 2 } }) // هر دو حالت یک و دو لایهٔ مخفی آزموده می‌شوند.
        {
            var network = NeuralNetwork.Create(new Settings { HiddenSizes = hidden }); // شبکهٔ کوچک هزینهٔ آزمون را پایین نگه می‌دارد.
            var sample = new Sample(3, 7, [0.23, -0.41], 0.21); // ورودی نامتقارن خطای جابه‌جایی ابعاد را آشکار می‌کند.
            var gradients = network.ComputeGradients(sample.Input, sample.Target); // مشتق تحلیلی قبل از هر اختلال گرفته می‌شود.
            const double epsilon = 1e-6; // گام تفاضل مرکزی بین خطای گردکردن و تقریب توازن ایجاد می‌کند.
            for (int l = 0; l < network.Layers.Length; l++) // تمام لایه‌ها بدون گزینش نمونه بررسی می‌شوند.
            {
                var layer = network.Layers[l]; // پارامترهای لایهٔ جاری به‌طور موقت تغییر می‌کنند.
                for (int j = 0; j < layer.Biases.Length; j++) // بایاس و تمام وزن‌های هر نورون آزموده می‌شوند.
                {
                    for (int k = 0; k < layer.Weights[j].Length; k++) // هر وزن جداگانه مختل می‌شود.
                    {
                        double original = layer.Weights[j][k]; // مقدار اصلی برای بازگردانی نگهداری می‌شود.
                        layer.Weights[j][k] = original + epsilon; // اختلال مثبت روی یک پارامتر اعمال می‌شود.
                        double plus = Loss(network, sample); // هزینهٔ سمت مثبت اندازه‌گیری می‌شود.
                        layer.Weights[j][k] = original - epsilon; // اختلال منفی با همان اندازه اعمال می‌شود.
                        double minus = Loss(network, sample); // هزینهٔ سمت منفی اندازه‌گیری می‌شود.
                        layer.Weights[j][k] = original; // شبکه پیش از بررسی پارامتر بعدی به حالت اولیه برمی‌گردد.
                        Near(gradients.Weights[l][j][k], (plus - minus) / (2 * epsilon), 1e-7, "Weight derivative"); // مشتق تحلیلی با تقریب مرکزی مستقل مقایسه می‌شود.
                    }
                    double bias = layer.Biases[j]; // بایاس نیز مانند وزن یک پارامتر قابل آموزش است.
                    layer.Biases[j] = bias + epsilon; // اختلال مثبت بایاس اعمال می‌شود.
                    double upper = Loss(network, sample); // هزینهٔ بالاتر محاسبه می‌شود.
                    layer.Biases[j] = bias - epsilon; // اختلال منفی بایاس اعمال می‌شود.
                    double lower = Loss(network, sample); // هزینهٔ پایین‌تر محاسبه می‌شود.
                    layer.Biases[j] = bias; // بایاس به مقدار اصلی بازگردانده می‌شود.
                    Near(gradients.Biases[l][j], (upper - lower) / (2 * epsilon), 1e-7, "Bias derivative"); // دلتا باید با مشتق عددی بایاس یکسان باشد.
                }
            }
        }
        Console.WriteLine("PASS: finite-difference gradients for every weight and bias."); // بررسی کامل مشتق‌ها تأیید می‌شود.
    }

    private static void CheckUpdate() // یک گام کوچک باید هم فرمول و هم کاهش هزینه را رعایت کند.
    {
        var model = ModelState.Create(new Settings { HiddenSizes = [3, 2] }); // مدل کوچک برای بررسی پارامتر به پارامتر ساخته می‌شود.
        var original = ModelCodec.Decode(ModelCodec.Encode(model)); // نسخهٔ مستقل مانع اشتراک آرایه‌ها در مقایسه می‌شود.
        var network = model.Network(); // شبکهٔ مورد آزمون به پارامترهای مدل متصل است.
        var sample = DataSet.Create(model.Settings)[36]; // نمونهٔ ثابت اجرای آزمون را تکرارپذیر می‌کند.
        var gradients = network.ComputeGradients(sample.Input, sample.Target); // جهت صحیح تغییر از گرادیان پیش از گام گرفته می‌شود.
        double before = Loss(network, sample); // هزینهٔ قبل از تغییر ثبت می‌شود.
        const double rate = 0.001; // گام کوچک برای بررسی کاهش محلی هزینه کافی است.
        network.TrainSample(sample, rate); // مسیر واقعی به‌روزرسانی برنامه اجرا می‌شود.
        for (int l = 0; l < model.Layers.Length; l++) // هیچ لایه‌ای از بررسی تغییر مستثنا نیست.
        {
            for (int j = 0; j < model.Layers[l].Biases.Length; j++) // تغییر هر نورون کنترل می‌شود.
            {
                Near(model.Layers[l].Biases[j], original.Layers[l].Biases[j] - rate * gradients.Biases[l][j], 1e-14, "Bias update"); // علامت و نرخ تغییر بایاس بررسی می‌شوند.
                for (int k = 0; k < model.Layers[l].Weights[j].Length; k++) // تمام اتصالات با فرمول SGD مقایسه می‌شوند.
                    Near(model.Layers[l].Weights[j][k], original.Layers[l].Weights[j][k] - rate * gradients.Weights[l][j][k], 1e-14, "Weight update"); // خطای ترتیب یا علامت به‌روزرسانی آشکار می‌شود.
            }
        }
        Assert(Loss(network, sample) < before, "Small gradient step should reduce loss"); // آزمون رفتاری کاهش هزینه مکمل بررسی فرمول است.
        Console.WriteLine("PASS: SGD updates and loss decrease."); // نتیجهٔ گام آموزشی گزارش می‌شود.
    }

    private static void ComparePredictions(ModelState first, ModelState second) // مقایسه فقط به چند نمونهٔ منتخب محدود نیست.
    {
        foreach (var sample in DataSet.Create(first.Settings)) // تمام زوج‌های دامنه پیمایش می‌شوند.
            Near(first.Predict(sample.A, sample.B), second.Predict(sample.A, sample.B), 1e-12, "Round-trip prediction"); // تلورانس در مقیاس واقعی حاصل‌ضرب اعمال می‌شود.
    }

    private static void CheckRoundTripAndResume() // ادامهٔ آموزش باید با اجرای پیوسته یکسان باشد.
    {
        var model = ModelState.Create(new Settings { HiddenSizes = [4, 3], HoldoutFraction = 0.2 }); // تقسیم داده نیز بخشی از وضعیت آزمون است.
        Trainer.Train(model, 8); // مدل پیش از ذخیره چند دوره یاد می‌گیرد.
        var restored = ModelCodec.Decode(ModelCodec.Encode(model)); // همان قالب مورد استفادهٔ SQL رفت‌وبرگشت می‌شود.
        ComparePredictions(model, restored); // همهٔ پیش‌بینی‌ها قبل و بعد از تبدیل مقایسه می‌شوند.
        Trainer.Train(model, 5); // مسیر پیوسته از وضعیت اصلی ادامه می‌دهد.
        Trainer.Train(restored, 5); // مسیر بازیابی‌شده همان دوره‌ها را اجرا می‌کند.
        ComparePredictions(model, restored); // برابری پس از ادامه، شمارنده و بذر دوره را نیز می‌آزماید.
        Assert(restored.CompletedEpochs == 13 && model.HeldOutIndices.SequenceEqual(restored.HeldOutIndices), "Persisted metadata"); // شمارنده و تقسیم داده نباید تغییر کنند.
        Console.WriteLine("PASS: JSON round-trip and deterministic continuation (tolerance 1e-12)."); // این آزمون با آزمون واقعی SQL اشتباه گرفته نمی‌شود.
    }

    private static void CheckHoldoutIsolation() // یک دورهٔ واقعی با اجرای دستی فقط نمونه‌های مجاز مقایسه می‌شود.
    {
        var model = ModelState.Create(new Settings { HiddenSizes = [3], HoldoutFraction = 0.2 }); // بیست زوج باید از آموزش حذف شوند.
        var expected = ModelCodec.Decode(ModelCodec.Encode(model)); // نسخهٔ مستقل برای محاسبهٔ مرجع ساخته می‌شود.
        var samples = DataSet.Create(model.Settings); // دادهٔ مرجع با ترتیب اصلی ساخته می‌شود.
        int[] order = Enumerable.Range(0, samples.Length).Except(model.HeldOutIndices).ToArray(); // مجموعهٔ مرجع فقط هشتاد نمونه دارد.
        Assert(order.Length == 80 && model.HeldOutIndices.Length == 20, "80/20 split"); // اندازهٔ دو بخش صریح کنترل می‌شود.
        DataSet.Shuffle(order, new Random(model.Settings.Seed)); // بذر نخستین دوره مستقل بازسازی می‌شود.
        foreach (int index in order) // مرجع به هیچ نمونهٔ کنارگذاشته‌شده دسترسی آموزشی ندارد.
            expected.Network().TrainSample(samples[index], expected.Settings.LearningRate); // دقیقاً یک دورهٔ دستی روی دادهٔ مجاز اجرا می‌شود.
        Trainer.Train(model, 1); // مسیر اصلی برنامه برای مقایسه اجرا می‌شود.
        ComparePredictions(model, expected); // ورود اشتباه حتی یک نمونه معمولاً برابری پارامترها را از بین می‌برد.
        Console.WriteLine("PASS: held-out samples excluded from training."); // استقلال دادهٔ ارزیابی تأیید می‌شود.
    }

    private static void CheckInvalidModels() // خطاهای قالب و عددی باید پیش از استفاده کشف شوند.
    {
        foreach (Action<ModelState> corrupt in new Action<ModelState>[] { m => m.FormatVersion = 99, m => m.Layers[0].Weights[0] = [1], m => m.Layers[0].Biases[0] = double.NaN, m => m.HeldOutIndices = [999] }) // چند نوع خرابی مستقل بررسی می‌شود.
        {
            var model = ModelState.Create(new Settings()); // هر خرابی روی مدل سالم تازه اعمال می‌شود.
            corrupt(model); // فقط یک شرط اعتبارسنجی در هر بار شکسته می‌شود.
            bool rejected = false; // پذیرش بی‌صدای دادهٔ خراب شکست محسوب می‌شود.
            try { model.Validate(); } // مسیر واقعی اعتبارسنجی آزموده می‌شود.
            catch (InvalidDataException) { rejected = true; } // تنها رد صریح دادهٔ نامعتبر موفقیت آزمون است.
            Assert(rejected, "Corrupt model must be rejected"); // مدل خراب نباید قابل استفاده باشد.
        }
        Console.WriteLine("PASS: corrupt models rejected."); // بررسی حالت‌های خطا گزارش می‌شود.
    }

    private static void CheckSqlRoundTrip() // این آزمون به SQL Server و جدول نصب‌شده نیاز دارد.
    {
        var model = ModelState.Create(new Settings { HiddenSizes = [4, 3], HoldoutFraction = 0.2 }); // مدل آزمایش دارای معماری و تقسیم قابل تشخیص است.
        Trainer.Train(model, 10); // وزن‌ها پیش از ذخیره از مقدار اولیه فاصله می‌گیرند.
        var repository = new SqlModelRepository(); // رشتهٔ اتصال از همان متغیر محیطی برنامه خوانده می‌شود.
        Guid id = repository.Save(model, "integration-test round-trip"); // تراکنش واقعی یک تصویر مستقل درج می‌کند.
        var restored = repository.Load(id); // خواندن واقعی پایگاه داده اجرا می‌شود.
        ComparePredictions(model, restored); // تمام صد پیش‌بینی با تلورانس بسیار کوچک مقایسه می‌شوند.
        Assert(ModelCodec.Encode(model) == ModelCodec.Encode(restored), "SQL metadata and parameters"); // علاوه بر خروجی، کل وضعیت مدل باید برابر باشد.
        Trainer.Train(model, 2); // مسیر اصلی دو دورهٔ دیگر آموزش می‌بیند.
        Trainer.Train(restored, 2); // مدل بازیابی‌شده باید همان مسیر را طی کند.
        ComparePredictions(model, restored); // قابلیت ادامهٔ آموزش پس از خواندن SQL نیز بررسی می‌شود.
        Console.WriteLine($"PASS: SQL save/load predictions within 1e-12; test snapshot retained: {id}"); // شناسهٔ رکورد آزمایشی برای بررسی بعدی چاپ می‌شود.
    }
}
