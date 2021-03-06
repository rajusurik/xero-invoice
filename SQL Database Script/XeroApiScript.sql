USE [XeroApiDatabase]
GO
/****** Object:  Table [dbo].[XeroApiLog]    Script Date: 08/10/2018 07:57:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[XeroApiLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[InvoiceType] [varchar](50) NULL,
	[LineAmountTypes] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[DueDate] [varchar](50) NULL,
 CONSTRAINT [PK_XeroApiLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[AddApiAuditLog]    Script Date: 08/10/2018 07:57:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		kona
-- Create date: 2018-08-10 02:02:35.880
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddApiAuditLog](
	@Name varchar(50),
	@InvoiceType varchar(50),
	@LineAmountTypes varchar(50),
	@CreateDate datetime,
	@DueDate datetime
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    insert into XeroApiLog values(@Name,@InvoiceType,@LineAmountTypes,@CreateDate,@DueDate)
END
GO
