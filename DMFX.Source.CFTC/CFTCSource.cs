using DMFX.QuotesInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Source.CFTC
{
    [Export("CFTC", typeof(IQuotesSource))]
    public class CFTCSource : IQuotesSource
    {
        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            throw new NotImplementedException();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            throw new NotImplementedException();
        }

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            throw new NotImplementedException();
        }

        public void Init(IQuotesSourceInitParams initParams)
        {
            throw new NotImplementedException();
        }
    }
}
