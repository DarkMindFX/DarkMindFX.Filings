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
    [Export("AYI", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AYI : SECParserBase
    {
        public AYI() : base("10-Q", Resources.AYI)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
