﻿
DECLARE @AccountKey NVARCHAR(255) = '5109BF8F9B094CBA99A06A415BCFBEA2'

DELETE FROM dbo.CORE_Alert_Subscription_T WHERE Subscriber_Account_Key = @AccountKey