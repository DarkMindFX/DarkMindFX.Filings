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
    [Export("JNPR", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class JNPR : SECParserBase
    {
        public JNPR() : base("10-K", Resources.JNPR)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
