# متن کامل پروژه

ساختار و محتوای کامل فایل‌های اجرایی، تنظیمات، آزمون‌ها و راهنماها در ادامه آمده است. هیچ متد یا بخشی حذف نشده است. `AGENTS.md` سند نیازمندی ورودی است و تغییر نکرده؛ این مجموعه نیز محتوای خودش را دوباره درج نمی‌کند.

## ساختار

```text
myml/
├── AGENTS.md                         نیازمندی‌های اصلی؛ بدون تغییر
├── README.md                         نصب، اجرا و تنظیمات
├── MultiplicationTrainer.sln          ساخت هر دو پروژه از ریشه
├── .editorconfig                     قالب کد
├── .gitignore                        فایل‌های محلی و خروجی ساخت
├── src/MultiplicationTrainer/
│   ├── MultiplicationTrainer.csproj   برنامهٔ .NET 8 و وابستگی SQL
│   ├── Program.cs                    منوی فارسی و اجرای غیرتعاملی
│   ├── Settings.cs                   تنظیمات و اعتبارسنجی
│   ├── DataSet.cs                    زوج‌ها، نرمال‌سازی و درهم‌ریزی
│   ├── NeuralNetwork.cs              لایه‌ها، انتشار و پس‌انتشار
│   ├── ModelState.cs                 وضعیت کامل و قالب JSON نسخهٔ ۱
│   ├── Trainer.cs                    SGD و زمان‌بندی نرخ یادگیری
│   ├── Evaluation.cs                 جدول کامل و معیارهای کیفیت
│   └── SqlModelRepository.cs         ذخیره و بارگذاری واقعی SQL
├── tests/MultiplicationTrainer.Tests/
│   ├── MultiplicationTrainer.Tests.csproj
│   └── Program.cs                    آزمون‌های عددی و یکپارچه‌سازی
├── sql/001_create_models.sql         ساخت جدول در پایگاه دادهٔ موجود
└── docs/
    ├── WALKTHROUGH.md                مثال عددی انتشار و پس‌انتشار
    ├── VALIDATION.md                 نتیجهٔ آزمون‌های این محیط
    └── SOURCE.md                     متن کامل فایل‌ها، بدون حذف کد
```

## `MultiplicationTrainer.sln`

````text
﻿
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{402CF294-C5F5-4EB7-8930-8EE4FCC45C54}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiplicationTrainer", "src\MultiplicationTrainer\MultiplicationTrainer.csproj", "{083327DF-B735-47BE-989A-48C895BFE6C4}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "tests", "tests", "{FD69EBC1-A736-44B3-B87A-0172A849BB4B}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MultiplicationTrainer.Tests", "tests\MultiplicationTrainer.Tests\MultiplicationTrainer.Tests.csproj", "{51E305CE-9322-4EA2-B8DE-156FD91B657E}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{083327DF-B735-47BE-989A-48C895BFE6C4}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{083327DF-B735-47BE-989A-48C895BFE6C4}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{083327DF-B735-47BE-989A-48C895BFE6C4}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{083327DF-B735-47BE-989A-48C895BFE6C4}.Release|Any CPU.Build.0 = Release|Any CPU
		{51E305CE-9322-4EA2-B8DE-156FD91B657E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{51E305CE-9322-4EA2-B8DE-156FD91B657E}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{51E305CE-9322-4EA2-B8DE-156FD91B657E}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{51E305CE-9322-4EA2-B8DE-156FD91B657E}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{083327DF-B735-47BE-989A-48C895BFE6C4} = {402CF294-C5F5-4EB7-8930-8EE4FCC45C54}
		{51E305CE-9322-4EA2-B8DE-156FD91B657E} = {FD69EBC1-A736-44B3-B87A-0172A849BB4B}
	EndGlobalSection
EndGlobal
````

## `.editorconfig`

````text
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true

[*.cs]
indent_style = space
indent_size = 4
````

## `.gitignore`

````text
**/bin/
**/obj/
.env
*.user
*.suo
````

## `src/MultiplicationTrainer/MultiplicationTrainer.csproj`

````xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Data.SqlClient" Version="7.1.1" />
  </ItemGroup>
</Project>
````

## `src/MultiplicationTrainer/Settings.cs`

````csharp
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
````

## `src/MultiplicationTrainer/DataSet.cs`

````csharp
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
````

## `src/MultiplicationTrainer/NeuralNetwork.cs`

````csharp
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
````

## `src/MultiplicationTrainer/ModelState.cs`

````csharp
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
````

## `src/MultiplicationTrainer/Trainer.cs`

````csharp
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
````

## `src/MultiplicationTrainer/Evaluation.cs`

````csharp
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
````

## `src/MultiplicationTrainer/SqlModelRepository.cs`

````csharp
using System.Data; // نوع صریح پارامترهای SQL از استنتاج نادرست طول و نوع جلوگیری می‌کند.
using Microsoft.Data.SqlClient; // این کتابخانه فقط برای ارتباط با SQL Server استفاده می‌شود.

namespace MultiplicationTrainer; // ذخیره‌سازی از محاسبات شبکه جدا نگه داشته می‌شود.

public sealed class SqlModelRepository // هر ذخیره یک تصویر مستقل ایجاد می‌کند و مدل قبلی را بازنویسی نمی‌کند.
{
    private readonly string connectionString; // رشتهٔ اتصال فقط در حافظه نگهداری و هرگز چاپ نمی‌شود.

    public SqlModelRepository() // اتصال فقط هنگام انتخاب عملیات پایگاه داده لازم است.
    {
        connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? ""; // اطلاعات اتصال از محیط خوانده می‌شود.
        if (string.IsNullOrWhiteSpace(connectionString)) // نبود تنظیمات باید پیش از تلاش اتصال مشخص شود.
            throw new InvalidOperationException("متغیر SQLSERVER_CONNECTION_STRING تنظیم نشده است؛ عملیات SQL انجام نشد."); // ذخیرهٔ موفق وانمود نمی‌شود.
    }

    public Guid Save(ModelState model, string name) // نتیجه فقط پس از ثبت تراکنش بازگردانده می‌شود.
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200) // نام با محدودیت ستون پایگاه داده تطبیق داده می‌شود.
            throw new ArgumentException("نام مدل باید ۱ تا ۲۰۰ نویسه باشد."); // خطای قابل اصلاح قبل از اتصال گزارش می‌شود.
        string payload = ModelCodec.Encode(model); // اعتبارسنجی کامل پیش از شروع تراکنش انجام می‌شود.
        Guid id = Guid.NewGuid(); // شناسهٔ جدید از بازنویسی تصادفی مدل قبلی جلوگیری می‌کند.
        using var connection = new SqlConnection(connectionString); // اتصال پس از پایان عملیات آزاد می‌شود.
        connection.Open(); // خطای دسترسی مستقیماً به رابط کاربر منتقل می‌شود.
        using var transaction = connection.BeginTransaction(); // درج مدل باید اتمی باشد.
        using var command = connection.CreateCommand(); // دستور از همان اتصال تراکنش استفاده می‌کند.
        command.Transaction = transaction; // درج خارج از تراکنش انجام نمی‌شود.
        command.CommandText = "INSERT INTO dbo.NeuralModels (ModelId, ModelName, FormatVersion, Payload) VALUES (@id, @name, @version, @payload);"; // متن SQL از دادهٔ کاربر جداست.
        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id; // شناسه به شکل پارامتر نوع‌دار ارسال می‌شود.
        command.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = name; // نام شامل نقل‌قول نیز داده باقی می‌ماند.
        command.Parameters.Add("@version", SqlDbType.Int).Value = model.FormatVersion; // نسخه در ستون قابل بررسی هم ثبت می‌شود.
        command.Parameters.Add("@payload", SqlDbType.NVarChar, -1).Value = payload; // منفی یک معادل NVARCHAR(MAX) و مانع قطع‌شدن وزن‌هاست.
        command.ExecuteNonQuery(); // تمام وضعیت مدل در یک سطر درج می‌شود.
        transaction.Commit(); // فقط این نقطه ذخیره را قطعی می‌کند؛ Dispose در خطا تراکنش باز را برمی‌گرداند.
        return id; // شناسهٔ ذخیرهٔ تأییدشده برای بارگذاری بعدی ارائه می‌شود.
    }

    public ModelState Load(Guid id) // مدل با شناسهٔ دقیق انتخاب می‌شود.
    {
        using var connection = new SqlConnection(connectionString); // اتصال خواندن مستقل از عملیات ذخیره است.
        connection.Open(); // نبود سرور به‌صورت خطا گزارش خواهد شد.
        using var command = connection.CreateCommand(); // دستور پارامتری از تزریق SQL جلوگیری می‌کند.
        command.CommandText = "SELECT FormatVersion, Payload FROM dbo.NeuralModels WHERE ModelId = @id;"; // فقط یک تصویر کامل خوانده می‌شود.
        command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id; // ورودی کاربر در متن دستور قرار نمی‌گیرد.
        using var reader = command.ExecuteReader(CommandBehavior.SingleRow); // خواننده فقط یک مدل انتظار دارد.
        if (!reader.Read()) // نبود شناسه نباید به ساخت خودکار مدل تازه منجر شود.
            throw new InvalidOperationException("مدلی با این شناسه پیدا نشد."); // وضعیت مدل فعلی در رابط کاربر حفظ می‌شود.
        if (reader.GetInt32(0) != 1) // نسخهٔ ستون باید با نسخهٔ پشتیبانی‌شده یکسان باشد.
            throw new InvalidDataException("نسخهٔ ذخیره‌سازی پشتیبانی نمی‌شود."); // خواندن قالب ناشناخته متوقف می‌شود.
        return ModelCodec.Decode(reader.GetString(1)); // وزن‌ها و تنظیمات دقیقاً از وضعیت ذخیره‌شده بازسازی می‌شوند.
    }
}
````

## `src/MultiplicationTrainer/Program.cs`

````csharp
using System.Globalization; // اعداد تنظیمات خط فرمان با قرارداد ثابت خوانده می‌شوند.
using System.Text; // خروجی فارسی با UTF-8 تنظیم می‌شود.
using System.Text.Json; // خطای قالب مدل جداگانه قابل تشخیص است.
using Microsoft.Data.SqlClient; // خطای اتصال بدون افشای رشتهٔ اتصال مدیریت می‌شود.

namespace MultiplicationTrainer; // نقطهٔ ورود از اجزای آموزشی مشترک استفاده می‌کند.

public static class Program // رابط کنسولی فقط جریان کار را هدایت می‌کند.
{
    public static int Main(string[] args) // کد خروج برای اجرای خودکار نیز قابل استفاده است.
    {
        Console.OutputEncoding = Encoding.UTF8; // متن فارسی به‌درستی به ترمینال ارسال می‌شود.
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; // نمایش و خواندن اعشار بین ماشین‌ها یکسان می‌ماند.
        try // خطاهای قابل انتظار به پیام روشن تبدیل می‌شوند.
        {
            if (args.Length > 0) // حالت غیرتعاملی برای آزمایش تکرارپذیر فراهم است.
                return RunCommand(args); // نتیجهٔ اجرای فرمان به سیستم‌عامل برمی‌گردد.
            RunMenu(); // بدون آرگومان، منوی فارسی اجرا می‌شود.
            return 0; // خروج عادی موفق است.
        }
        catch (Exception exception) when (IsExpected(exception)) // خطاهای کاربر، داده و اتصال مدیریت می‌شوند.
        {
            PrintError(exception); // پیام اتصال شامل رمز یا رشتهٔ اتصال نیست.
            return 1; // اجرای ناموفق با کد غیرصفر اعلام می‌شود.
        }
    }

    private static int RunCommand(string[] args) // فرمان train بدون SQL هم قابل اجراست.
    {
        if (args[0] == "--help") // راهنمای کوتاه بدون آغاز آموزش نمایش داده می‌شود.
        {
            Console.WriteLine("dotnet run --project src/MultiplicationTrainer -- train [--holdout 0.2] [--epochs 12000] [--seed 42]"); // گزینه‌های قابل استفاده چاپ می‌شوند.
            return 0; // نمایش راهنما یک عملیات موفق است.
        }
        if (args[0] != "train" || args.Length % 2 == 0) // پس از نام فرمان، گزینه‌ها باید زوج نام و مقدار باشند.
            throw new ArgumentException("فرمان نامعتبر است؛ از --help استفاده کنید."); // ورودی مبهم اجرا نمی‌شود.
        var settings = new Settings(); // پیش‌فرض‌ها همان تنظیمات مدل تعاملی هستند.
        for (int i = 1; i < args.Length; i += 2) // هر گزینه همراه مقدارش بررسی می‌شود.
        {
            switch (args[i]) // فقط گزینه‌های شناخته‌شده پذیرفته می‌شوند.
            {
                case "--holdout": settings.HoldoutFraction = double.Parse(args[i + 1], CultureInfo.InvariantCulture); break; // نسبت تقسیم پیش از ساخت مدل اعمال می‌شود.
                case "--epochs": settings.EpochsPerRun = int.Parse(args[i + 1], CultureInfo.InvariantCulture); break; // تعداد دورهٔ این اجرا تعیین می‌شود.
                case "--seed": settings.Seed = int.Parse(args[i + 1], CultureInfo.InvariantCulture); break; // بذر هم وزن‌ها و هم ترتیب‌ها را کنترل می‌کند.
                default: throw new ArgumentException($"گزینهٔ ناشناخته: {args[i]}"); // غلط تایپی بی‌صدا نادیده گرفته نمی‌شود.
            }
        }
        var model = ModelState.Create(settings); // اجرای فرمان همیشه آموزش از صفر است.
        TrainAndPrint(model); // پس از آموزش، تمام زوج‌ها و معیارها چاپ می‌شوند.
        return 0; // این کد فقط اتمام محاسبات را نشان می‌دهد؛ کیفیت در گزارش مشخص است.
    }

    private static void RunMenu() // مدل فعال تا خروج از برنامه در حافظه باقی می‌ماند.
    {
        ModelState? model = null; // شروع برنامه هیچ مدل ذخیره‌شده‌ای را خودکار بارگذاری نمی‌کند.
        while (true) // پس از هر عملیات کاربر به منو بازمی‌گردد.
        {
            Console.WriteLine("\n۱/1 ساخت مدل جدید و آموزش از صفر\n۲/2 بارگذاری مدل ذخیره‌شده\n۳/3 ادامه آموزش مدل بارگذاری‌شده\n۴/4 آزمون دو عدد\n۵/5 نمایش کل جدول و معیارها\n۶/6 ذخیره مدل\n۰/0 خروج"); // همهٔ عملیات خواسته‌شده در دسترس‌اند.
            string? choice = Console.ReadLine(); // پایان ورودی نیز امکان خروج تمیز می‌دهد.
            if (choice is null or "0" or "۰") // ورودی بسته‌شده باعث حلقهٔ بی‌پایان نمی‌شود.
                return; // برنامه بدون ذخیرهٔ خودکار پایان می‌یابد.
            try // خطای یک عملیات مانع استفادهٔ بعدی از منو نمی‌شود.
            {
                switch (choice) // ارقام فارسی و لاتین برای گزینه‌های منو پذیرفته می‌شوند.
                {
                    case "1": case "۱": // انتخاب مدل جدید مستقل از پایگاه داده است.
                        Console.WriteLine("نسبت دادهٔ دیده‌نشده را وارد کنید: 0 برای کل جدول یا مثلاً 0.2"); // تفاوت حالت‌ها پیش از آموزش مشخص می‌شود.
                        var settings = new Settings { HoldoutFraction = double.Parse(ReadRequired(), CultureInfo.InvariantCulture) }; // تنظیمات مدل جدید از پیش‌فرض‌ها ساخته می‌شود.
                        var fresh = ModelState.Create(settings); // وزن‌ها و تقسیم داده از صفر ساخته می‌شوند.
                        TrainAndPrint(fresh); // مدل تازه پیش از جایگزینی کامل آموزش می‌بیند.
                        model = fresh; // فقط نتیجهٔ موفق محاسبات به مدل فعال تبدیل می‌شود.
                        break; // کنترل به منو برمی‌گردد.
                    case "2": case "۲": // بارگذاری فقط با درخواست صریح انجام می‌شود.
                        model = LoadRequested(); // در صورت خطا، انتساب انجام نمی‌شود و مدل قبلی حفظ می‌شود.
                        Console.WriteLine($"مدل با {model.CompletedEpochs} دوره بارگذاری شد."); // تعداد دورهٔ بازیابی‌شده نمایش داده می‌شود.
                        break; // کاربر می‌تواند مدل را بدون آموزش آزمایش کند.
                    case "3": case "۳": // ادامهٔ آموزش از یک تصویر ذخیره‌شده آغاز می‌شود.
                        var resumed = LoadRequested(); // طبق قرارداد ابتدا مدل از SQL بارگذاری می‌شود.
                        Console.WriteLine("تعداد دورهٔ اضافی را وارد کنید:"); // تعداد تازه به شمارندهٔ قبلی اضافه می‌شود.
                        int epochs = int.Parse(ReadRequired(), CultureInfo.InvariantCulture); // تعداد دوره از ورودی کاربر خوانده می‌شود.
                        Trainer.Train(resumed, epochs, PrintProgress); // همان وزن‌های بازیابی‌شده تغییر می‌کنند.
                        Evaluation.Print(resumed); // کیفیت پس از ادامه روی تمام زوج‌ها بررسی می‌شود.
                        model = resumed; // مدل موفق جدید در حافظه جایگزین می‌شود.
                        Console.WriteLine("برای ثبت ادامهٔ آموزش، گزینهٔ ذخیره را انتخاب کنید."); // ذخیرهٔ ضمنی یا ادعای ثبت خودکار وجود ندارد.
                        break; // تصویر اصلی پایگاه داده همچنان مستقل باقی می‌ماند.
                    case "4": case "۴": // پیش‌بینی فقط روی مدل فعال انجام می‌شود.
                        var active = RequireModel(model); // نبود مدل پیش از خواندن ورودی اعلام می‌شود.
                        Console.WriteLine($"دو عدد صحیح بین {active.Settings.Minimum} و {active.Settings.Maximum} با فاصله وارد کنید:"); // دامنه از مدل بارگذاری‌شده خوانده می‌شود.
                        string[] parts = ReadRequired().Split(' ', StringSplitOptions.RemoveEmptyEntries); // دو عامل از هم جدا می‌شوند.
                        if (parts.Length != 2) // ورودی اضافی یا ناکافی پذیرفته نمی‌شود.
                            throw new ArgumentException("دقیقاً دو عدد وارد کنید."); // کاربر می‌تواند دوباره تلاش کند.
                        double prediction = active.Predict(int.Parse(parts[0]), int.Parse(parts[1])); // پاسخ از وزن‌های مدل محاسبه می‌شود.
                        Console.WriteLine($"پیش‌بینی خام: {prediction:F8}؛ گرد‌شده: {Math.Round(prediction, MidpointRounding.AwayFromZero)}"); // مقدار خام و گرد‌شده از هم متمایزند.
                        break; // خروجی هیچ‌گاه با حاصل‌ضرب واقعی جایگزین نمی‌شود.
                    case "5": case "۵": // ارزیابی به آموزش دوباره نیاز ندارد.
                        Evaluation.Print(RequireModel(model)); // تمام زوج‌ها و معیارهای هر بخش نمایش داده می‌شوند.
                        break; // مدل در ارزیابی تغییر نمی‌کند.
                    case "6": case "۶": // ذخیره فقط پس از انتخاب کاربر انجام می‌شود.
                        var toSave = RequireModel(model); // مدل تهی قابل ذخیره نیست.
                        Console.WriteLine("نام مدل را وارد کنید:"); // نام صرفاً برچسب قابل خواندن برای انسان است.
                        Guid id = new SqlModelRepository().Save(toSave, ReadRequired()); // درج پارامتری در تراکنش اجرا می‌شود.
                        Console.WriteLine($"ذخیره تأیید شد. شناسه برای بارگذاری: {id}"); // این پیام فقط پس از Commit موفق چاپ می‌شود.
                        break; // شناسه باید برای مراجعهٔ بعدی نگهداری شود.
                    default: Console.WriteLine("گزینهٔ معتبر انتخاب کنید."); break; // گزینهٔ ناشناخته هیچ عملیاتی انجام نمی‌دهد.
                }
            }
            catch (EndOfStreamException) // بسته‌شدن ورودی وسط عملیات خروج عادی ایجاد می‌کند.
            {
                return; // منو دوباره منتظر ورودی ناموجود نمی‌ماند.
            }
            catch (Exception exception) when (IsExpected(exception)) // خطاهای قابل انتظار در همان منو مدیریت می‌شوند.
            {
                PrintError(exception); // خطای واضح بدون ادعای موفقیت چاپ می‌شود.
            }
        }
    }

    private static ModelState LoadRequested() // مسیر بارگذاری بین گزینهٔ دوم و سوم مشترک است.
    {
        Console.WriteLine("شناسهٔ GUID مدل ذخیره‌شده را وارد کنید:"); // مدل مقصد صریحاً توسط کاربر انتخاب می‌شود.
        Guid id = Guid.Parse(ReadRequired()); // رشتهٔ نامعتبر قبل از اتصال رد می‌شود.
        return new SqlModelRepository().Load(id); // تنها دادهٔ معتبر پایگاه داده به مدل تبدیل می‌شود.
    }

    private static ModelState RequireModel(ModelState? model) => model ?? throw new InvalidOperationException("ابتدا مدل بسازید یا بارگذاری کنید."); // عملیات بدون مدل با راهنمای روشن متوقف می‌شود.
    private static string ReadRequired() => Console.ReadLine() ?? throw new EndOfStreamException(); // پایان ورودی از متن خالی متمایز می‌شود.
    private static void PrintProgress(int epoch, double loss, double rate) => Console.WriteLine($"Epoch {epoch}: mean(0.5 × error²)={loss:G8}; rate={rate:G6}"); // خطای نرمال‌شدهٔ آموزش با نرخ فعلی چاپ می‌شود.

    private static void TrainAndPrint(ModelState model) // هر آموزش کامل با گزارش کیفیت همراه است.
    {
        Trainer.Train(model, model.Settings.EpochsPerRun, PrintProgress); // پارامترهای قابل تغییر از تنظیمات مدل خوانده می‌شوند.
        Evaluation.Print(model); // موفقیت یادگیری از تمام زوج‌ها سنجیده می‌شود.
    }

    private static bool IsExpected(Exception exception) => exception is ArgumentException or InvalidOperationException or InvalidDataException or ArithmeticException or FormatException or JsonException or SqlException or IOException; // خطاهای قابل توضیح به کاربر محدود می‌شوند.

    private static void PrintError(Exception exception) // پیام‌های SQL نباید اطلاعات حساس اتصال را افشا کنند.
    {
        if (exception is SqlException sql) // خطای SQL با شمارهٔ تشخیصی گزارش می‌شود.
            Console.Error.WriteLine($"عملیات SQL تأیید نشد (شمارهٔ خطا {sql.Number}). دسترسی سرور، رشتهٔ اتصال و اجرای sql/001_create_models.sql را بررسی کنید؛ ذخیرهٔ موفق اعلام نمی‌شود."); // قطع شبکه نیز به‌عنوان موفقیت تلقی نمی‌شود.
        else // سایر خطاهای شناخته‌شده شامل راهنمای دامنه و تنظیمات‌اند.
            Console.Error.WriteLine($"خطا: {exception.Message}"); // علت واقعی شکست برای اصلاح ورودی نمایش داده می‌شود.
    }
}
````

## `tests/MultiplicationTrainer.Tests/MultiplicationTrainer.Tests.csproj`

````xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/MultiplicationTrainer/MultiplicationTrainer.csproj" />
  </ItemGroup>
</Project>
````

## `tests/MultiplicationTrainer.Tests/Program.cs`

````csharp
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
````

## `sql/001_create_models.sql`

````sql
-- این اسکریپت را در پایگاه دادهٔ مقصد اجرا کنید؛ پایگاه داده باید از قبل ساخته شده باشد.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.NeuralModels', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NeuralModels
    (
        ModelId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_NeuralModels PRIMARY KEY,
        ModelName NVARCHAR(200) NOT NULL,
        CreatedUtc DATETIME2(7) NOT NULL CONSTRAINT DF_NeuralModels_CreatedUtc DEFAULT SYSUTCDATETIME(),
        FormatVersion INT NOT NULL,
        Payload NVARCHAR(MAX) NOT NULL,
        CONSTRAINT CK_NeuralModels_FormatVersion CHECK (FormatVersion = 1),
        CONSTRAINT CK_NeuralModels_Json CHECK (ISJSON(Payload) = 1)
    );
END;

COMMIT TRANSACTION;
````

## `README.md`

````markdown
# یادگیری جدول ضرب با شبکهٔ عصبی از صفر

پروژهٔ کنسولی C# برای یادگیری سازوکار شبکهٔ عصبی است. تمام لایه‌ها، مقداردهی اولیه، مشتق‌ها، پس‌انتشار و SGD در کد پروژه پیاده شده‌اند. تنها بستهٔ مستقیم خارجی، `Microsoft.Data.SqlClient`، برای SQL Server است. شبکه دو ورودی، یک یا دو لایهٔ مخفی `tanh` و یک خروجی خطی دارد؛ پاسخ خام همیشه از شبکه به دست می‌آید.

## ساختار فایل‌ها

```text
myml/
├── AGENTS.md                         نیازمندی‌های اصلی؛ بدون تغییر
├── README.md                         نصب، اجرا و تنظیمات
├── MultiplicationTrainer.sln          ساخت هر دو پروژه از ریشه
├── .editorconfig                     قالب کد
├── .gitignore                        فایل‌های محلی و خروجی ساخت
├── src/MultiplicationTrainer/
│   ├── MultiplicationTrainer.csproj   برنامهٔ .NET 8 و وابستگی SQL
│   ├── Program.cs                    منوی فارسی و اجرای غیرتعاملی
│   ├── Settings.cs                   تنظیمات و اعتبارسنجی
│   ├── DataSet.cs                    زوج‌ها، نرمال‌سازی و درهم‌ریزی
│   ├── NeuralNetwork.cs              لایه‌ها، انتشار و پس‌انتشار
│   ├── ModelState.cs                 وضعیت کامل و قالب JSON نسخهٔ ۱
│   ├── Trainer.cs                    SGD و زمان‌بندی نرخ یادگیری
│   ├── Evaluation.cs                 جدول کامل و معیارهای کیفیت
│   └── SqlModelRepository.cs         ذخیره و بارگذاری واقعی SQL
├── tests/MultiplicationTrainer.Tests/
│   ├── MultiplicationTrainer.Tests.csproj
│   └── Program.cs                    آزمون‌های عددی و یکپارچه‌سازی
├── sql/001_create_models.sql         ساخت جدول در پایگاه دادهٔ موجود
└── docs/
    ├── WALKTHROUGH.md                مثال عددی انتشار و پس‌انتشار
    ├── VALIDATION.md                 نتیجهٔ آزمون‌های این محیط
    └── SOURCE.md                     متن کامل فایل‌ها، بدون حذف کد
```

## ساخت و اجرای محلی

پیش‌نیاز: SDK دات‌نت ۸ و دسترسی به NuGet برای دریافت بسته‌ها. SQL Server فقط برای ذخیره و بارگذاری لازم است؛ آموزش و ارزیابی بدون آن کار می‌کنند.

```bash
dotnet restore tests/MultiplicationTrainer.Tests/MultiplicationTrainer.Tests.csproj
dotnet build MultiplicationTrainer.sln -c Release --no-restore -m:1
dotnet run --project src/MultiplicationTrainer -c Release --no-build
```

منو گزینه‌های ساخت مدل از صفر، بارگذاری، ادامهٔ آموزش مدل ذخیره‌شده، پیش‌بینی دو عدد، گزارش کامل، ذخیره و خروج را دارد. برای مقادیر عددی از ارقام لاتین و نقطهٔ اعشار استفاده کنید؛ شمارهٔ گزینه‌های منو ارقام فارسی را نیز می‌پذیرد. بستن برنامه مدل حافظه را خودکار ذخیره نمی‌کند.

اجرای تکرارپذیر بدون منو:

```bash
# همهٔ ۱۰۰ زوج در آموزش شرکت می‌کنند.
dotnet run --project src/MultiplicationTrainer -c Release -- train

# ۸۰ زوج برای آموزش، ۲۰ زوج برای ارزیابی مستقل؛ تمام ۱۰۰ زوج نمایش داده می‌شوند.
dotnet run --project src/MultiplicationTrainer -c Release -- train --holdout 0.2

# تغییر تعداد دوره و بذر برای آزمایش
dotnet run --project src/MultiplicationTrainer -c Release -- train --epochs 60000 --seed 42
```

عدد واقعی ضرب فقط برای تولید برچسب و محاسبهٔ معیار استفاده می‌شود. هیچ جدول پاسخ یا میان‌بری در مسیر `Predict` وجود ندارد. گزارش شامل MAE، بیشترین خطای مطلق و تعداد جواب‌های صحیح پس از گردکردن است. هدف «تمام پاسخ‌های گرد‌شده صحیح» صریح بررسی می‌شود؛ کامل‌نشدن آن شکست کیفیت اعلام می‌شود، هرچند اجرای محاسبات می‌تواند سالم تمام شود.

## نرمال‌سازی و آموزش

برای بازهٔ پیش‌فرض ۱ تا ۱۰:

- ورودی: `x = 2 × (a − 1) / 9 − 1`؛ دو کران به `−1` و `+1` تبدیل می‌شوند.
- هدف: `t = (a × b) / 100`؛ خروجی هدف بین `0.01` و `1` است.
- پیش‌بینی واقعی: `prediction = network(x1, x2) × 100`؛ هیچ بریدن خروجی یا جایگزینی جواب انجام نمی‌شود.
- هزینهٔ هر نمونه: `L = 0.5 × (predictionNormalized − targetNormalized)²`.
- SGD: بعد از هر نمونه تمام گرادیان‌ها محاسبه و سپس وزن‌ها و بایاس‌ها به‌روز می‌شوند.
- نرخ دورهٔ `e` از صفر: `LearningRate / (1 + e / DecayEpochs)`.

وزن‌ها با Xavier یکنواخت و بایاس‌ها با تصادف کوچک حول صفر مقداردهی می‌شوند. ترتیب نمونه‌ها در هر دوره با فیشر–یتس تغییر می‌کند. بذر هر دوره از بذر اصلی و تعداد دورهٔ تکمیل‌شده ساخته می‌شود، بنابراین ذخیرهٔ وضعیت داخلی مولد تصادف لازم نیست. تکرارپذیری عددی در همان نسخهٔ زمان اجرا و محیط انتظار می‌رود؛ بین معماری‌های پردازنده تضمین برابری بیت‌به‌بیت نداریم.

ارزیابی روی همهٔ داده‌های آموزشی، بازسازی همان جدول را می‌سنجد. گزینهٔ `--holdout 0.2` زوج‌های کنارگذاشته‌شده را از همهٔ گام‌های آموزش حذف می‌کند و معیار آموزش و دیده‌نشده را جدا نشان می‌دهد. تقسیم تصادفی بر اساس **زوج مرتب** است؛ ممکن است `(a,b)` در آموزش و `(b,a)` در ارزیابی باشد. این سنجش یادگیری عوامل کاملاً جدید یا اعداد خارج از بازه نیست. دادهٔ کنارگذاشته‌شده در انتخاب نرخ، توقف یا به‌روزرسانی وزن شرکت نمی‌کند؛ تنظیم مکرر مدل با نگاه به نتیجهٔ آن نیز استقلال آزمایش را کاهش می‌دهد.

## SQL Server

۱. یک پایگاه دادهٔ مخصوص این پروژه بسازید و اسکریپت `sql/001_create_models.sql` را در همان پایگاه داده اجرا کنید، مثلاً با SSMS یا `sqlcmd`. جدول از `ISJSON` استفاده می‌کند؛ SQL Server 2016 یا جدیدتر لازم است.

۲. متغیر محیطی زیر را در همان ترمینال تنظیم کنید. مقدارها نمونه‌اند؛ گذرواژهٔ واقعی را در فایل‌های پروژه قرار ندهید:

```bash
export SQLSERVER_CONNECTION_STRING='Server=localhost,1433;Database=MultiplicationLab;User ID=YOUR_USER;Password=YOUR_SECRET;Encrypt=True;TrustServerCertificate=False;Connect Timeout=10'
```

برای سرور توسعهٔ محلی با گواهی خودامضا می‌توان `TrustServerCertificate=True` گذاشت؛ این انتخاب اعتبارسنجی گواهی سرور را غیرفعال می‌کند. حساب اجرا به `SELECT` و `INSERT` روی `dbo.NeuralModels` نیاز دارد؛ ایجاد جدول را با حساب دارای مجوز ساخت انجام دهید.

۳. مدل را آموزش دهید و گزینهٔ ۶ را انتخاب کنید. شناسهٔ GUID چاپ‌شده را نگه دارید. در اجرای بعدی گزینهٔ ۲ و همان شناسه، مدل را بدون آموزش بازسازی می‌کنند. گزینهٔ ۳ ابتدا تصویر انتخاب‌شده را از SQL بارگذاری می‌کند و سپس دوره‌های اضافی را روی وزن‌های موجود اجرا می‌کند. برای ثبت ادامهٔ آموزش دوباره گزینهٔ ۶ را بزنید.

هر ذخیره یک سطر جدید و مستقل است. ستون `Payload` شامل معماری، نام فعال‌سازی‌ها، فرمول و کران‌های نرمال‌سازی، تنظیمات آموزش، بذر، تعداد دورهٔ تکمیل‌شده، اندیس‌های کنارگذاشته‌شده و تمام وزن‌ها و بایاس‌هاست. اعداد `double` با JSON استاندارد ذخیره می‌شوند. نسخهٔ قالب در ستون جدا و payload ثبت می‌شود. درج با پارامترهای نوع‌دار و تراکنش انجام می‌شود؛ موفقیت فقط پس از `Commit` اعلام می‌شود. در قطع ارتباط هنگام Commit ممکن است نتیجهٔ ثبت برای کلاینت نامعلوم باشد؛ برنامه تأیید موفقیت نمی‌دهد. تصاویر قبلی بازنویسی نمی‌شوند.

## آزمون‌ها

اجراکنندهٔ آزمون یک Console App مستقل است و به چارچوب آزمایش اضافی نیاز ندارد:

```bash
dotnet run --project tests/MultiplicationTrainer.Tests -c Release

# آزمون واقعی پایگاه داده، پس از تنظیم اتصال و اجرای اسکریپت جدول
dotnet run --project tests/MultiplicationTrainer.Tests -c Release -- --sql
```

آزمون‌ها تمام گرادیان‌ها را برای یک و دو لایهٔ مخفی با تفاضل مرکزی بررسی می‌کنند، فرمول به‌روزرسانی و کاهش هزینه را می‌سنجند، استقلال دادهٔ ارزیابی و رد مدل خراب را کنترل می‌کنند و پیش‌بینی هر ۱۰۰ زوج را پیش و پس از بازیابی با تلورانس `1e-12` مقایسه می‌کنند. ادامهٔ آموزش پیوسته و بازیابی‌شده نیز مقایسه می‌شود.

حالت عادی فقط رفت‌وبرگشت قالب JSON را می‌آزماید و آزمون SQL را **SKIP** اعلام می‌کند. `--sql` واقعاً در SQL درج و سپس خواندن می‌کند؛ نبود اتصال به شکست و کد خروج ۱ منجر می‌شود. رکورد آزمون با نام `integration-test round-trip` باقی می‌ماند و شناسه‌اش چاپ می‌شود. آستانهٔ پوشش کد تعیین نشده است؛ تمرکز آزمون‌ها بر درستی عددی و قرارداد ذخیره‌سازی است.

## کجا تغییر بدهم؟

| تغییر | محل |
| --- | --- |
| تعداد نورون‌ها و لایه‌ها | `Settings.cs`، مثلاً `HiddenSizes = [24]` یا `[16,16]`؛ سپس مدل تازه بسازید |
| نرخ یادگیری و کاهش آن | `Settings.cs`: `LearningRate` و `DecayEpochs` |
| تعداد دوره و فاصلهٔ گزارش | `Settings.cs`: `EpochsPerRun` و `ReportEvery`؛ یا گزینهٔ `--epochs` |
| بذر و سهم ارزیابی | `Settings.cs`: `Seed` و `HoldoutFraction`؛ یا `--seed` و `--holdout` |
| محدودهٔ جدول | `Settings.cs`: `Minimum` و `Maximum`؛ داده، مقیاس و منو خودکار تطبیق می‌یابند؛ مدل تازه لازم است |
| تابع فعال‌سازی و مشتق | `NeuralNetwork.cs`: `Forward` و `TanhDerivative`؛ قرارداد `ModelState` و نسخهٔ قالب را هم تغییر دهید |
| روش آموزش | `NeuralNetwork.TrainSample` و `Trainer.Train`؛ وضعیت بهینه‌ساز جدید را در صورت نیاز ذخیره کنید |
| اتصال پایگاه داده | فقط متغیر محیطی `SQLSERVER_CONNECTION_STRING` |
| قالب یا روش ذخیره | `ModelState.cs`، `SqlModelRepository.cs` و اسکریپت SQL؛ برای تغییر ناسازگار نسخه و مهاجرت تعریف کنید |

تنظیمات ذخیره‌شده همراه مدل بارگذاری می‌شوند؛ عوض‌کردن پیش‌فرض‌های کد، معماری یا نرخ یک مدل قبلی را خودکار تغییر نمی‌دهد. مثال عددی کامل در [راهنمای محاسبات](docs/WALKTHROUGH.md)، متن کامل فایل‌ها در [مجموعهٔ کد](docs/SOURCE.md) و نتایج اجرا در [گزارش اعتبارسنجی](docs/VALIDATION.md) آمده‌اند.
````

## `docs/WALKTHROUGH.md`

````markdown
# مثال عددی انتشار و پس‌انتشار

برای ساده‌کردن محاسبه، یک شبکه با دو ورودی، یک نورون مخفی `tanh` و یک خروجی خطی در نظر بگیرید. این مثال مستقل از دادهٔ صحیح جدول ضرب است؛ هدفش نمایش یک گام محاسباتی است. همان فرمول‌ها در شبکهٔ چندنورونی پروژه به صورت حلقه اجرا می‌شوند.

## داده و پارامترهای اولیه

```text
x1 = 0.2       x2 = 0.4       target = 0.08
w1 = 0.5       w2 = -0.25     hiddenBias = 0.1
v  = 0.8                      outputBias = 0.05
learningRate = 0.1
```

ورودی‌ها از قبل نرمال شده‌اند و خروجی نیز در مقیاس نرمال‌شده است.

## انتشار رو به جلو

```text
z = w1*x1 + w2*x2 + hiddenBias
  = 0.5*0.2 - 0.25*0.4 + 0.1 = 0.1
h = tanh(z) = 0.099667994625
prediction = v*h + outputBias = 0.129734395700
error = prediction - target = 0.049734395700
L = 0.5*error² = 0.001236755058
```

خروجی خطی است، بنابراین مشتق آن نسبت به جمع وزن‌دار برابر ۱ است. در دامنهٔ اصلی پروژه، این خروجی نرمال‌شده در ۱۰۰ ضرب می‌شد تا به واحد حاصل‌ضرب برگردد.

## پس‌انتشار

مشتق هزینه نسبت به خروجی `error` است؛ ضریب ۲ مربع با ضریب نصف هزینه خنثی می‌شود:

```text
deltaOutput = error = 0.049734395700
dL/dv = deltaOutput*h = 0.004956927483
dL/dOutputBias = deltaOutput

tanh'(z) = 1 - h² = 0.990066290847
deltaHidden = deltaOutput*v*(1-h²) = 0.039392278943
dL/dw1 = deltaHidden*x1 = 0.007878455789
dL/dw2 = deltaHidden*x2 = 0.015756911577
dL/dHiddenBias = deltaHidden
```

نکتهٔ ترتیب محاسبات: `deltaHidden` با **وزن قدیمی** `v = 0.8` محاسبه می‌شود. اگر اول `v` را تغییر بدهیم و سپس آن را در پس‌انتشار استفاده کنیم، گرادیان یک وضعیت واحد از شبکه را محاسبه نکرده‌ایم. در پروژه، `ComputeGradients` همهٔ مشتق‌ها را پیش از هر تغییر کامل می‌کند.

## گام گرادیان کاهشی

برای هر پارامتر `parameterNew = parameterOld − learningRate * gradient`:

```text
vNew          = 0.799504307252
outputBiasNew = 0.045026560430
w1New         = 0.499212154421
w2New         = -0.251575691158
hiddenBiasNew = 0.096060772106
```

در نمونهٔ بعدی، انتشار رو به جلو با همین پارامترهای جدید آغاز می‌شود. یک epoch یعنی عبور از تمام نمونه‌های **بخش آموزش**، پس از درهم‌ریزی ترتیب آن‌ها. دادهٔ کنارگذاشته‌شده هیچ گامی ایجاد نمی‌کند.

## ابعاد در شبکهٔ واقعی

با معماری پیش‌فرض `2 → 16 → 16 → 1`:

| لایه | شکل وزن `[خروجی][ورودی]` | شکل بایاس | فعال‌سازی |
| --- | --- | --- | --- |
| مخفی اول | `16 × 2` | `16` | tanh |
| مخفی دوم | `16 × 16` | `16` | tanh |
| خروجی | `1 × 16` | `1` | خطی |

برای هر نورون `j`، جمع رو به جلو `z[j] = b[j] + Σ W[j][k] a[k]` است. در مسیر برگشت، `deltaPrevious[k] = (Σ W[j][k] delta[j]) × (1 − a[k]²)` و `gradientW[j][k] = delta[j] × aPrevious[k]` است. این محاسبه همان ضرب ترانهادهٔ ماتریس وزن است، بدون نیاز به کتابخانهٔ ماتریسی.

آزمون تفاضل مرکزی هر پارامتر را یک بار `+1e-6` و یک بار `−1e-6` تغییر می‌دهد و `(Lplus − Lminus)/(2e-6)` را با گرادیان تحلیلی مقایسه می‌کند. این بررسی برای تمام وزن‌ها و بایاس‌ها در هر دو معماری تک‌لایه و دولایه انجام می‌شود.
````

## `docs/VALIDATION.md`

````markdown
# گزارش اعتبارسنجی

این بررسی‌ها در محیط موجود با SDK دات‌نت `8.0.134` و زمان اجرای `8.0.34` روی لینوکس انجام شدند. فایل `AGENTS.md` تغییر نکرد.

## ساخت

ساخت Release برنامه و پروژهٔ آزمون موفق بود: **صفر خطا، صفر هشدار**. ساخت solution نیز با `dotnet build MultiplicationTrainer.sln -c Release --no-restore -m:1` تأیید شد. اجرای ساخت solution بدون `-m:1` در این محیط با وضعیت ناموفق ولی بدون پیام خطای تشخیصی پایان یافت؛ علت آن مشخص نشد، بنابراین دستور تأییدشدهٔ تک‌پردازشی در README آمده است. دریافت از `api.nuget.org` به دلیل محدودیت شبکهٔ محیط ممکن نبود. بستهٔ `Microsoft.Data.SqlClient 7.1.1` و وابستگی‌های آن از کش محلی موجود بازیابی شدند؛ فایل پروژه همین نسخه را مشخص می‌کند. برای ساخت معمول در محیط دیگر از دستورهای README و منبع NuGet استفاده کنید.

## آزمون‌های محاسباتی

همهٔ موارد زیر اجرا شدند و گذشتند:

- تولید ۱۰۰ زوج مرتب یکتا، مقیاس ورودی و هدف، و اندازهٔ فعال‌سازی‌ها.
- مقایسهٔ گرادیان تمام وزن‌ها و بایاس‌ها با تفاضل مرکزی برای یک و دو لایهٔ مخفی؛ تلورانس `1e-7`.
- فرمول تغییر تمام پارامترها و کاهش هزینه پس از یک گام کوچک.
- برابری پیش‌بینی هر ۱۰۰ زوج قبل و بعد از رفت‌وبرگشت JSON؛ تلورانس `1e-12` در مقیاس واقعی.
- برابری ادامهٔ آموزش پیوسته و ادامه پس از بازیابی، با حفظ تقسیم داده و تعداد دوره.
- حذف واقعی ۲۰ نمونهٔ ارزیابی از گام‌های آموزش.
- رد نسخهٔ ناشناخته، ابعاد ناسازگار، پارامتر نامتناهی و اندیس ارزیابی نامعتبر.
- خروج تمیز هنگام بسته‌شدن ورودی، مدیریت نبود مدل، فرمان نامعتبر، نسبت ارزیابی نامعتبر و تنظیم‌نبودن اتصال SQL.
- وجود کامنت فارسی برای تمام خطوط دارای اعلان یا دستور C#، با بررسی خطوط فایل‌های منبع.

## کیفیت اندازه‌گیری‌شده

همهٔ اجراها از صفر، با بذر `42`، معماری `2 → 16 → 16 → 1`، نرخ اولیهٔ `0.03` و `DecayEpochs = 5000` انجام شدند. تعداد دورهٔ پیش‌فرض همچنان `12000` است. خطاها بر اساس پیش‌بینی خام و در واحد حاصل‌ضرب هستند.

| آزمایش | بخش | MAE | بیشترین خطای مطلق | صحیح پس از گردکردن |
| --- | --- | --- | --- | --- |
| ۱۲۰۰۰ دوره، بدون جداسازی | همهٔ ۱۰۰ زوج آموزشی | 0.252514 | 1.112187 | 93/100 |
| ۱۲۰۰۰ دوره، جداسازی ۲۰٪ | تمام زوج‌ها | 0.273117 | 1.156023 | 90/100 |
| همان مدل با جداسازی | فقط آموزش | 0.298060 | 1.156023 | 71/80 |
| همان مدل با جداسازی | فقط دیده‌نشده | 0.173342 | 0.563984 | 19/20 |
| ۶۰۰۰۰ دوره، بدون جداسازی | همهٔ ۱۰۰ زوج آموزشی | 0.133339 | 0.766335 | 98/100 |

هیچ‌یک از این اجراها به هدف تمام پاسخ‌های گرد‌شدهٔ صحیح در کل جدول نرسید. آموزش طولانی‌تر کیفیت را بهتر کرد ولی تضمین دقت کامل نمی‌دهد. نتیجهٔ آموزش روی تمام زوج‌ها، آزمون تعمیم نیست. آزمایش ۸۰/۲۰ مستقل از اجرای تمام داده‌ها ساخته شد و نمونهٔ دیده‌نشده در گام‌های آموزش یا تنظیم خودکار پارامترها وارد نشد.

## وضعیت آزمون SQL

**آزمون واقعی رفت‌وبرگشت SQL در این محیط اجرا نشد**، چون `SQLSERVER_CONNECTION_STRING` تنظیم نشده بود و پایگاه دادهٔ در دسترسی معرفی نشده است. مسیر عادی آزمون به‌درستی `SKIP` چاپ کرد. همچنین بررسی شد که اجرای صریح `--sql` بدون اتصال با پیام روشن و کد خروج ۱ شکست می‌خورد؛ این بررسی جای آزمون پایگاه داده را نمی‌گیرد.

پیاده‌سازی SQL واقعی است و در ساخت کامپایل شد: INSERT پارامتری داخل تراکنش، Commit پیش از اعلام موفقیت و SELECT پارامتری همراه اعتبارسنجی وضعیت بازیابی‌شده. برای تأیید عملی این بخش، اسکریپت SQL را در پایگاه دادهٔ مقصد اجرا، اتصال را تنظیم و فرمان زیر را اجرا کنید:

```bash
dotnet run --project tests/MultiplicationTrainer.Tests -c Release -- --sql
```

این آزمون یک مدل آموزش‌دیده را ذخیره و مجدداً بارگذاری می‌کند، تمام پیش‌بینی‌ها و وضعیت مدل را مقایسه می‌کند و دو دوره ادامهٔ آموزش را نیز می‌سنجد. رکورد آزمایشی با شناسهٔ چاپ‌شده باقی می‌ماند.
````
