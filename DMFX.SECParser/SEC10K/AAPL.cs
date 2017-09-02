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
    public class AAPL : SECParserBase
    {
        public AAPL() : base("10-K")
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            throw new NotImplementedException();
        }
    }
}
