using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMFX.Interfaces;
using System.ComponentModel.Composition;
using System.Xml;

namespace DMFX.SECParser.SEC10K
{
    [Export("EIX", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EIX : SECParserBase
    {
        public EIX() : base("10-K", Resources.EIX)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
