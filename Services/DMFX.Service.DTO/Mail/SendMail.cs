using ServiceStack;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    public class MailDetails
    {
        public MailDetails()
        {
            Parameters = new Dictionary<string, object>();
        }
        public string ToAddress
        {
            get;
            set;
        }

        public string MessageType
        {
            get;
            set;
        }

        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }

    [Route("/SendMail")]
    public class SendMail : RequestBase, IReturn<SendMailResponse>
    {
        public SendMail()
        {
            Details = new List<MailDetails>();
        }

        public List<MailDetails> Details
        {
            get;
            set;
        }
    }

    public class SendMailResponse : ResponseBase
    {
    }
}
