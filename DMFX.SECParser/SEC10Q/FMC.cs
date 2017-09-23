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
    [Export("FMC", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FMC : SECParserBase
    {
        public FMC() : base("10-Q", Resources.FMC)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
