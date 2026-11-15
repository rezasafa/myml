namespace MultiplicationTrainer; // ارزیابی مستقل از حلقهٔ بهینه‌سازی اجرا می‌شود.

public sealed record Metrics(int Count, double MeanAbsoluteError, double MaximumAbsoluteError, int RoundedCorrect); // معیارها در مقیاس واقعی حاصل‌ضرب گزارش می‌شوند.

public static class Evaluation // هیچ اصلاحی روی پیش‌بینی خام انجام نمی‌دهد.
{
    public static Metrics Measure(ModelState model, IEnumerable<Sample> samples) // یک زیرمجموعهٔ مشخص ارزیابی می‌شود.
    {
        var rows = samples.Select(s => (Expected: s.A * (double)s.B, Predicted: model.Predict(s.A, s.B))).ToArray(); // جواب واقعی صرفاً برای مقایسه محاسبه می‌شود.
        if (rows.Length == 0) // مجموعهٔ خالی میانگین معنادار ندارد.
            throw new ArgumentException("مجموعهٔ ارزیابی خالی است."); // تقسیم بر صفر یا گزارش گمراه‌کننده جلوگیری می‌شود.
        double[] errors = rows.Select(r => Math.Abs(r.Predicted - r.Expected)).ToArray(); // قدر مطلق، خنثی‌شدن خطاهای مثبت و منفی را حذف می‌کند.
        int correct = rows.Count(r => Math.Round(r.Predicted, MidpointRounding.AwayFromZero) == r.Expected); // تنها این معیار از گردکردن استفاده می‌کند.
        return new Metrics(rows.Length, errors.Average(), errors.Max(), correct); // میانگین و بیشینه بر اساس خروجی خام هستند.
    }

    public static void Print(ModelState model) // جدول کامل در هر دو حالت آموزش نمایش داده می‌شود.
    {
        var samples = DataSet.Create(model.Settings); // همان ترتیب ثابت، اندیس‌های تقسیم را قابل تفسیر می‌کند.
        var heldOut = model.HeldOutIndices.ToHashSet(); // هر ردیف با نقش واقعی آن برچسب می‌خورد.
        Console.WriteLine("A × B | واقعی | پیش‌بینی خام | خطای مطلق | بخش"); // ستون‌ها تفاوت هدف و پیش‌بینی را روشن می‌کنند.
        for (int i = 0; i < samples.Length; i++) // هیچ زوجی از گزارش حذف نمی‌شود.
        {
            var sample = samples[i]; // نمونهٔ جاری برای چاپ انتخاب می‌شود.
            double predicted = model.Predict(sample.A, sample.B); // پیش‌بینی فقط از شبکه گرفته می‌شود.
            double expected = sample.A * (double)sample.B; // حقیقت مبنا مستقل محاسبه می‌شود.
            string partition = heldOut.Contains(i) ? "دیده‌نشده" : "آموزش"; // نقش هر ردیف بدون ابهام چاپ می‌شود.
            Console.WriteLine($"{sample.A,2} × {sample.B,2} | {expected,5:F0} | {predicted,12:F6} | {Math.Abs(predicted - expected),10:F6} | {partition}"); // دقت خام همراه خطا نمایش داده می‌شود.
        }
        PrintMetrics("تمام زوج‌ها", Measure(model, samples)); // کیفیت کل جدول همیشه گزارش می‌شود.
        if (heldOut.Count > 0) // ارزیابی تعمیم فقط هنگام کنارگذاشتن واقعی داده معنا دارد.
        {
            PrintMetrics("فقط آموزش", Measure(model, samples.Where((_, i) => !heldOut.Contains(i)))); // خطای آموزشی از خطای تعمیم جداست.
            PrintMetrics("فقط دیده‌نشده", Measure(model, samples.Where((_, i) => heldOut.Contains(i)))); // این بخش در گرادیان‌ها شرکت نکرده است.
            Console.WriteLine("این آزمایش روی زوج‌های کنارگذاشته‌شده است؛ تضمینی برای اعداد خارج از دامنه نیست."); // دامنهٔ استنتاج علمی محدود نگه داشته می‌شود.
        }
        else // بدون تقسیم، معیار کل جدول فقط بازسازی دادهٔ آموزشی را می‌سنجد.
            Console.WriteLine("همهٔ زوج‌ها در آموزش استفاده شده‌اند؛ این نتیجه سنجش تعمیم به دادهٔ دیده‌نشده نیست."); // دقت آموزشی با تعمیم اشتباه گرفته نمی‌شود.
    }

    private static void PrintMetrics(string label, Metrics metrics) // قالب واحد مقایسهٔ بخش‌های مختلف را ساده می‌کند.
    {
        Console.WriteLine($"{label}: MAE={metrics.MeanAbsoluteError:F6}, MaxAE={metrics.MaximumAbsoluteError:F6}, گردکردهٔ صحیح={metrics.RoundedCorrect}/{metrics.Count}"); // خطاها در واحد حاصل‌ضرب و شمارش با مخرج واقعی چاپ می‌شوند.
        Console.WriteLine(metrics.RoundedCorrect == metrics.Count ? "تمام پاسخ‌های این بخش پس از گردکردن درست‌اند؛ خروجی خام همچنان تقریبی است." : "مدل هنوز به دقت مطلوبِ تمام پاسخ‌های صحیح پس از گردکردن نرسیده است."); // کیفیت ناکافی صریح و بدون جایگزینی پاسخ اعلام می‌شود.
    }
}
