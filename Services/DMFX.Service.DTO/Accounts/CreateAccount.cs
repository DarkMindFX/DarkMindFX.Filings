﻿using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/CreateAccount/{Email}/{Name}/{Pwd}", "GET")]
    [Route("/CreateAccount", "POST")]
    public class CreateAccount : RequestBase, IReturn<CreateAccountResponse>
    {
        public string Email
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Pwd
        {
            get;
            set;
        }
        
    }

    public class CreateAccountResponse : ResponseBase
    {
        public class ResponsePayload
        {
            public string AccountKey
            {
                get; set;
            }
        }

        public CreateAccountResponse()
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
