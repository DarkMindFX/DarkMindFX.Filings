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
    [Export("BF_B", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BF_B : SECParserBase
    {
        public BF_B() : base("10-Q", Resources.BF_B)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
