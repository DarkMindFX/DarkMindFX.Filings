using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ResetPassword/{Email}", "GET")]
    [Route("/ResetPassword", "POST")]
    public class ResetPassword : RequestBase, IReturn<ResetPasswordResponse>
    {
        public string Email
        {
            get;
            set;
        }        
        
    }

    public class ResetPasswordResponse : ResponseBase
    {
        public string AccountKey
        {
            get; set;
        }
    }

}
