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
    [Export("CTXS", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CTXS : SECParserBase
    {
        public CTXS() : base("10-K", Resources.CTXS)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
