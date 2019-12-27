using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser.SECForm13F
{
    [Export("Default", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class SECForm13FDefaultParser : IFilingParser
    {
        public SECForm13FDefaultParser()
        {
        }

        public string ReportType
        {
            get
            {
                return "13F";
            }
        }

        public IFilingParserParams CreateFilingParserParams()
        {
            return new SECParserParams();
        }

        public IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            throw new NotImplementedException();
        }
    }
}
