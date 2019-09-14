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

        public virtual string[] SplitLine(string line)
        {
            string[] vals = line.Trim().Split(new char[] { ',' });

            return vals;
        }
    }

    public class CFTCParserParamsCOTFinFutOpt : CFTCParserParamsBase
    {
        public CFTCParserParamsCOTFinFutOpt()
        {
            Init();
        }

        private void Init()
        {
            TickerPrefix = "COT.FinFutOpt.";
            RecentDataPath = string.Format("/files/dea/history/com_fin_txt_{0}.zip", DateTime.Now.Year);
                        
            for (int i = DateTime.Now.Year - 1; i > 2016 ; --i)
            {
                HistorticalDataPaths.Add(string.Format("/files/dea/history/com_fin_txt_{0}.zip", i));
            }
            HistorticalDataPaths.Add("/files/dea/history/fin_com_txt_2006_2016.zip");

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
    public class CFTCParserParamsCOTCmdtsFutOpt : CFTCParserParamsBase
    {
        public CFTCParserParamsCOTCmdtsFutOpt()
        {
            Init();
        }

        private void Init()
        {
            TickerPrefix = "COT.CmdtsFutOpt.";
            RecentDataPath = string.Format("/files/dea/history/com_disagg_txt_{0}.zip", DateTime.Now.Year);

            for (int i = DateTime.Now.Year - 1; i > 2016; --i)
            {
                HistorticalDataPaths.Add(string.Format("/files/dea/history/com_disagg_txt_{0}.zip", i));
            }
            HistorticalDataPaths.Add("/files/dea/history/com_disagg_txt_hist_2006_2016.zip");

            ColumnsToTimeseriesMapping.Add(7, "Open_Interest_All");
            ColumnsToTimeseriesMapping.Add(8, "Prod_Merc_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(9, "Prod_Merc_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(10, "Swap_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(11, "Swap_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(13, "M_Money_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(14, "M_Money_Positions_Short_All");
            ColumnsToTimeseriesMapping.Add(16, "Other_Rept_Positions_Long_All");
            ColumnsToTimeseriesMapping.Add(18, "Other_Rept_Positions_Short_All");

        }

   
    }

    public class CFTCParserResult : ICFTCParserResult
    {
        public CFTCParserResult()
        {
            Instruments = new Dictionary<string, CFTCInstrumentQuotes>();
        }

        public Dictionary<string, CFTCInstrumentQuotes> Instruments
        {
            get;
            set;
        }
    }
}
