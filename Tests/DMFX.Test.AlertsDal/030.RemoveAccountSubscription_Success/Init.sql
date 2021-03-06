﻿
DECLARE @AccountKey AS NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'
DECLARE @Sub01Name AS NVARCHAR(255) = 'Test Subscription 03 To Remove'
DECLARE @EmailId AS BIGINT
DECLARE @CustomUrlId AS BIGINT
DECLARE @dataEmail AS TYPE_Alert_Subscription_Data
DECLARE @dataCustomUrl AS TYPE_Alert_Subscription_Data

IF(NOT EXISTS(SELECT 1 FROM CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey))
BEGIN

	-- 1. Email subscription
	INSERT INTO @dataEmail
	SELECT 'Email', 'test01@darkmindfx.com'

	SET @EmailId = dbo.FN_GetAlertNotificationTypeId('Email')

	EXEC SP_Add_Alert_Subscription
		@Sub01Name, 
		@AccountKey,
		1,
		@EmailId,
		'2019-11-01',
		@dataEmail

	
END

