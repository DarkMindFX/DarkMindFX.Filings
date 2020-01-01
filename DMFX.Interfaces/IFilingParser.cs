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
        IDictionary<string,Stream> FileContent
        {
            get;
            set;
        }

        bool ExtractNumbers
        {
            get;set;
        }

        bool ExtractDates
        {
            get; set;
        }

        bool ExtractStrings
        {
            get; set;
        }
    }

    public class StatementRecord
    {
        public StatementRecord(string title, object value, string unit, DateTime periodStart, DateTime periodEnd, DateTime instant, string sourceFactId, string factId = null)
        {
            Title = title;
            Value = value;
            Unit = unit;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            Instant = instant;
            SourceFactId = sourceFactId;
            FactId = factId;
        }
        public string Title
        {
            get;
            set;
        }

        public object Value
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

        public string SourceFactId
        {
            get;
            set;
        }

        public string FactId
        {
            get;
            set;
        }
        
        public override bool Equals(object obj)
        {
            StatementRecord other = obj as StatementRecord;
            if (other != null)
            {
                return (!string.IsNullOrEmpty(SourceFactId) && SourceFactId.Equals(other.SourceFactId)) || 
                    (Title.Equals(other.Title) && PeriodStart.Equals(other.PeriodStart) && PeriodEnd.Equals(other.PeriodEnd) && (FactId != null && FactId.Equals(other.FactId)));
            }
            else
            {
                return false;
            }
        }       
        

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(SourceFactId))
            {
                return SourceFactId.GetHashCode();
            }
            else
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 117;

                    hash = hash * 123 + Title.GetHashCode();
                    hash = hash * 123 + PeriodStart.GetHashCode();
                    hash = hash * 123 + PeriodEnd.GetHashCode();

                    return hash;
                }
            }
        }

        public override string ToString()
        {
            return Title + ": " + Value;
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

        public override string ToString()
        {
            return Title + " (" + Records.Count + " records)";
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

        string Type
        {
            get;
        }

        DateTime PeriodStart
        {
            get;
        }

        DateTime PeriodEnd
        {
            get;
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
