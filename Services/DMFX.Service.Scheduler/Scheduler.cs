using DMFX.Interfaces;
using DMFX.SchedulerInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DMFX.Service.Scheduler
{
    public class SchedulerParams
    {
        public SchedulerParams()
        {

        }
        // delay between checks in seconds
        public int SleepDelay
        {
            get;
            set;
        }
        // list of jobs to be executed
        public IList<ISchedulerJob> Jobs
        {
            get;
            set;
        }

        public string ServicesHost
        {
            get;
            set;
        }
    }
    

    public class Scheduler
    {
        public enum ESchedulerState
        {
            Idle = 0,
            Init = 1,
            RunningJobs = 2,
            Delay = 3
        }

        private bool _isRunning = false;
        private ESchedulerState _state = ESchedulerState.Idle;
        private Task _schedulerThread = null;
        private ILogger _logger = null;
        private List<Error> _errors = new List<Error>();
        private ISchedulerDal _dal = null;

        private List<ISchedulerJob> _jobs = new List<ISchedulerJob>();

        public Scheduler()
        {
            _isRunning = false;
            _state = ESchedulerState.Idle;
            _logger = Global.Logger;            
        }

        public bool Start(SchedulerParams schdlrParams)
        {
            bool result = false;
            try
            {

                _logger.Log(EErrorType.Info, "Starting scheduler");

                _errors.Clear();

                IResult prepResult = PrepareJobsList();
                if (prepResult != null && !prepResult.Success)
                {
                    _errors.AddRange(prepResult.Errors);
                    return false;
                }

                _schedulerThread = new Task(() => ShedulerThread(schdlrParams));
                _schedulerThread.Start();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Log(EErrorType.Error, "Failed to start scheduler");
                _logger.Log(ex);

                _errors.Add(new Error() { Code = EErrorCodes.GeneralError, Type = EErrorType.Error, Message = ex.Message });
            }

            return result;
        }

        public bool Stop()
        {
            _isRunning = false;

            return true;
        }

        public ESchedulerState CurrentState
        {
            get
            {
                return _state;
            }
        }

        public bool SetJobActiveState(string code, bool IsActive, out Error error)
        {
            ISchedulerJob job = _jobs.FirstOrDefault(x => x.Code == code);
            if(job != null)
            {
                job.IsActive = IsActive;
                error = null;
                return true;
            }
            else
            {
                error = new Error()
                {
                    Code = EErrorCodes.SchedulerJobsNotFound,
                    Type = EErrorType.Error,
                    Message = string.Format("Job {0} not found", code)
                };

                return false;
            }
        }

       

        private void ShedulerThread(SchedulerParams schdlrParams)
        {
            try
            {
                _isRunning = true;
                _state = ESchedulerState.Init;                

                while (_isRunning)
                {
                    _state = ESchedulerState.RunningJobs;

                    RunJobs(schdlrParams);

                    _state = ESchedulerState.Delay;

                    Thread.Sleep(schdlrParams.SleepDelay * 1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(EErrorType.Error, "Failed to start scheduler");
                _logger.Log(ex);
            }

            _state = ESchedulerState.Idle;
        }

        private IResult PrepareJobsList()
        {
#if DEBUG
            Thread.Sleep(12000);
#endif
            if (_dal == null)
            {
                string dalType = ConfigurationManager.AppSettings["SchedulerDal"];

                var dal = Global.Container.GetExport<ISchedulerDal>(dalType);
                ISchedulerDalInitParams initParams = dal.Value.CreateInitParams();

                string dalParams = ConfigurationManager.AppSettings["SchedulerDalParams"];
                string[] kvs = dalParams.Split(new char[] { ';' });
                foreach (var kv in kvs)
                {
                    if (!string.IsNullOrEmpty(kv))
                    {
                        string[] param = kv.Split(new char[] { '=' });
                        
                        // TODO: BAD! need to fix this to compose the data automatically
                        if (dalType == "JSON" && param[0] == "RootFolder")
                        {
                            param[1] = Path.Combine(Global.Container.GetExportedValue<string>("ServiceRootFolder"), param[1]);
                        }

                        initParams.Params[param[0].Trim()] = param[1].Trim();
                    }
                }

                ISchedulerDalInitResult initResult = dal.Value.Init(initParams);
                if (!initResult.Success)
                {
                    return initResult;
                }

                _dal = dal.Value;
            }

            ISchedulerDalGetJobsParams jobsParams = _dal.CreateGetJobsParams();
            ISchedulerDalGetJobsResult jobsResult = _dal.GetJobs(jobsParams);
            if(!jobsResult.Success)
            {
                return jobsResult;
            }

            _jobs.AddRange(jobsResult.Jobs);            

            return null;

            #region deprecated
            /*
            // TODO: need to have a separate DB for the jobs and schedules
            // for now we just pushing importer every night
            SchedulerJob importerJob = new SchedulerJob();
            importerJob.Name = "Daily Filings Importing";
            importerJob.LastRun = DateTime.MinValue;
            importerJob.Interval = TimeSpan.FromDays(1);
            importerJob.Hour = 0;
            importerJob.JobUrl = ConfigurationManager.AppSettings["ImporterJobUrl"];

            // performing DB sanitization
            SchedulerJob sanitizeJob = new SchedulerJob();
            sanitizeJob.Name = "Daily Sanitization Tasks";
            sanitizeJob.LastRun = DateTime.MinValue;
            sanitizeJob.Interval = TimeSpan.FromDays(1);
            sanitizeJob.Hour = 0;
            sanitizeJob.JobUrl = ConfigurationManager.AppSettings["SanitizerJobUrl"];

            _jobs.Add(importerJob);
            _jobs.Add(sanitizeJob);
            */
            #endregion
        }

        private void RunJobs(SchedulerParams schdlrParams)
        {
            // running jobs based on EST time
            DateTime easternNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
            foreach (var job in _jobs)
            {
                if (_isRunning)
                {
                    if (job.IsActive && easternNow - job.LastRun >= job.Interval && (job.Hour == null || job.Hour == easternNow.Hour) && (job.Minute == null || job.Minute == easternNow.Minute))
                    {
                        try
                        {
                            job.LastRun = easternNow;
                            foreach (var url in job.JobUrls)
                            {
                                _logger.Log(EErrorType.Info, string.Format("Calling job '{0}': URL - {1}", job.Name, url));
                                SendRequest(schdlrParams.ServicesHost, url, job.Method, job.RequestPayload);
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.Log(EErrorType.Error, string.Format("Error in job '{0}:'", job.Name));
                            _logger.Log(ex);
                        }
                    }
                }
            }
        }

        void SendRequest(string host, string url, string method, string payload)
        {
            if(string.IsNullOrEmpty(method) || method.ToUpper().Equals("GET"))
            {
                WebRequest req = WebRequest.Create(host + url);
                WebResponse response = req.GetResponse();
            }
            else if(method.ToUpper().Equals("POST"))
            {
                using (WebClient wc = new WebClient())
                {                    
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string response = wc.UploadString(host + url, method, payload);
                }
            }


        }
    }
}