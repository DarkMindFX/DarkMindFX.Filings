DECLARE @AccountKey NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @Sub01Name AS NVARCHAR(255) = 'Test Remove Sub 01'
DECLARE @Sub02Name AS NVARCHAR(255) = 'Test Remove Sub 02'

IF (EXISTS(SELECT 1 FROM dbo.CORE_Alert_Subscription_T 
				WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @Sub01Name))
BEGIN
	THROW 51001, 'Subscription Test Remove Sub 01 was not removed', 1
END

IF (EXISTS(SELECT 1 FROM dbo.CORE_Alert_Subscription_T 
				WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @Sub02Name))
BEGIN
	THROW 51001, 'Subscription Test Remove Sub 02 was not removed', 1
END

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey