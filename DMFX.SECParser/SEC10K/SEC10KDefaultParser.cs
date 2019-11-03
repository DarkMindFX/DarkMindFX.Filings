using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser.SEC10K
{
    [Export("Default", typeof(IFilingParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SEC10KDefaultParser : SECParserBase
    {
        public SEC10KDefaultParser() : base("10-K", null)
        {
        }
    }
}
