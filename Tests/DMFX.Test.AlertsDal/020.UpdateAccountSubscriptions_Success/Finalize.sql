DECLARE @AccountKey AS NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @SubNameLike AS NVARCHAR(255) = '%Test Subscription 02%'
DECLARE @SubName AS NVARCHAR(255) = 'Test Subscription 02 Updated'

DECLARE @ID AS BIGINT
DECLARE @NewName AS NVARCHAR(255)
DECLARE @NewNotTypeId AS BIGINT
DECLARE @NewSubscribedDttm AS DATETIME

SELECT 
	@ID = Alert_Subscription_Id,
	@NewName = Alert_Subscription_Name,
	@NewNotTypeId = Notification_Type_Id,
	@NewSubscribedDttm = Subscribed_Dttm
FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name = @SubName

IF @NewName <> @SubName 
BEGIN
	THROW 51001, 'Name was not updated', 1
END

IF @NewNotTypeId <> 1
BEGIN
	THROW 51001, 'Notification Type was not updated', 1
END

IF @NewSubscribedDttm = '2019-11-01'
BEGIN
	THROW 51001, 'Subscribed Date was not updated', 1
END

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey AND Alert_Subscription_Name LIKE @SubName