DECLARE @AccountKey AS NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @SubName AS NVARCHAR(255) = 'Test Subscription 04 Status Changed'


DECLARE @NewStatusId AS BIGINT


SELECT 
	@NewStatusId = Alert_Subscription_Status_Id
FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @SubName

IF @NewStatusId = dbo.FN_GetAlertSubscriptionStatusId('Enabled') 
BEGIN
	THROW 51001, 'Status was not updated', 1
END



DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @SubName