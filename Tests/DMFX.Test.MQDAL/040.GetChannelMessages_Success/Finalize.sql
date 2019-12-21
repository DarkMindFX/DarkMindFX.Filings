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
	WHERE m.Message_Type LIKE '%' + @msgType + '%' AND m.Message_Payload LIKE '%' +  @msgBody + '%'))
BEGIN
	THROW 50001, 'Messages were not sent', 1
END


EXEC dbo.SP_Get_Subscriber_Id @SubsName, @SubsId OUTPUT
EXEC dbo.SP_Get_Subscriber_Id @RecName, @RecId OUTPUT


DELETE m FROM dbo.CORE_Message_T m 
WHERE 
	m.Message_Type LIKE		'%' + @msgType + '%' AND 
	m.Message_Payload LIKE  '%' + @msgBody + '%'

DELETE c FROM dbo.CORE_Channel_T c WHERE c.Channel_Name = @ChannelName

DELETE s FROM dbo.CORE_Subscriber_T s WHERE s.Subscriber_Name = @SubsName

DELETE s FROM dbo.CORE_Subscriber_T s WHERE s.Subscriber_Name = @RecName