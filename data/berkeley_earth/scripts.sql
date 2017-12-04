CREATE TABLE [dbo].[Sites](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Country] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Sites] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX Idx_Sites_Name ON Sites(Name) INCLUDE (Country)

GO

CREATE TABLE [dbo].[AverageHigh](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SiteId] [int] NOT NULL,
	[DateAsFloat] [float] NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Temperature] [float] NULL,
 CONSTRAINT [PK_AverageHigh] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AverageHigh]  WITH CHECK ADD  CONSTRAINT [FK_AverageHigh_SiteId_Sites_Id] FOREIGN KEY([SiteId])
REFERENCES [dbo].[Sites] ([Id])
GO

ALTER TABLE [dbo].[AverageHigh] CHECK CONSTRAINT [FK_AverageHigh_SiteId_Sites_Id]
GO

CREATE NONCLUSTERED INDEX IDX_AverageHigh_SiteId_Year ON AverageHigh(SiteId, Year)

INSERT INTO Sites(Id, Name, Country)
SELECT [Column 0], LTRIM(RTRIM([Column 1])), LTRIM(RTRIM([Column 8]))
FROM site_detail



INSERT INTO AverageHigh(SiteId, DateAsFloat, Year, Month, Temperature)
SELECT 
	CAST([Column 0] AS int) AS SiteId,
	CAST([Column 2] AS float) AS DateAsFloat,
	FLOOR(CAST([Column 2] AS float)) AS Year,
	1 + FLOOR(12 * (CAST([Column 2] AS float) - FLOOR(CAST([Column 2] AS float)))) AS Month,
	CAST([Column 3] AS float) AS Temperature
FROM tmax_data