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
    [Export("WMT", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class WMT : SECParserBase
    {
        public WMT() : base("10-Q", Resources.WMT)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
