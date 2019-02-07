using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO.QuotesSourcing
{
    [Route("/GetImporterQuotesState/{SessionToken}", "GET")]
    [Route("/GetImporterQuotesState", "POST")]
    public class GetImporterQuotesState : RequestBase, IReturn<GetImporterQuotesStateResponse>
    {
    }

    public class GetImporterQuotesStateResponse : ResponseBase
    {
        public GetImporterQuotesStateResponse()
        {
            QuotesProcessed = new List<TickerInfo>();
        }
        public string State
        {
            get;
            set;
        }

        public DateTime? LastImportRun
        {
            get;
            set;
        }

        public DateTime? LastImportEnd
        {
            get;
            set;
        }

        public int ProcessedCount
        {
            get;
            set;
        }

        public List<TickerInfo> QuotesProcessed
        {
            get;
            set;
        }
    }
}
