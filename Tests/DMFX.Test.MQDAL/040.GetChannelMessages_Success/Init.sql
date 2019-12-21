DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'
DECLARE @channelID AS BIGINT

DELETE FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName

EXEC dbo.SP_Create_Channel @ChannelName, '1:00:00', @channelID OUTPUT

DECLARE @SubsName AS NVARCHAR(255) = 'TestSenderSrv-QAZXSWP'
DECLARE @RecName AS NVARCHAR(255) = 'TestReceiverSrv-EDCVFRL'
DECLARE @SubsId AS BIGINT
DECLARE @RecId AS BIGINT

DELETE FROM dbo.CORE_Subscriber_T 
WHERE Subscriber_Name = @SubsName
OR Subscriber_Name = @RecName

EXEC dbo.SP_Register_Subscriber @SubsName, NULL, @SubsId OUTPUT

IF(@SubsId IS NULL)
BEGIN
	THROW 50001, 'Init Failed: subscriber was not created', 1
END

EXEC dbo.SP_Register_Subscriber @RecName, NULL, @RecId OUTPUT

IF(@RecId IS NULL)
BEGIN
	THROW 50001, 'Init Failed: receiver was not created', 1
END

DECLARE @msgType AS NVARCHAR(255) = 'TestMessageType'
DECLARE @msgBody AS NVARCHAR(255) = 'TestMessagePayload'

DELETE m
FROM dbo.CORE_Message_T m
WHERE	m.Message_Type LIKE '%' + @msgType + '%' AND 
		m.Message_Payload LIKE  '%' + @msgBody + '%'