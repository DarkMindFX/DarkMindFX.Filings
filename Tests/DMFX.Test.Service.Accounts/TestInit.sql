
DECLARE @AccountKey NVARCHAR(255) = 'DC43D270C3BCC2FF88E9FC9081C256D7B2A7ABCD12'

IF NOT EXISTS(SELECT * FROM dbo.UM_Account_Key_T WHERE Account_Key_Value = @AccountKey)
BEGIN
	/* Creating user for unit tests*/
	EXEC SP_Create_User_Account
		'AccountsUnitTestUser',
		'AccountsUnitTestUser@gmail.com',
		'#AccountsUser_PWD_HASH#',
		@AccountKey
END