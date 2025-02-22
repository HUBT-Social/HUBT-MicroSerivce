CREATE TABLE [dbo].[ThoiKhoaBieu] (
    [ClassName] NVARCHAR (50)  NOT NULL,
    [Day]       NVARCHAR (10)  NOT NULL,
    [Session]   NVARCHAR (50)  NOT NULL,
    [Subject]   NVARCHAR (100) NOT NULL,
    [Room]      NVARCHAR (50)  NOT NULL,
    [ZoomID]    NVARCHAR (10)  NULL,
    CONSTRAINT [PK_thoikhoabieu] PRIMARY KEY CLUSTERED ([ClassName] ASC, [Day] ASC, [Session] ASC)
);

