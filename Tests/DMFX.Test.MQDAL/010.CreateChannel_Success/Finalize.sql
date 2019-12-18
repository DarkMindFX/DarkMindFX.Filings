DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'

IF(NOT EXISTS(SELECT 1 FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName))
BEGIN
	THROW 50001, 'Channel wasn''t created', 1
END

DELETE FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName