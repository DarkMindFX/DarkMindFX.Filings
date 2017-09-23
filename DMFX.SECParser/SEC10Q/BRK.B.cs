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
    [Export("BRK_B", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BRK_B : SECParserBase
    {
        public BRK_B() : base("10-Q", Resources.BRK_B)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
