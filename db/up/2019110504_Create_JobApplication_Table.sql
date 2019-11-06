SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [JobApplication](
	[JobApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[Position] [varchar](100) NOT NULL,
	[Company] [varchar](50) NOT NULL,
	[RecruiterId] [int] NULL,
	[ApplicationStatusId] [int] NOT NULL,
	[InitialContactDate] [datetime] NOT NULL,
	[JobDescriptionUrl] [varchar](MAX) NULL
 CONSTRAINT [PK_JobApplication] PRIMARY KEY CLUSTERED 
(
	[JobApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [JobApplication]  WITH CHECK ADD  CONSTRAINT [FK_dbo.JobApplication_dbo.ApplicationStatus_ApplicationStatusId] FOREIGN KEY([ApplicationStatusId])
REFERENCES [ApplicationStatus] ([ApplicationStatusId])
GO

ALTER TABLE [JobApplication]  WITH CHECK ADD  CONSTRAINT [FK_dbo.JobApplication_dbo.Recruiter_RecruiterId] FOREIGN KEY([RecruiterId])
REFERENCES [Recruiter] ([RecruiterId])
GO

SET ANSI_PADDING OFF
GO
