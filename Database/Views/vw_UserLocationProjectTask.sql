CREATE OR ALTER VIEW [dbo].[vw_UserLocationProjectTask]
AS
SELECT
	u.[UserId],
	u.[Name] AS [UserName],
	u.[Email] AS [UserEmail],
	l.[LocationId],
	l.[Address] AS [LocationAddress],
	l.[PostalCode],
	p.[ProjectId],
	p.[ProjectTitle],
	p.[Type] AS [ProjectType],
	p.[StartDate] AS [ProjectStartDate],
	t.[TaskId],
	t.[DepartmentId],
	t.[Type] AS [TaskType],
	t.[Status] AS [TaskStatus],
	t.[StartDate] AS [TaskStartDate],
	t.[EndDate] AS [TaskEndDate]
FROM [dbo].[Tasks] AS t
INNER JOIN [dbo].[Users] AS u
	ON u.[UserId] = t.[CreatorUserId]
INNER JOIN [dbo].[Locations] AS l
	ON l.[LocationId] = t.[LocationId]
INNER JOIN [dbo].[Projects] AS p
	ON p.[ProjectId] = t.[ProjectId];
GO
