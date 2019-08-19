﻿using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Service.DTO
{
    [Route("/ForceRunImportTimeSeries/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DaysBack}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DaysBack}/{SymbolCodes}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DateStart}/{DateEnd}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries/{DateStart}/{DateEnd}/{SymbolCodes}/{Timeframe}/{SessionToken}", "GET")]
    [Route("/ForceRunImportTimeSeries", "PUT")]
    public class ForceRunImportTimeSeries : RequestBase, IReturn<ForceRunImportTimeSeriesResponse>
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

        public ETimeFrame? Timeframe
        {
            get;
            set;
        }
    }

    public class ForceRunImportTimeSeriesResponse : ResponseBase
    {
    }
}
