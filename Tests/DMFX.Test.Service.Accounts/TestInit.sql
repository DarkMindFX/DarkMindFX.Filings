
DECLARE @AccountKey NVARCHAR(255) = 'DC43D270C3BCC2FF88E9FC9081C256D7B2A7ABCD12'
DECLARE @Email NVARCHAR(255) = 'AccountsUnitTestUser@test.com'

IF NOT EXISTS(SELECT * FROM dbo.UM_User_T WHERE Email = @Email)
BEGIN
	/* Creating user for unit tests*/
	EXEC SP_Create_User_Account
		'AccountsUnitTestUser',
		@Email,
		'/lIGdrGh2T2rqyMZ7qA2dPNjLq7rFj0eiCRPXrHeEOs=',
		@AccountKey
END