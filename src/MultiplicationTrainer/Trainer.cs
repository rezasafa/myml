namespace MultiplicationTrainer; // آموزش تنها از هستهٔ محاسبات و داده استفاده می‌کند.

public static class Trainer // ترتیب آموزش و گزارش‌گیری از پیاده‌سازی لایه جداست.
{
    public static void Train(ModelState model, int epochs, Action<int, double, double>? report = null) // گزارش اختیاری آزمون خودکار را بی‌صدا نگه می‌دارد.
    {
        model.Validate(); // مدل بازیابی‌شده نیز باید قابل آموزش باشد.
        if (epochs <= 0 || epochs > 1000000 || model.CompletedEpochs > int.MaxValue - epochs) // شمارنده نباید سرریز یا غیرمثبت شود.
            throw new ArgumentOutOfRangeException(nameof(epochs)); // درخواست نامعتبر پیش از تغییر وزن‌ها رد می‌شود.
        var samples = DataSet.Create(model.Settings); // برچسب‌ها فقط در مسیر آموزش و ارزیابی استفاده می‌شوند.
        var heldOut = model.HeldOutIndices.ToHashSet(); // عضویت در مجموعهٔ کنارگذاشته‌شده سریع بررسی می‌شود.
        int[] training = Enumerable.Range(0, samples.Length).Where(i => !heldOut.Contains(i)).ToArray(); // هیچ نمونهٔ ارزیابی به حلقهٔ آموزش وارد نمی‌شود.
        var network = model.Network(); // شبکه به همان وزن‌های فعلی مدل وصل می‌شود.
        for (int epoch = 0; epoch < epochs; epoch++) // تعداد دورهٔ درخواستی به دوره‌های قبلی افزوده می‌شود.
        {
            int[] order = (int[])training.Clone(); // هر دوره از ترتیب ثابت شروع می‌شود تا ادامهٔ اجرا قابل تکرار باشد.
            int shuffleSeed = unchecked(model.Settings.Seed + model.CompletedEpochs * 104729); // بذر دوره از شمارندهٔ ذخیره‌شده به دست می‌آید.
            DataSet.Shuffle(order, new Random(shuffleSeed)); // نمونه‌ها در هر دوره مجدداً درهم می‌ریزند.
            double rate = model.Settings.LearningRate / (1 + model.CompletedEpochs / model.Settings.DecayEpochs); // کاهش تدریجی گام، نوسان نزدیک جواب را کم می‌کند.
            foreach (int index in order) // فقط نمونه‌های بخش آموزش پیمایش می‌شوند.
                network.TrainSample(samples[index], rate); // پس‌انتشار و یک گام گرادیان کاهشی انجام می‌شود.
            model.CompletedEpochs++; // شمارنده تنها پس از پایان یک دوره افزایش می‌یابد.
            if (epoch == 0 || model.CompletedEpochs % model.Settings.ReportEvery == 0 || epoch == epochs - 1) // ابتدا، دوره‌های معین و انتها گزارش می‌شوند.
            {
                double loss = training.Average(i => Math.Pow(network.Predict(samples[i].Input) - samples[i].Target, 2) / 2); // خطای پس از دوره با وزن ثابت و فقط روی بخش آموزش محاسبه می‌شود.
                if (!double.IsFinite(loss)) // خطای نامتناهی نشانهٔ شکست عددی است.
                    throw new ArithmeticException("خطای آموزش متناهی نیست؛ مدل قابل استفاده نیست."); // موفقیت کاذب گزارش نمی‌شود.
                report?.Invoke(model.CompletedEpochs, loss, rate); // رابط کاربری دربارهٔ شیوهٔ نمایش تصمیم می‌گیرد.
            }
        }
    }
}
