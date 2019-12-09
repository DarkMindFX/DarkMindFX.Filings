
DECLARE @AccountKey NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @Name NVARCHAR(255) = 'Test Add Sub 01 InvalidToken'

IF (EXISTS(SELECT 1 FROM dbo.CORE_Alert_Subscription_T 
				WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @Name))
BEGIN
	THROW 51001, 'Subscription was added using invalid token', 1
END

-- cleaning up
DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey