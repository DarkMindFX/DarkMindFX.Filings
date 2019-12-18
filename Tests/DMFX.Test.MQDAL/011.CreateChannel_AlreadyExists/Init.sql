DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'
DECLARE @channelID AS BIGINT

DELETE FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName

EXEC dbo.SP_Create_Channel @ChannelName, '1:00:00', @channelID OUTPUT

IF(@channelID IS NULL)
BEGIN
	THROW 50001, 'Init Failed: channel was not created', 1
END

