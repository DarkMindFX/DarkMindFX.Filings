DECLARE @AccountKey NVARCHAR(255) = '#NEW_ACCOUNT_KEY#'
DECLARE @Sub01Name AS NVARCHAR(255) = 'Test Update Sub 01 Updated'
DECLARE @Sub02Name AS NVARCHAR(255) = 'Test Update Sub 02 Updated'

IF (NOT EXISTS(SELECT 1 FROM dbo.CORE_Alert_Subscription_T 
				WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @Sub01Name))
BEGIN
	THROW 51001, 'Subscription Test Update Sub 01 was updated unexpectedly', 1
END

IF (NOT EXISTS(SELECT 1 FROM dbo.CORE_Alert_Subscription_T 
				WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @Sub02Name))
BEGIN
	THROW 51001, 'Subscription Test Update Sub 02 was updated unexpectedly', 1
END

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey