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
    [Export("UNH", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UNH : SECParserBase
    {
        public UNH() : base("10-K", Resources.UNH)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}