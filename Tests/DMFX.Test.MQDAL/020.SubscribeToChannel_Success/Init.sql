DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'
DECLARE @channelID AS BIGINT

DELETE FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName

EXEC dbo.SP_Create_Channel @ChannelName, '1:00:00', @channelID OUTPUT

DECLARE @SubsName AS NVARCHAR(255) = 'TestSenderSrv-QAZXSWP'
DECLARE @SubsId AS BIGINT


DELETE FROM dbo.CORE_Subscriber_T WHERE Subscriber_Name = @SubsName

EXEC dbo.SP_Register_Subscriber @SubsName, NULL, @SubsId OUTPUT

IF(@SubsId IS NULL)
BEGIN
	THROW 50001, 'Init Failed: subscriber was not created', 1
END