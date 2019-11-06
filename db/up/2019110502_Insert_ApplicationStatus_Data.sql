SET IDENTITY_INSERT [dbo].[ApplicationStatus] ON 
INSERT [dbo].[ApplicationStatus] ([ApplicationStatusId], [Name]) VALUES (1, 'Pending')
INSERT [dbo].[ApplicationStatus] ([ApplicationStatusId], [Name]) VALUES (2, 'Rejected')
INSERT [dbo].[ApplicationStatus] ([ApplicationStatusId], [Name]) VALUES (3, 'Offer Extended')
INSERT [dbo].[ApplicationStatus] ([ApplicationStatusId], [Name]) VALUES (4, 'Offer Accepted')
SET IDENTITY_INSERT [dbo].[ApplicationStatus] OFF
