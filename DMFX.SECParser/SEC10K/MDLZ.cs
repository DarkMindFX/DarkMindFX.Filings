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
    [Export("MDLZ", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MDLZ : SECParserBase
    {
        public MDLZ() : base("10-K", Resources.MDLZ)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
