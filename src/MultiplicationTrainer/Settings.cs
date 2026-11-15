// همهٔ کلاس‌های آموزشی در یک فضای نام مشترک قرار می‌گیرند.
namespace MultiplicationTrainer;

// این تنظیمات همراه وزن‌ها ذخیره می‌شوند تا معنای مدل پس از بارگذاری تغییر نکند.
public sealed class Settings
{
    public int Minimum { get; set; } = 1; // کران پایین ورودی در تولید داده و تبدیل مقیاس مشترک است.
    public int Maximum { get; set; } = 10; // کران بالا مقیاس خروجی را نیز تعیین می‌کند.
    public int[] HiddenSizes { get; set; } = [16, 16]; // یک یا دو لایهٔ مخفی مجاز است.
    public double LearningRate { get; set; } = 0.03; // اندازهٔ گام اولیهٔ گرادیان کاهشی است.
    public double DecayEpochs { get; set; } = 5000; // نرخ با گذشت این تعداد دوره به نصف می‌رسد.
    public int EpochsPerRun { get; set; } = 12000; // هر بار انتخاب آموزش، این تعداد دوره اضافه می‌کند.
    public int ReportEvery { get; set; } = 1000; // فاصلهٔ گزارش‌ها بر حسب دوره است.
    public int Seed { get; set; } = 42; // بذر ثابت، آزمایش را قابل تکرار می‌کند.
    public double HoldoutFraction { get; set; } = 0; // صفر یعنی آموزش روی کل جدول؛ مثلاً ۰٫۲ یعنی جداسازی بیست درصد.
    public double OutputScale => (double)Maximum * Maximum; // خروجی واقعی با تقسیم بر این مقدار نرمال می‌شود.

    // تنظیمات نامعتبر پیش از ساخت آرایه یا آغاز آموزش رد می‌شوند.
    public void Validate()
    {
        if (Minimum < 1 || Maximum <= Minimum || Maximum > 100) // دامنهٔ مثبت و محدود، حجم آزمایش را کنترل می‌کند.
            throw new ArgumentException("محدوده باید مثبت باشد و 1 <= Minimum < Maximum <= 100 برقرار باشد."); // خطا به جای تولید دادهٔ نامعتبر اعلام می‌شود.
        if (HiddenSizes is null || HiddenSizes.Length is < 1 or > 2 || HiddenSizes.Any(n => n is < 1 or > 128)) // ابعاد شبکه باید کوچک و غیرصفر باشند.
            throw new ArgumentException("یک یا دو لایهٔ مخفی، هرکدام با ۱ تا ۱۲۸ نورون لازم است."); // علت ناسازگاری معماری مشخص می‌شود.
        if (!double.IsFinite(LearningRate) || LearningRate <= 0 || LearningRate > 1 || !double.IsFinite(DecayEpochs) || DecayEpochs <= 0) // اعداد نامتناهی اجازهٔ ورود به محاسبات ندارند.
            throw new ArgumentException("نرخ یادگیری باید در (0,1] و دورهٔ کاهش نرخ مثبت باشد."); // نرخ غیرمجاز زود تشخیص داده می‌شود.
        if (EpochsPerRun < 1 || EpochsPerRun > 1000000 || ReportEvery < 1) // تعداد دوره و فاصلهٔ گزارش باید معتبر باشند.
            throw new ArgumentException("تعداد دوره باید ۱ تا یک میلیون و فاصلهٔ گزارش مثبت باشد."); // حلقهٔ نامعتبر اجرا نمی‌شود.
        if (!double.IsFinite(HoldoutFraction) || HoldoutFraction < 0 || HoldoutFraction > 0.5) // دست‌کم نصف نمونه‌ها برای یادگیری باقی می‌ماند.
            throw new ArgumentException("نسبت دادهٔ کنارگذاشته‌شده باید بین صفر و نیم باشد."); // تقسیم نامعتبر مجموعه متوقف می‌شود.
    }
}
