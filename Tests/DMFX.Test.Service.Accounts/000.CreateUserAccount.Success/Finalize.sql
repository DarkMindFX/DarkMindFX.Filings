

DECLARE @email AS NVARCHAR(255) = 'TestCreateNewUser@test.com'

IF(EXISTS(SELECT 1 FROM [dbo].UM_User_T WHERE Email = @email)) 
BEGIN	
	DELETE s FROM dbo.UM_Active_Session_T s
	INNER JOIN dbo.UM_User_T u ON s.User_Id = u.User_Id
	WHERE u.Email = @email

	DELETE s FROM dbo.UM_Dead_Session_T s
	INNER JOIN dbo.UM_User_T u ON s.User_Id = u.User_Id
	WHERE u.Email = @email

	DELETE FROM dbo.UM_User_T
	WHERE Email = @email

	DELETE a FROM dbo.UM_Account_Key_T a
	INNER JOIN dbo.UM_User_T u ON a.Account_Key_Id = u.Account_Key_Id
	WHERE u.Email = @email 
END