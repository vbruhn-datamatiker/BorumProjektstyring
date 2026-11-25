SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @UserId int = (SELECT TOP (1) UserId FROM dbo.Users ORDER BY UserId);

IF @UserId IS NULL
	THROW 51000, 'Der skal findes mindst én bruger, før mock-data kan indsættes.', 1;

DECLARE @Customers TABLE
(
	CustomerId int NOT NULL,
	CustomerName nvarchar(150) NOT NULL PRIMARY KEY
);

DECLARE @CustomerSeed TABLE
(
	CustomerName nvarchar(150) NOT NULL PRIMARY KEY,
	ContactPerson nvarchar(150) NOT NULL,
	Email nvarchar(256) NOT NULL,
	Phone nvarchar(50) NOT NULL,
	Website nvarchar(255) NOT NULL
);

INSERT INTO @CustomerSeed (CustomerName, ContactPerson, Email, Phone, Website)
VALUES
	(N'Ballerup Boligforening', N'Mette Sørensen', N'mette@ballerupbolig.dk', N'4455 1200', N'https://ballerupbolig.dk'),
	(N'Nordkystens Ejendomme', N'Lars Nielsen', N'lars@nordkyst-ejendomme.dk', N'4877 2300', N'https://nordkyst-ejendomme.dk'),
	(N'Øresund Byg', N'Anne Jensen', N'anne@oresundbyg.dk', N'3255 3400', N'https://oresundbyg.dk'),
	(N'Sjællands Erhvervsejendomme', N'Peter Hansen', N'peter@sjaellandsejendomme.dk', N'7020 4500', N'https://sjaellandsejendomme.dk'),
	(N'Københavns Grundejerforening', N'Camilla Larsen', N'camilla@kbhgrundejer.dk', N'3399 5600', N'https://kbhgrundejer.dk');

INSERT INTO dbo.Customers (Name, ContactPerson, Email, Phone, Website, OwnerId)
SELECT s.CustomerName, s.ContactPerson, s.Email, s.Phone, s.Website, @UserId
FROM @CustomerSeed AS s
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Customers AS c
	WHERE c.Name = s.CustomerName
);

INSERT INTO @Customers (CustomerId, CustomerName)
SELECT c.CustomerId, c.Name
FROM dbo.Customers AS c
INNER JOIN @CustomerSeed AS s ON s.CustomerName = c.Name;

DECLARE @Projects TABLE
(
	ProjectId int NOT NULL PRIMARY KEY,
	CustomerId int NOT NULL,
	ProjectTitle nvarchar(150) NOT NULL,
	StartDate datetime NOT NULL
);

DECLARE @ProjectSeed TABLE
(
	CustomerName nvarchar(150) NOT NULL,
	ProjectTitle nvarchar(150) NOT NULL PRIMARY KEY,
	ProjectType nvarchar(100) NOT NULL,
	StartDate datetime NOT NULL,
	History nvarchar(max) NOT NULL
);

INSERT INTO @ProjectSeed (CustomerName, ProjectTitle, ProjectType, StartDate, History)
VALUES
	(N'Ballerup Boligforening', N'Forsikringssager Ballerup', N'Forsikringssag', '20260105', N'Opfølgning på forsikringssager i Ballerup.'),
	(N'Ballerup Boligforening', N'Facaderenovering Ballerup', N'Renovering', '20260202', N'Planlægning af facaderenovering på flere ejendomme.'),
	(N'Nordkystens Ejendomme', N'Tagskader Helsingør', N'Forsikringssag', '20260112', N'Gennemgang og udbedring af tagskader i Helsingør.'),
	(N'Nordkystens Ejendomme', N'Vinduesudskiftning Hillerød', N'Udskiftning', '20260209', N'Udskiftning af vinduer i boligafdelinger i Hillerød.'),
	(N'Øresund Byg', N'Vandskader Gentofte', N'Forsikringssag', '20260119', N'Koordinering af udbedring efter vandskader i Gentofte.'),
	(N'Øresund Byg', N'Køkkenrenovering Lyngby', N'Renovering', '20260216', N'Renovering af køkkener i Lyngby.'),
	(N'Sjællands Erhvervsejendomme', N'Kontorombygning Roskilde', N'Ombygning', '20260126', N'Ombygning af kontorlokaler i Roskilde.'),
	(N'Sjællands Erhvervsejendomme', N'Brandsikring Køge', N'Brandsikring', '20260223', N'Opgradering af brandsikring i ejendomme i Køge.'),
	(N'Københavns Grundejerforening', N'Altanrenovering Valby', N'Renovering', '20260302', N'Renovering og sikkerhedsgennemgang af altaner i Valby.'),
	(N'Københavns Grundejerforening', N'Kælderrenovering Amager', N'Renovering', '20260309', N'Fugt- og kælderrenovering på Amager.');

INSERT INTO dbo.Projects (ProjectTitle, Type, StartDate, History, CustomerId, UserId)
SELECT p.ProjectTitle, p.ProjectType, p.StartDate, p.History, c.CustomerId, @UserId
FROM @ProjectSeed AS p
INNER JOIN @Customers AS c ON c.CustomerName = p.CustomerName
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Projects AS existingProject
	WHERE existingProject.ProjectTitle = p.ProjectTitle
);

INSERT INTO @Projects (ProjectId, CustomerId, ProjectTitle, StartDate)
SELECT p.ProjectId, p.CustomerId, p.ProjectTitle, p.StartDate
FROM dbo.Projects AS p
INNER JOIN @ProjectSeed AS seed ON seed.ProjectTitle = p.ProjectTitle;

DECLARE @PostalCodes TABLE
(
	PostalCode nvarchar(20) NOT NULL PRIMARY KEY,
	City nvarchar(100) NOT NULL
);

INSERT INTO @PostalCodes (PostalCode, City)
VALUES
	(N'2750', N'Ballerup'),
	(N'3000', N'Helsingør'),
	(N'3400', N'Hillerød'),
	(N'2820', N'Gentofte'),
	(N'2800', N'Kongens Lyngby'),
	(N'4000', N'Roskilde'),
	(N'4600', N'Køge'),
	(N'2500', N'Valby'),
	(N'2300', N'København S');

INSERT INTO dbo.PostalCodes (PostalCode, City)
SELECT p.PostalCode, p.City
FROM @PostalCodes AS p
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.PostalCodes AS existingPostalCode
	WHERE existingPostalCode.PostalCode = p.PostalCode
);

DECLARE @Addresses TABLE
(
	AddressNumber int NOT NULL PRIMARY KEY,
	Address nvarchar(255) NOT NULL,
	PostalCode nvarchar(20) NOT NULL
);

INSERT INTO @Addresses (AddressNumber, Address, PostalCode)
VALUES
	(1, N'Heimdalsvej 50', N'2750'),
	(2, N'Baltorpvej 12', N'2750'),
	(3, N'Parkvej 8', N'2750'),
	(4, N'Centrumgade 21', N'2750'),
	(5, N'Søndergade 17', N'3000'),
	(6, N'Kongevejen 44', N'3000'),
	(7, N'Bakkegade 6', N'3400'),
	(8, N'Frederiksværksgade 31', N'3400'),
	(9, N'Adelgade 9', N'2820'),
	(10, N'Jægersborg Allé 18', N'2820'),
	(11, N'Lyngby Hovedgade 72', N'2800'),
	(12, N'Virumvej 15', N'2800'),
	(13, N'Algade 33', N'4000'),
	(14, N'Københavnsvej 27', N'4000'),
	(15, N'Ølbyvej 19', N'4600'),
	(16, N'Torvet 5', N'4600'),
	(17, N'Gl. Køge Landevej 101', N'2500'),
	(18, N'Vigerslevvej 63', N'2500'),
	(19, N'Amagerbrogade 142', N'2300'),
	(20, N'Englandsvej 22', N'2300');

DECLARE @ProjectAddresses TABLE
(
	ProjectId int NOT NULL,
	AddressNumber int NOT NULL,
	LocationId int NOT NULL,
	Address nvarchar(255) NOT NULL,
	PostalCode nvarchar(20) NOT NULL,
	PRIMARY KEY (ProjectId, AddressNumber)
);

INSERT INTO dbo.Locations (Address, PostalCode)
SELECT a.Address, a.PostalCode
FROM @Addresses AS a
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Locations AS existingLocation
	WHERE existingLocation.Address = a.Address
	  AND existingLocation.PostalCode = a.PostalCode
);

INSERT INTO @ProjectAddresses (ProjectId, AddressNumber, LocationId, Address, PostalCode)
SELECT p.ProjectId, a.AddressNumber, l.LocationId, a.Address, a.PostalCode
FROM @Projects AS p
CROSS JOIN @Addresses AS a
INNER JOIN dbo.Locations AS l
	ON l.Address = a.Address
   AND l.PostalCode = a.PostalCode;

INSERT INTO dbo.Tasks (DepartmentId, LocationId, Type, Status, StartDate, EndDate, ProjectId, CreatorUserId)
SELECT
	1,
	pa.LocationId,
	N'Besigtigelse',
	CASE pa.AddressNumber % 4
		WHEN 0 THEN N'Afsluttet'
		WHEN 1 THEN N'Planlagt'
		WHEN 2 THEN N'I gang'
		ELSE N'Afventer'
	END,
	DATEADD(DAY, pa.AddressNumber - 1, p.StartDate),
	CASE WHEN pa.AddressNumber % 4 = 0 THEN DATEADD(DAY, pa.AddressNumber + 2, p.StartDate) END,
	p.ProjectId,
	@UserId
FROM @Projects AS p
INNER JOIN @ProjectAddresses AS pa ON pa.ProjectId = p.ProjectId
WHERE NOT EXISTS
(
	SELECT 1
	FROM dbo.Tasks AS existingTask
	WHERE existingTask.ProjectId = p.ProjectId
	  AND existingTask.LocationId = pa.LocationId
);

COMMIT TRANSACTION;

SELECT
	COUNT(*) AS AntalKunder
FROM dbo.Customers
WHERE Name IN (SELECT CustomerName FROM @CustomerSeed);

SELECT
	COUNT(*) AS AntalProjekter
FROM dbo.Projects
WHERE ProjectTitle IN (SELECT ProjectTitle FROM @ProjectSeed);

SELECT
	COUNT(*) AS AntalOpgaver
FROM dbo.Tasks AS t
INNER JOIN @Projects AS p ON p.ProjectId = t.ProjectId;
