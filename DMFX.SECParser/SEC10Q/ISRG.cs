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
    [Export("ISRG", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ISRG : SECParserBase
    {
        public ISRG() : base("10-Q", Resources.ISRG)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
