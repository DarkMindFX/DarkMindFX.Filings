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
    [Export("LRCX", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LRCX : SECParserBase
    {
        public LRCX() : base("10-Q", Resources.LRCX)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
