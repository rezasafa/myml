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
