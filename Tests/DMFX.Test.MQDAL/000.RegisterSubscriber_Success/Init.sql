DECLARE @SubsName AS NVARCHAR(255) = 'TestSenderSrv-QAZXSWP'

DELETE FROM dbo.CORE_Subscriber_T WHERE Subscriber_Name = @SubsName