using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DMFX.Service.Sourcing
{
    public class Importer
    {
        public enum EImportState
        {
            Idle = 0,
            Init = 1,
            ValidatingSource = 2,
            ReadingFilingInfo = 3,
            ImportSources = 4,
            Parsing = 5,
            Saving = 6
        }

        Task _importTask = null;
        DateTime _lastRun = DateTime.MinValue;
        EImportState _currentState = EImportState.Idle;
        CompositionContainer _compContainer = null;
        List<Error> _errorsLog = new List<Error>();
        Interfaces.DAL.IDal _dal = null;

        public Importer(CompositionContainer compContainer)
        {
            _compContainer = compContainer;
            Errors = new List<Error>();

            InitDAL();
        }

        #region Properties

        public EImportState CurrentState
        {
            get
            {
                return _currentState;
            }
            private set
            {
                _currentState = value;
            }
        }

        public DateTime LastRun
        {
            get
            {
                return _lastRun;
            }
        }

        public List<Error> Errors
        {
            get
            {
                return _errorsLog;
            }
            private set
            {
                _errorsLog = value;
            }
        }

        public CompositionContainer CompositionContainer
        {
            get
            {
                return _compContainer;
            }
            set
            {
                _compContainer = value;
            }
        }

        #endregion

        public bool StartImport()
        {
            if (CurrentState == EImportState.Idle)
            {
                _importTask = new Task(ImportThread);
                _importTask.Start();
                _lastRun = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ImportThread()
        {
            if (_compContainer != null)
            {
                // validating if there are anything need to be imported
                CurrentState = EImportState.Init;

                // TODO: this list need to be extracted from DB
                string[] regulators = { "SEC" };

                foreach (var regulator in regulators)
                {

                    Lazy<ISource> source = null;
                    try
                    {
                        source = _compContainer.GetExport<ISource>(regulator);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", regulator) });
                    }
                    if (source != null && source.Value != null)
                    {
                        List<string> companies = GetCompaniesToUpdate(regulator);
                        foreach (string companyCode in companies)
                        {
                            try
                            {
                                ImportPipeline(regulator, companyCode, source.Value);
                            }
                            catch (Exception ex)
                            {
                                Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Error, Message = string.Format("Import failed: Regulator '{0}', Company '{1}', Error '{2}'", regulator, companyCode, ex.Message) });
                            }
                        }
                    }
                    else
                    {
                        Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Warning, Message = string.Format("ISource importer not registered for '{0}' - skipped", regulator) });
                    }
                }
            }
            else
            {
                Errors.Add(new Error() { Code = EErrorCodes.ImporterError, Type = EErrorType.Error, Message = "Import failed. Composition comtainer is NULL" });
            }
            CurrentState = EImportState.Idle;

        }

        #region Support method
        private void InitDAL()
        {
            Lazy<Interfaces.DAL.IDal> dal = _compContainer.GetExport<Interfaces.DAL.IDal>();
            Interfaces.DAL.IDalParams dalParams = dal.Value.CreateDalParams();
            dalParams.Parameters.Add("ConnectionString", ConfigurationManager.AppSettings["ConnectionString"]);

            dal.Value.Init(dalParams);

            _dal = dal.Value;
        }
        private List<string> GetCompaniesToUpdate(string regulatorCode)
        {
            // TODO: right now returning only AAPL - need to add access to DB here
            List<string> result = new List<string>();
            result.Add("AAPL");

            return result;

        }

        private void ImportPipeline(string regulatorCode, string companyCode, ISource source)
        {
            // 1. validating if new reports are available
            CurrentState = EImportState.ValidatingSource;

            ISourceValidateParams vldParams = source.CreateValidateParams();
            vldParams.RegulatorCode = regulatorCode;
            vldParams.CompanyCode = companyCode;

            ISourceValidateResult vldResult = source.ValidateSourceDelta(vldParams);

            // 2. if any - importing from source
            if (vldResult.Success && vldResult.Delta != null && vldResult.Delta.Count > 0)
            {
                
                CurrentState = EImportState.ReadingFilingInfo;
                ISourceSubmissionsInfoParams subInfoParams = source.CreateSourceSubmissionsInfoParams();
                subInfoParams.CompanyCode = companyCode;
                subInfoParams.RegulatorCode = regulatorCode;

                subInfoParams.Items.AddRange(vldResult.Delta);

                ISourceSubmissionsInfoResult subInfoResult = source.GetSubmissionsInfo(subInfoParams);

                if (subInfoResult != null && subInfoResult.Submissions != null && subInfoResult.Submissions.Count > 0)
                {
                    // 3. importing sources
                    CurrentState = EImportState.ImportSources;

                    ISourceExtractParams extrParams = source.CreateExtractParams();
                    extrParams.CompanyCode = companyCode;
                    extrParams.RegulatorCode = regulatorCode;

                    foreach (var submissionInfo in subInfoResult.Submissions)
                    {
                        extrParams.Items.Clear();

                        ISourceItemInfo srcItemInfo = source.CreateSourceItemInfo();
                        srcItemInfo.Name = submissionInfo.Name;
                        extrParams.Items.Add(srcItemInfo);

                        ISourceExtractResult extrResult = source.ExtractReports(extrParams);
                        if (extrResult != null && extrResult.Success && extrParams.Items.Count > 0)
                        {
                            ISourceItem filingContent = extrResult.Items.FirstOrDefault(i => i.Name == submissionInfo.Report);
                            if (filingContent != null)
                            {
                                MemoryStream ms = new MemoryStream(filingContent.Content.ToArray());

                                // 4. Parsing
                                CurrentState = EImportState.Parsing;

                                var parsersRepository = _compContainer.GetExport<IParsersRepository>(regulatorCode);
                                if (parsersRepository != null)
                                {
                                    IFilingParser parser = parsersRepository.Value.GetParser(companyCode, submissionInfo.Type);
                                    if (parser != null)
                                    {
                                        IFilingParserParams parserParams = parser.CreateFilingParserParams();
                                        parserParams.FileContent = ms;
                                        IFilingParserResult parserResults = parser.Parse(parserParams);

                                        if (parserResults != null && parserResults.Success)
                                        {
                                            CurrentState = EImportState.Saving;
                                            StoreFiling(regulatorCode, companyCode, submissionInfo, parserResults);
                                        }
                                    }
                                   
                                }

                            }
                        }
                    }
                }
            }
            
        }

        private bool StoreFiling(string regulatorCode, string companyCode, ISourceSubmissionInfo submissionInfo, IFilingParserResult parserResults)
        {


            // preparing params
            Interfaces.DAL.InsertFilingDetailsParams insertParams = new Interfaces.DAL.InsertFilingDetailsParams();

            // preparing metadata
            insertParams.Metadata.Add( new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "RegulatorCode", Value = regulatorCode, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "CompanyCode", Value = companyCode, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "FilingName", Value = submissionInfo.Name, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "Source", Value = submissionInfo.Report, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "FilingType", Value = submissionInfo.Type, Type = "String" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "Submitted", Value = submissionInfo.Submitted.ToString(), Type = "DateTime" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "PeriodStart", Value = parserResults.PeriodStart.ToString(), Type = "DateTime" });
            insertParams.Metadata.Add(new Interfaces.DAL.InsertFilingDetailsParams.FilingMetadaRecord() { Name = "PeriodEnd", Value = parserResults.PeriodEnd.ToString(), Type = "DateTime" });

            // preparing filing data records
            foreach (var statement in parserResults.Statements)
            {
                foreach (var r in statement.Records)
                {

                    insertParams.Data.Add(new Interfaces.DAL.FilingRecord()
                    {
                        Code = r.Title,
                        Instant = r.Instant,
                        PeriodEnd = r.PeriodEnd,
                        PeriodStart = r.PeriodStart,
                        Unit = r.Unit,
                        Value = r.Value,
                        SourceFactId = r.SourceFactId

                    });
                }
            }

            _dal.InsertFilingDetails(insertParams);

            return true;


        }
        #endregion
    }
}