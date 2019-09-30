using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ChangePassword/{Email}/{Name}/{Pwd}/{SessionToken}", "GET")]
    [Route("/ChangePassword", "POST")]
    public class ChangePassword : RequestBase, IReturn<ChangePasswordResponse>
    {
        public string Email
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

    public class ChangePasswordResponse : ResponseBase
    {
    }

}
