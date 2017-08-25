﻿using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser.SEC10Q
{
    [Export("SEC_10-Q_MSFT", typeof(IFilingParser))]
    public class MSFT : SECParserBase
    {
        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            throw new NotImplementedException();
        }
    }
}
