using DMFX.CFTC.Api;
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
        public IQuotesSourceCanImportResult CanImport(IQuotesSourceCanImportParams canImportParams)
        {
            IQuotesSourceCanImportResult result = new CFTCSourceCanImportResult();
            
            ICFTCParserParams[] paramTypes = new ICFTCParserParams[]
            {
                new CFTCParserParamsCOTCmdtsFut(),
                new CFTCParserParamsCOTCmdtsFutOpt(),
                new CFTCParserParamsCOTFinFut(),
                new CFTCParserParamsCOTFinFutOpt()
            };

            foreach (var ticker in canImportParams.Tickers)
            {
                bool found = false;
                int i = 0;
                while (!found && i < paramTypes.Count())
                {
                    found = ticker.IndexOf(paramTypes[i].TickerPrefix) >= 0;
                    ++i;
                }

                if (found)
                {
                    result.Tickers.Add(ticker);
                }
            }

            result.Success = result.Tickers.Count > 0;

            return result;
        }        

        public IQuotesSourceGetQuotesResult GetQuotes(IQuotesSourceGetQuotesParams getQuotesParams)
        {
            throw new NotImplementedException();
        }

        public void Init(IQuotesSourceInitParams initParams)
        {
            
        }

        #region Create* functions
        public IQuotesSourceCanImportParams CreateCanImportParams()
        {
            return new CFTCSourceCanImportParams();
        }

        public IQuotesSourceGetQuotesParams CreateGetQuotesParams()
        {
            return new CFTCSourceGetQuotesParams();
        }

        public IQuotesSourceInitParams CreateInitParams()
        {
            return new CFTCSourceInitParams();
        }
        #endregion
    }
}
