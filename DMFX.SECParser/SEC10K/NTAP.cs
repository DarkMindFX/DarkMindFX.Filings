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
    [Export("NTAP", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NTAP : SECParserBase
    {
        public NTAP() : base("10-K", Resources.NTAP)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}