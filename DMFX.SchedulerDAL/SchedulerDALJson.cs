using DMFX.Interfaces;
using DMFX.SchedulerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SchedulerDAL
{
    [Export("JSON", typeof(ISchedulerDAL))]
    public class SchedulerDALJson : ISchedulerDAL
    {
        string _rootFolder = null;
        Dictionary<string, ISchedulerJob> _jobs = null;

        public ISchedulerDalInitResult Init(ISchedulerDalInitParams paramInit)
        {
            var result = new SchedulerDalJsonInitResult();
            _rootFolder = paramInit.Params["RootFolder"];
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
            throw new NotImplementedException();
        }        

        public ISchedulerDalSetLastRunResult SetJobLastRun(ISchedulerDalSetLastRunParams paramLastRun)
        {
            throw new NotImplementedException();
        }

        #region Support and private methods
        void ReadJobsData()
        {

        }
        #endregion
    }
}
