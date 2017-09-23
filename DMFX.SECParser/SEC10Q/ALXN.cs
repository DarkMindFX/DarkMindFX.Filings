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
    [Export("ALXN", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ALXN : SECParserBase
    {
        public ALXN() : base("10-Q", Resources.ALXN)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
