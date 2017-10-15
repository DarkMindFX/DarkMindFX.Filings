using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    }

    public class SchedulerJob
    {
        public string Name
        {
            get;
            set;
        }

        public DateTime LastRun
        {
            get;
            set;
        }

        public TimeSpan Interval
        {
            get;
            set;
        }

        public string JobUrl
        {
            get;
            set;
        }

        public int? Hour
        {
            get;
            set;
        }

        public int? Minute
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

        private List<SchedulerJob> _jobs = new List<SchedulerJob>();

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
                _logger.Log(EErrorType.Error, "Failed to start scheduler");

                _errors.Clear();

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

       

        private void ShedulerThread(SchedulerParams schdlrParams)
        {
            try
            {
                _isRunning = true;
                _state = ESchedulerState.Init;

                PrepareJobsList();

                while (_isRunning)
                {
                    _state = ESchedulerState.RunningJobs;

                    RunJobs();

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

        private void PrepareJobsList()
        {
            // TODO: need to have a separate DB for the jobs and schedules
            // for now we just pushing importer every night
            SchedulerJob importerJob = new SchedulerJob();
            importerJob.Name = "Daily Filings Importing";
            importerJob.LastRun = DateTime.MinValue;
            importerJob.Interval = TimeSpan.FromDays(1);
            importerJob.Hour = 0;
            importerJob.JobUrl = ConfigurationManager.AppSettings["ImporterJobUrl"];

            _jobs.Add(importerJob);
        }

        private void RunJobs()
        {
            // running jobs based on EST time
            DateTime easternNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
            foreach (var job in _jobs)
            {
                if (easternNow - job.LastRun >= job.Interval && (job.Hour == null || job.Hour == easternNow.Hour) && (job.Minute == null || job.Minute == easternNow.Minute))
                {
                    try
                    {
                        job.LastRun = easternNow;
                        _logger.Log(EErrorType.Info, string.Format("Calling job '{0}': URL - {1}", job.Name, job.JobUrl));
                        WebRequest req = WebRequest.Create(job.JobUrl);
                        WebResponse response = req.GetResponse();
                        
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
}