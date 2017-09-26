using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser.SEC10K
{
    [Export("AAPL", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AAPL : SECParserBase
    {
        public AAPL() : base("10-K", Resources.AAPL)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);

            return result;
        }

    }
}
