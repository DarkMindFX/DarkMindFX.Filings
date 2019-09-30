using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

namespace DMFX.Service.Mail.MessageGenerators
{
    [Export("PasswordResetNotification", typeof(IMessageGenerator))]
    public class MsgGenPasswordResetNotification : IMessageGenerator
    {
        public string CreateMessage(IMessageGeneratorParams msgParams)
        {
            string result = msgParams.Template;

            result = string.Format(result,
                msgParams.Parameters["UserName"],
                msgParams.Parameters["Password"]
                );

            return result;
        }

        public IMessageGeneratorParams CreateParams()
        {
            var result = new BaseMessageGeneratorParams();

            result.Parameters.Add("UserName", string.Empty);
            result.Parameters.Add("Password", string.Empty);

            return result;
        }
    }
}