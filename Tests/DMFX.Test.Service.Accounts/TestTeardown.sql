

DECLARE @AccountKey NVARCHAR(255) = 'DC43D270C3BCC2FF88E9FC9081C256D7B2A7ABCD12'
DECLARE @Email NVARCHAR(255) = 'AccountsUnitTestUser@test.com'

DELETE s FROM dbo.UM_Active_Session_T s
INNER JOIN dbo.UM_User_T u ON u.User_Id = s.User_Id
WHERE u.Email = @Email

DELETE s FROM dbo.UM_Dead_Session_T s
INNER JOIN dbo.UM_User_T u ON u.User_Id = s.User_Id
WHERE u.Email = @Email

DELETE u FROM dbo.UM_User_T u
WHERE u.Email = @Email

DELETE acc FROM dbo.UM_Account_Key_T acc
WHERE acc.Account_Key_Value = @AccountKey