namespace DMFX.Service.DTO
{

    public class Echo : RequestBase
    {
        public string Message
        {
            get; set;
        }
    }

    public class EchoResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public string Message
            {
                get; set;
            }
        }

        public EchoResponse()
        {
            Payload = new ResponsePayload();
        }

        public ResponsePayload Payload
        {
            get;
            set;
        }
    }
}
