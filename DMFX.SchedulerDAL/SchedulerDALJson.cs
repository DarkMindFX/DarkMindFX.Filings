using DMFX.Interfaces;
using DMFX.SchedulerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace DMFX.SchedulerDAL
{
    [Export("JSON", typeof(ISchedulerDal))]
    public class SchedulerDALJson : ISchedulerDal
    {
        string _rootFolder = null;
        Dictionary<string, ISchedulerJob> _jobs = null;

        public ISchedulerDalInitResult Init(ISchedulerDalInitParams paramInit)
        {
            var result = new SchedulerDalJsonInitResult();
            _rootFolder = paramInit.Params["RootFolder"];
            _jobs = new Dictionary<string, ISchedulerJob>();

            ReadJobsData();
            result.Success = true;

            if(_jobs.Count == 0)
            {
                result.Errors.Add(new Interfaces.Error() 
                { 
                    Code = EErrorCodes.SchedulerJobsNotFound, 
                    Type = EErrorType.Warning,
                    Message = "SchedulerDAL was initialized but no jobs were read. Please verify that corresponding json-s exist and configuration is correct."
                });
            }

            return result;
        }

        public ISchedulerDalGetJobsResult GetJobs(ISchedulerDalGetJobsParams paramGetJobs)
        {
            ISchedulerDalGetJobsResult result = new SchedulerDalJsonGetJobsResult();

            result.Jobs = new List<ISchedulerJob>(_jobs.Values);

            return result;
        }        

        public ISchedulerDalSetLastRunResult SetJobLastRun(ISchedulerDalSetLastRunParams paramLastRun)
        {
            var result = new SchedulerDalJsonSetLastRunResult();

            ISchedulerJob job = null;
            if(_jobs.TryGetValue(paramLastRun.JobCode, out job))
            {
                job.LastRun = paramLastRun.LastRun;
                SaveJobData(job);

                result.Success = true;

            }
            else
            {
                result.Success = false;
                result.Errors.Add(new Error() { Code = EErrorCodes.SchedulerJobsNotFound, Type = EErrorType.Error, Message = string.Format("Jon '{0}' not found", paramLastRun.JobCode) });
            }

            return result;
        }

        public ISchedulerDalInitParams CreateInitParams()
        {
            return new SchedulerDalJsonInitParam();
        }

        public ISchedulerDalGetJobsParams CreateGetJobsParams()
        {
            return new SchedulerDalJsonGetJobsParams();
        }

        public ISchedulerDalSetLastRunParams CreateSetLastRunParams()
        {
            return new SchedulerDalJsonSetLastRunParams();
        }

        #region Support and private methods
        void ReadJobsData()
        {
            
            foreach(var f in Directory.GetFiles(_rootFolder, "*.json"))
            {
                using (var fs = new FileStream(f, FileMode.Open))
                {
                    var sr = new StreamReader(fs);
                    SchedulerJobJson job = JSonSerializer<SchedulerJobJson>.DeSerialize(sr.ReadToEnd());
                    if (job != null)
                    {
                        job.FileName = f;
                        _jobs[job.Code] = job;
                    }
                }

            }
        }

        void SaveJobData(ISchedulerJob job)
        {

        }

        


        #endregion
    }
}
