﻿using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SECParser
{
    public class SECParserParams : IFilingParserParams
    {
        public SECParserParams()
        {
            ExtractNumbers = true;
            ExtractDates = true;
            ExtractStrings = false;
            FileContent = new Dictionary<string, Stream>();
        }

        public IDictionary<string, Stream> FileContent
        {
            get;
            set;
        }
        public bool ExtractNumbers
        {
            get;
            set;
        }
        public bool ExtractDates
        {
            get;
            set;
        }
        public bool ExtractStrings
        {
            get;
            set;
        }
    }

    public class FilingContex
    {
        public FilingContex(string id, DateTime startDate, DateTime endDate, DateTime instant)
        {
            ID = id;
            StartDate = startDate;
            EndDate = endDate;
            Instant = instant;
        }
        public string ID
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
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

    public class SECParserResult : ResultBase, IFilingParserResult
    {
        public SECParserResult()
        {
            CompanyData = new Dictionary<string, string>();
            FilingData = new Dictionary<string, string>();
            Statements = new List<Statement>();
            Contexts = new List<FilingContex>();
            Success = true;
            Errors = new List<Error>();
        }
        public Dictionary<string, string> CompanyData
        {
            get;
            set;
        }

  
        public Dictionary<string, string> FilingData
        {
            get;
            set;
        }

        public string RegulatorCode
        {
            get;
            set;
        }

        public List<Statement> Statements
        {
            get;
        }

     
        public List<FilingContex> Contexts
        {
            get;
            set;
        }

        public string Type
        {
            get
            {
                return FilingData.ContainsKey("DocumentType") ?  FilingData["DocumentType"] : string.Empty;
            }
        }

        public DateTime PeriodStart
        {
            get
            {
                return FilingData.ContainsKey("DocumentPeriodStartDate") ? DateTime.Parse(FilingData["DocumentPeriodStartDate"]) : DateTime.MinValue;
            }
        }

        public DateTime PeriodEnd
        {
            get
            {
                return FilingData.ContainsKey("DocumentPeriodEndDate") ? DateTime.Parse(FilingData["DocumentPeriodEndDate"]) : DateTime.MinValue;
            }
        }
    }
}
