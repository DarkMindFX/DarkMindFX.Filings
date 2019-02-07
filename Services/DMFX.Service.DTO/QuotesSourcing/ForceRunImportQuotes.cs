using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImportQuotes/{SessionToken}", "GET")]
    [Route("/ForceRunImportQuotes/{DaysBack}/{SessionToken}", "GET")]
    [Route("/ForceRunImportQuotes/{DaysBack}/{SymbolCodes}/{SessionToken}", "GET")]
    [Route("/ForceRunImportQuotes/{DateStart}/{DateEnd}/{SessionToken}", "GET")]
    [Route("/ForceRunImportQuotes/{DateStart}/{DateEnd}/{SymbolCodes}/{SessionToken}", "GET")]
    [Route("/ForceRunImportQuotes", "PUT")]
    public class ForceRunImportQuotes : RequestBase, IReturn<ForceRunImportQuotesResponse>
    {
        public DateTime? DateStart
        {
            get;
            set;
        }

        public DateTime? DateEnd
        {
            get;
            set;
        }

        public string[] SymbolCodes
        {
            get;
            set;
        }

        public decimal? DaysBack
        {
            get;
            set;
        }
    }

    public class ForceRunImportQuotesResponse : ResponseBase
    {
    }
}
