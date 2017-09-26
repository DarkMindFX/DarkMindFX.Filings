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
    [Export("JNJ", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class JNJ : SECParserBase
    {
        public JNJ() : base("10-K", Resources.JNJ)
        {
        }

        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            IFilingParserResult result = base.Parse(parserParams);
            
            return result;
        }

    }
}
