using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.CFTC.Api
{
    public abstract class CFTCParserParamsBase : ICFTCParserParams
    {
        public CFTCParserParamsBase()
        {
            HistorticalDataPaths = new List<string>();
            ColumnsToTimeseriesMapping = new Dictionary<int, string>();
            OnlyLast = false;
        }

        public Dictionary<int, string> ColumnsToTimeseriesMapping
        {
            get;
            protected set;
        }

        public IList<string> HistorticalDataPaths
        {
            get;
            protected set;
        }

        public string RecentDataPath
        {
            get;
            protected set;
        }

        public string TickerPrefix
        {
            get;
            protected set;
        }

        public bool OnlyLast
        {
            get;
            set;
        }
    }

    class CFTCParserParamsCOTFinFutOpt : CFTCParserParamsBase
    {
        public CFTCParserParamsCOTFinFutOpt()
        {
            Init();
        }

        private void Init()
        {
            TickerPrefix = "COT.FinFutOpt";
            RecentDataPath = string.Format("/files/dea/history/com_fin_txt_{0}.zip", DateTime.Now.Year);

            HistorticalDataPaths.Add("/files/dea/history/fin_com_txt_2006_2016.zip");
            for (int i = 2016; i < DateTime.Now.Year; ++i)
            {
                HistorticalDataPaths.Add(string.Format("/files/dea/history/com_fin_txt_{0}.zip", i));
            }

            ColumnsToTimeseriesMapping.Add(7, "Open_Interest_All");
            ColumnsToTimeseriesMapping.Add(8, "Dealer_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(9, "Dealer_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(11, "Asset_Mgr_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(12, "Asset_Mgr_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(14, "Lev_Money_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(15, "Lev_Money_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(17, "Other_Rept_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(18, "Other_Rept_Positions_Short_All");

        }
    }

    class CFTCParserResult : ICFTCParserResult
    {
        public CFTCParserResult()
        {
            Instruments = new HashSet<CFTCInstrumentQuotes>();
        }

        public HashSet<CFTCInstrumentQuotes> Instruments
        {
            get;
            set;
        }
    }
}
