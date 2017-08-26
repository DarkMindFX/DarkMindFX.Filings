using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface IFilingParserParams
    {
        Stream FileContent
        {
            get;
            set;
        }
    }

    public class StatementRecord
    {
        public StatementRecord(string title, decimal value, string unit, DateTime periodStart, DateTime periodEnd, DateTime instant)
        {
            Title = title;
            Value = value;
            Unit = unit;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            Instant = instant;
        }
        public string Title
        {
            get;
            set;
        }

        public decimal Value
        {
            get;
            set;
        }

        public string Unit
        {
            get;
            set;
        }

        public DateTime PeriodStart
        {
            get;
            set;
        }

        public DateTime PeriodEnd
        {
            get;
            set;
        }

        public DateTime Instant
        {
            get;
            set;
        }
    }


    public class Statement
    {
        public Statement(string title = null)
        {
            Records = new List<StatementRecord>();
            Title = title;
        }

        public string Title
        {
            get;
            set;
        }

        public List<StatementRecord> Records
        {
            get;
        }
    }

    public interface IFilingParserResult : IResult
    {
        string RegulatorCode
        {
            get;
            set;
        }

        Dictionary<string, string> CompanyData
        {
            get;
            set;
        }

        Dictionary<string, string> FilingData
        {
            get;
            set;
        }

        List<Statement> Statements
        {
            get;
        }
  
    }

    public interface IFilingParser
    {
        /// <summary>
        /// Main method responsible for parsing reports
        /// </summary>
        /// <param name="parserParams"></param>
        /// <returns></returns>
        IFilingParserResult Parse(IFilingParserParams parserParams);

        /// <summary>
        /// Type of report this parser can parse
        /// </summary>
        string ReportType
        {
            get;
        }

        IFilingParserParams CreateFilingParserParams();
    }
}
