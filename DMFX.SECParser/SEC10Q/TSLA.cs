using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using System.ComponentModel.Composition;

namespace DMFX.SECParser.SEC10Q
{
    [Export("SEC_10-Q_TSLA", typeof(IFilingParser))]
    public class TSLA : SECParserBase
    {
        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            SECParserParams secParams = parserParams as SECParserParams;

            SECParserResult result = new SECParserResult();

            try
            {
                ValidateFile(secParams, result);
                if (result.Success)
                {
                    var doc = OpenDocument(secParams);
                    if (doc != null)
                    {
                        ExtractCompanyData(doc, result);
                        ExtractFilingData(doc, result);

                        // CONDENSED CONSOLIDATED STATEMENTS OF OPERATIONS
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.AddError(EErrorCodes.ParserError, EErrorType.Error, ex.Message);
            }

            return result;
        }
    }
}
