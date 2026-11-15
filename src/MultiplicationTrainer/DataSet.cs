namespace MultiplicationTrainer; // داده و شبکه از قرارداد مشترک مقیاس استفاده می‌کنند.

public sealed record Sample(int A, int B, double[] Input, double Target); // جواب واقعی فقط برای آموزش و ارزیابی در Target قرار می‌گیرد.

public static class DataSet // تولید داده از منطق پیش‌بینی مستقل است.
{
    public static double NormalizeInput(double value, Settings settings) // بازهٔ ورودی به بازهٔ متقارن مناسب tanh منتقل می‌شود.
        => 2 * (value - settings.Minimum) / (settings.Maximum - settings.Minimum) - 1; // دو کران دقیقاً به منفی یک و مثبت یک می‌روند.

    public static Sample[] Create(Settings settings) // تمام زوج‌های مرتب، شامل هر دو ترتیب عوامل، ساخته می‌شوند.
    {
        settings.Validate(); // پیش از حلقه، کران‌ها بررسی می‌شوند.
        var samples = new List<Sample>(); // ترتیب اولیهٔ نمونه‌ها ثابت می‌ماند.
        for (int a = settings.Minimum; a <= settings.Maximum; a++) // عامل اول سراسر دامنه را طی می‌کند.
        {
            for (int b = settings.Minimum; b <= settings.Maximum; b++) // عامل دوم برای هر مقدار عامل اول پیمایش می‌شود.
                samples.Add(new Sample(a, b, [NormalizeInput(a, settings), NormalizeInput(b, settings)], a * (double)b / settings.OutputScale)); // ضرب فقط برچسب هدف را تولید می‌کند.
        }
        return samples.ToArray(); // آرایهٔ ثابت امکان نگهداری اندیس‌های ارزیابی را فراهم می‌کند.
    }

    public static void Shuffle(int[] indices, Random random) // الگوریتم فیشر–یتس ترتیب را بدون حذف یا تکرار نمونه عوض می‌کند.
    {
        for (int i = indices.Length - 1; i > 0; i--) // هر جایگاه نهایی تنها یک بار انتخاب می‌شود.
        {
            int j = random.Next(i + 1); // انتخاب یکنواخت از بخش باقی‌مانده انجام می‌شود.
            (indices[i], indices[j]) = (indices[j], indices[i]); // جابه‌جایی همهٔ اندیس‌ها را حفظ می‌کند.
        }
    }

    public static int[] HeldOutIndices(Settings settings) // تقسیم آزمایش فقط هنگام ساخت مدل انجام می‌شود.
    {
        int count = Create(settings).Length; // اندازهٔ جدول از کران‌های تنظیمات به دست می‌آید.
        int heldOutCount = settings.HoldoutFraction == 0 ? 0 : Math.Max(1, (int)(count * settings.HoldoutFraction)); // حالت آزمایش دست‌کم یک نمونهٔ مستقل دارد.
        int[] indices = Enumerable.Range(0, count).ToArray(); // ابتدا هر نمونه دقیقاً یک اندیس دارد.
        Shuffle(indices, new Random(unchecked(settings.Seed ^ 0x51ED270B))); // بذر تقسیم با بذر وزن‌ها متفاوت است.
        return indices.Take(heldOutCount).Order().ToArray(); // اندیس‌ها ذخیره می‌شوند تا ادامهٔ آموزش همان تقسیم را حفظ کند.
    }
}
