﻿using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser.SEC10Q
{
    [Export("JNJ", typeof(IFilingParser))]
    public class JNJ : SECParserBase
    {
        public JNJ() : base("10-Q")
        {
        }
        public override IFilingParserResult Parse(IFilingParserParams parserParams)
        {
            throw new NotImplementedException();
        }
    }
}
