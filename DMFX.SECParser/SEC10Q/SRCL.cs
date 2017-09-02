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
    [Export("SRCL", typeof(IFilingParser))]
    public class SRCL : SECParserBase
    {
        public SRCL() : base("10-Q", Resources.SRCL)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
