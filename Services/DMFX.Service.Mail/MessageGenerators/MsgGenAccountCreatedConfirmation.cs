using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

namespace DMFX.Service.Mail.MessageGenerators
{
    [Export("AccountCreatedConfirmation", typeof(IMessageGenerator))]
    public class MsgGenAccountCreatedConfirmation : IMessageGenerator
    {
        public string CreateMessage(IMessageGeneratorParams msgParams)
        {
            string result = msgParams.Template;

            result = string.Format(result,
                msgParams.Parameters["UserName"],
                msgParams.Parameters["Login"],
                msgParams.Parameters["AccountKey"]
                );

            return result;
        }

        public IMessageGeneratorParams CreateParams()
        {
            var result = new BaseMessageGeneratorParams();

            result.Parameters.Add("UserName", string.Empty);
            result.Parameters.Add("Login", string.Empty);
            result.Parameters.Add("AccountKey", string.Empty);

            return result;
        }
    }
}