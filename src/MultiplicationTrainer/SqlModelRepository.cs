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
