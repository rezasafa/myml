-- این اسکریپت را در پایگاه دادهٔ مقصد اجرا کنید؛ پایگاه داده باید از قبل ساخته شده باشد.
USE [MultiplicationLab]
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
