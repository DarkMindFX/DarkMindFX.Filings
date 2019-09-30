using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

namespace DMFX.Service.Mail.MessageGenerators
{
    [Export("NewReportNotification", typeof(IMessageGenerator))]
    public class MsgGenNewReportNotification : IMessageGenerator
    {
        public string CreateMessage(IMessageGeneratorParams msgParams)
        {
            string result = msgParams.Template;

            result = string.Format(result,
                msgParams.Parameters["UserName"],
                msgParams.Parameters["RegulatorCode"],
                msgParams.Parameters["CompanyCode"],
                msgParams.Parameters["ReportType"],
                msgParams.Parameters["ReportName"],
                msgParams.Parameters["ReportSubmitted"]
                );

            return result;
        }

        public IMessageGeneratorParams CreateParams()
        {
            var result = new BaseMessageGeneratorParams();

            result.Parameters.Add("UserName", string.Empty);
            result.Parameters.Add("RegulatorCode", string.Empty);
            result.Parameters.Add("CompanyCode", string.Empty);
            result.Parameters.Add("ReportType", string.Empty);
            result.Parameters.Add("ReportName", string.Empty);
            result.Parameters.Add("ReportSubmitted", string.Empty);

            return result;
        }
    }
}