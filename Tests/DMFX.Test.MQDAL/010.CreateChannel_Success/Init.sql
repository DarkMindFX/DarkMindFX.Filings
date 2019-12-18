DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'

DELETE FROM dbo.CORE_Channel_T WHERE Channel_Name = @ChannelName