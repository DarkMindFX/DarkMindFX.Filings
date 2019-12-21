DECLARE @ChannelName AS NVARCHAR(255) = 'TestChannel-TGBNHYM'
DECLARE @invalidSubscriberId AS BIGINT = 666000666

IF(EXISTS(
		SELECT
		 1
		FROM dbo.CORE_Channel_Subscription_T cs
		INNER JOIN dbo.CORE_Channel_T c ON c.Channel_Id  = cs.Channel_Id
		INNER JOIN dbo.CORE_Subscriber_T s ON s.Subscriber_Id = cs.Subscriber_Id
		WHERE
			c.Channel_Name = @ChannelName
			AND s.Subscriber_Id = @invalidSubscriberId))
BEGIN
	THROW 50001, 'Subscription succeeded uexpectedly', 1
END

DELETE cs
FROM dbo.CORE_Channel_Subscription_T cs
INNER JOIN dbo.CORE_Channel_T c ON c.Channel_Id  = cs.Channel_Id
WHERE
	c.Channel_Name = @ChannelName
