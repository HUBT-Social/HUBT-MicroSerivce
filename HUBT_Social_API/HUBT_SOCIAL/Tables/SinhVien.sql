CREATE TABLE [dbo].[SinhVien] (
    [MASV]     CHAR (10)     NOT NULL,
    [HoTen]    NVARCHAR (50) NOT NULL,
    [NgaySinh] DATE          NOT NULL,
    [GioiTinh] NVARCHAR (50) NOT NULL,
    [TenLop]   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_SinhVien] PRIMARY KEY CLUSTERED ([MASV] ASC)
);