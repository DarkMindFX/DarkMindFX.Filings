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
    [Export("BHF", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BHF : SECParserBase
    {
        public BHF() : base("10-K", Resources.BHF)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
