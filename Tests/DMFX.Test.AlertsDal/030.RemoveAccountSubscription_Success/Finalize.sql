DECLARE @AccountKey AS NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @Sub01Name AS NVARCHAR(255) = 'Test Subscription 03 To Remove'

IF(EXISTS(	SELECT 1 
				FROM CORE_Alert_Subscription_T 
				WHERE 
						Subscriber_Account_Key = @AccountKey AND 
						Alert_Subscription_Name = @Sub01Name))
BEGIN
	THROW 51000, 'Subscription was not removed', 1
END

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND 
						Alert_Subscription_Name = @Sub01Name