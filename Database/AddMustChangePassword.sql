IF COL_LENGTH('dbo.UserAuthentications', 'MustChangePassword') IS NULL
BEGIN
	ALTER TABLE dbo.UserAuthentications
	ADD MustChangePassword bit NOT NULL
		CONSTRAINT DF_UserAuthentications_MustChangePassword DEFAULT (1) WITH VALUES;
END;
GO

-- Use this for accounts that should not be forced to change their password.
-- UPDATE dbo.UserAuthentications
-- SET MustChangePassword = 0;
