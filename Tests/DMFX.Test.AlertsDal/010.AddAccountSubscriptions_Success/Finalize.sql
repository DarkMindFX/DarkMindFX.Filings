DECLARE @AccountKey AS NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'

IF(NOT EXISTS(	SELECT 1 
				FROM CORE_Alert_Subscription_T 
				WHERE 
						Subscriber_Account_Key = @AccountKey AND 
						Alert_Subscription_Name = 'Test Subscription 01'))
BEGIN
	THROW 51000, 'Subscription was not added', 1
END

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND 
						Alert_Subscription_Name = 'Test Subscription 01'