DECLARE @SubsName AS NVARCHAR(255) = 'TestSenderSrv-QAZXSWP'
DECLARE @RecName AS NVARCHAR(255) = 'TestReceiverSrv-EDCVFRL'
DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'
DECLARE @msgType AS NVARCHAR(255) = 'TestMessageType'
DECLARE @msgBody AS NVARCHAR(255) = 'TestMessagePayload'
DECLARE @channelID AS BIGINT
DECLARE @SubsId AS BIGINT
DECLARE @RecId AS BIGINT
DECLARE @msgId AS BIGINT

IF(NOT EXISTS(SELECT 1 FROM dbo.CORE_Message_T m
	WHERE m.Message_Type = @msgType AND m.Message_Payload = @msgBody))
BEGIN
	THROW 50001, 'Message was not sent', 1
END


EXEC dbo.SP_Get_Subscriber_Id @SubsName, @SubsId OUTPUT
EXEC dbo.SP_Get_Subscriber_Id @RecName, @RecId OUTPUT

SELECT @msgId = m.Message_Id
FROM dbo.CORE_Message_T m
WHERE 
	m.Sender_Id = @SubsId AND
	m.Message_Type = @msgType AND
	m.Message_Payload = @msgBody

IF(NOT EXISTS(
	SELECT 1 FROM dbo.CORE_Message_Routing_T mr 
	INNER JOIN dbo.CORE_Message_T m ON mr.Message_Id = mr.Message_Id
	INNER JOIN dbo.CORE_Channel_Subscription_T cs ON cs.Channel_Id = m.Channel_Id and cs.Subscriber_Id = @RecId
	
	WHERE 
		mr.Message_Id = @msgId AND
		mr.Channel_Subscription_Id = cs.Channel_Subscription_Id
))
BEGIN
	THROW 50001, 'Message was not routed to receiver', 1
END

DELETE m FROM dbo.CORE_Message_T m WHERE m.Message_Id = @msgId

DELETE c FROM dbo.CORE_Channel_T c WHERE c.Channel_Name = @ChannelName

DELETE s FROM dbo.CORE_Subscriber_T s WHERE s.Subscriber_Name = @SubsName

DELETE s FROM dbo.CORE_Subscriber_T s WHERE s.Subscriber_Name = @RecName