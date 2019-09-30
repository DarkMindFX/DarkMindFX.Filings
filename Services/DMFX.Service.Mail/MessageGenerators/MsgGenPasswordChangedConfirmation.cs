using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

namespace DMFX.Service.Mail.MessageGenerators
{
    [Export("PasswordChangedConfirmation", typeof(IMessageGenerator))]
    public class MsgGenPasswordChangedConfirmation : IMessageGenerator
    {
        public string CreateMessage(IMessageGeneratorParams msgParams)
        {
            string result = msgParams.Template;

            result = string.Format(result,
                msgParams.Parameters["UserName"]
                );

            return result;
        }

        public IMessageGeneratorParams CreateParams()
        {
            var result = new BaseMessageGeneratorParams();

            result.Parameters.Add("UserName", string.Empty);
            
            return result;
        }
    }
}