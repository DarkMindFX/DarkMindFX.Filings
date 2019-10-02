
-- checking if update was successful
DECLARE @accountKey AS NVARCHAR(255) = 'DC43D270C3BCC2FF88E9FC9081C256D7B2A7ABCD12'
DECLARE @userName AS NVARCHAR(255)
DECLARE @email AS NVARCHAR(255)
DECLARE @pwdHash AS NVARCHAR(255)

SELECT 
	@userName = u.User_Name,
	@email = u.Email,
	@pwdHash = u.Password_Hash
FROM 
	dbo.UM_User_T u
INNER JOIN dbo.UM_Account_Key_T a ON a.account_Key_Id = u.Account_Key_Id
WHERE a.Account_Key_Value = @accountKey

-- reverting back changes (if any)
UPDATE u
SET 
	u.User_Name = 'AccountsUnitTestUser',
	u.Email = 'AccountsUnitTestUser@test.com',
	u.Password_Hash = '/lIGdrGh2T2rqyMZ7qA2dPNjLq7rFj0eiCRPXrHeEOs='
FROM dbo.UM_User u
INNER JOIN dbo.UM_Account_Key_T a ON a.account_Key_Id = u.Account_Key_Id
WHERE a.Account_Key_Value = @accountKey

-- throwing error
IF @userName <> 'AccountsUnitTestUser' THROW 50001, 'User name was updated', 1
IF @email <> 'AccountsUnitTestUser@test.com' THROW 50001, 'Email was updated', 1
IF @pwdHash <> 'B2cXi/G+Y0cP7jkZklo02sI2oguDjRKO4xg0OBymze4=' THROW 50001, 'Password hash was not updated', 1

