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
    [Export("INTU", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class INTU : SECParserBase
    {
        public INTU() : base("10-K", Resources.INTU)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}