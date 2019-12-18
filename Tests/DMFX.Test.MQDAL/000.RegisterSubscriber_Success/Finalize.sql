DECLARE @SubsName AS NVARCHAR(255) = 'TestSenderSrv-QAZXSWP'

IF(NOT EXISTS(SELECT 1 FROM dbo.CORE_Subscriber_T WHERE Subscriber_Name = @SubsName))
BEGIN
	THROW 50001, 'Subscriber was not registered', 1	
END

DELETE FROM dbo.CORE_Subscriber_T WHERE Subscriber_Name = @SubsName

