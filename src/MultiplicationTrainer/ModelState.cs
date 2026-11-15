using System.Text.Json; // قالب JSON اعداد double و آرایه‌های تو در تو را بدون وابستگی اضافی نگه می‌دارد.

namespace MultiplicationTrainer; // قالب ذخیره‌سازی بخشی از قرارداد مشترک برنامه است.

public sealed class ModelState // این شیء تمام اطلاعات لازم برای بازسازی مدل را در خود دارد.
{
    public int FormatVersion { get; set; } = 1; // تغییر قرارداد فایل باید با نسخهٔ تازه همراه باشد.
    public string HiddenActivation { get; set; } = "tanh"; // نوع تابع مخفی برای جلوگیری از تفسیر اشتباه ثبت می‌شود.
    public string OutputActivation { get; set; } = "linear"; // خروجی مدل یک رگرسیون خطی است.
    public string Normalization { get; set; } = "input:minmax[-1,1];output:y/max^2"; // فرمول تبدیل مقیاس صریحاً ذخیره می‌شود.
    public Settings Settings { get; set; } = new(); // معماری، مقیاس، بذر و تنظیمات آموزش همراه مدل هستند.
    public int CompletedEpochs { get; set; } // ادامهٔ برنامه زمان‌بندی نرخ و ترتیب نمونه‌ها را از این عدد پی می‌گیرد.
    public int[] HeldOutIndices { get; set; } = []; // نمونه‌های کنارگذاشته‌شده در ادامهٔ آموزش نیز دیده نمی‌شوند.
    public DenseLayer[] Layers { get; set; } = []; // تمام وزن‌ها و بایاس‌ها با دقت double ذخیره می‌شوند.

    public static ModelState Create(Settings settings) // مدل تازه هیچ اطلاعاتی از پایگاه داده نمی‌خواند.
    {
        settings.Validate(); // تنظیمات قبل از ساخت مدل بررسی می‌شوند.
        return new ModelState { Settings = settings, HeldOutIndices = DataSet.HeldOutIndices(settings), Layers = NeuralNetwork.Create(settings).Layers }; // تقسیم داده و وزن‌های اولیه یکجا ساخته می‌شوند.
    }

    public NeuralNetwork Network() => new(Layers); // شبکه به پارامترهای همین وضعیت متصل می‌شود تا آموزش آن‌ها را تغییر دهد.

    public double Predict(int a, int b) // ورودی کاربر در این نقطه بررسی و نرمال می‌شود.
    {
        if (a < Settings.Minimum || a > Settings.Maximum || b < Settings.Minimum || b > Settings.Maximum) // پیش‌بینی خارج از دامنهٔ مورد آموزش مجاز نیست.
            throw new ArgumentOutOfRangeException(nameof(a), "هر دو عدد باید در محدودهٔ تنظیم‌شده باشند."); // محدودیت دامنه روشن اعلام می‌شود.
        return Network().Predict([DataSet.NormalizeInput(a, Settings), DataSet.NormalizeInput(b, Settings)]) * Settings.OutputScale; // فقط خروجی شبکه به مقیاس واقعی برمی‌گردد.
    }

    public void Validate() // دادهٔ ذخیره‌شده پیش از استفاده به‌طور ساختاری و عددی بررسی می‌شود.
    {
        if (FormatVersion != 1 || HiddenActivation != "tanh" || OutputActivation != "linear" || Normalization != "input:minmax[-1,1];output:y/max^2") // نسخه یا فرمول ناشناخته نباید بی‌صدا تفسیر شود.
            throw new InvalidDataException("نسخه، تابع فعال‌سازی یا نرمال‌سازی مدل پشتیبانی نمی‌شود."); // مدل ناسازگار رد می‌شود.
        if (Settings is null || CompletedEpochs < 0 || HeldOutIndices is null || Layers is null) // مقادیر ضروری نباید حذف یا منفی باشند.
            throw new InvalidDataException("اطلاعات پایهٔ مدل ناقص است."); // خرابی داده پیش از دسترسی به آرایه‌ها اعلام می‌شود.
        Settings.Validate(); // تنظیمات بازیابی‌شده نیز همان قواعد مدل تازه را رعایت می‌کنند.
        int[] sizes = [2, .. Settings.HiddenSizes, 1]; // ابعاد مورد انتظار از معماری ذخیره‌شده استخراج می‌شوند.
        if (Layers.Length != sizes.Length - 1) // تعداد ماتریس‌ها باید با معماری یکسان باشد.
            throw new InvalidDataException("تعداد لایه‌های ذخیره‌شده نادرست است."); // ناسازگاری معماری متوقف می‌شود.
        for (int l = 0; l < Layers.Length; l++) // شکل و محتوای همهٔ پارامترها بررسی می‌شود.
        {
            var layer = Layers[l]; // هر لایه جداگانه با قرارداد معماری مقایسه می‌شود.
            if (layer is null || layer.Weights is null || layer.Biases is null || layer.Weights.Length != sizes[l + 1] || layer.Biases.Length != sizes[l + 1]) // تعداد نورون‌ها باید در وزن و بایاس برابر باشد.
                throw new InvalidDataException("ابعاد لایهٔ ذخیره‌شده نادرست است."); // پارامتر ناقص وارد محاسبات نمی‌شود.
            if (layer.Biases.Any(x => !double.IsFinite(x)) || layer.Weights.Any(row => row is null || row.Length != sizes[l] || row.Any(x => !double.IsFinite(x)))) // هر سطر ورودی کافی و اعداد متناهی لازم دارد.
                throw new InvalidDataException("وزن یا بایاس نامعتبر است."); // NaN و بی‌نهایت قابل قبول نیستند.
        }
        int count = DataSet.Create(Settings).Length; // دامنهٔ اندیس‌های داده تعیین می‌شود.
        int expected = Settings.HoldoutFraction == 0 ? 0 : Math.Max(1, (int)(count * Settings.HoldoutFraction)); // اندازهٔ تقسیم باید با تنظیمات منطبق باشد.
        if (HeldOutIndices.Length != expected || HeldOutIndices.Distinct().Count() != expected || HeldOutIndices.Any(i => i < 0 || i >= count)) // اندیس تکراری یا خارج از محدوده نشانهٔ خرابی است.
            throw new InvalidDataException("تقسیم دادهٔ ذخیره‌شده نامعتبر است."); // ارزیابی روی تقسیم اشتباه انجام نمی‌شود.
    }
}

public static class ModelCodec // یک مسیر مشترک برای ذخیرهٔ SQL و آزمون رفت‌وبرگشت استفاده می‌شود.
{
    public static string Encode(ModelState model) // مدل معتبر به متن JSON تبدیل می‌شود.
    {
        model.Validate(); // ذخیرهٔ مدل ناقص یا واگرا رد می‌شود.
        return JsonSerializer.Serialize(model); // قالب عددی استاندارد دقت لازم برای بازیابی double را حفظ می‌کند.
    }

    public static ModelState Decode(string json) // JSON به یک مدل مستقل با آرایه‌های تازه تبدیل می‌شود.
    {
        var model = JsonSerializer.Deserialize<ModelState>(json) ?? throw new InvalidDataException("مدل خالی است."); // دادهٔ تهی مدل معتبر محسوب نمی‌شود.
        model.Validate(); // ابعاد و نسخه قبل از ارائه به کاربر بررسی می‌شوند.
        return model; // مدل بازسازی‌شده آمادهٔ پیش‌بینی یا ادامهٔ آموزش است.
    }
}
