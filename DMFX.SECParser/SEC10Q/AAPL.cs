using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using System.ComponentModel.Composition;
using System.Xml;

namespace DMFX.SECParser.SEC10Q
{
    [Export("AAPL", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AAPL : SECParserBase
    {
        public AAPL() : base("10-Q", Resources.AAPL)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
