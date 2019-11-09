using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/GetAccountInfo/{SessionToken}", "GET")]
    [Route("/GetAccountInfo", "POST")]
    public class GetAccountInfo : RequestBase, IReturn<GetAccountInfoResponse>
    {
        
    }

    public class GetAccountInfoResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public string Name
            {
                get;
                set;
            }

            public string AccountKey
            {
                get;
                set;
            }

            public string Email
            {
                get;
                set;
            }

            public DateTime DateCreated
            {
                get;
                set;
            }

            public string DateCreatedStr
            {
                get;
                set;
            }

            public DateTime DateExpires
            {
                get;
                set;
            }

            public string DateExpiresStr
            {
                get;
                set;
            }
        }
        public GetAccountInfoResponse()
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
