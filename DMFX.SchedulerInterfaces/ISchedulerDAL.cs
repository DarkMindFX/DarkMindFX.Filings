using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SchedulerInterfaces
{

    public interface ISchedulerDalInitParams
    {
        IDictionary<string, string> Params
        {
            get;
            set;
        }
    }

    public interface ISchedulerDalInitResult : IResult
    {
    }

    public interface ISchedulerDalGetJobsParams
    {
    }

    public interface ISchedulerDalGetJobsResult : IResult 
    {
        IList<ISchedulerJob> Jobs
        {
            get;
            set;
        }
    }

    public interface ISchedulerDalSetLastRunParams
    {
        string JobCode
        {
            get;
            set;
        }

        DateTime LastRun
        {
            get;
            set;
        }
    }

    public interface ISchedulerDalSetLastRunResult : IResult
    {        
    }

    public interface ISchedulerDal
    {
        /// <summary>
        /// Use this method to initialize DAL object
        /// </summary>
        /// <param name="paramInit"></param>
        /// <returns></returns>
        ISchedulerDalInitResult Init(ISchedulerDalInitParams paramInit);

        /// <summary>
        /// Returns list jobs
        /// </summary>
        /// <param name="paramGetJobs"></param>
        /// <returns></returns>
        ISchedulerDalGetJobsResult GetJobs(ISchedulerDalGetJobsParams paramGetJobs);

        /// <summary>
        /// Marks job with given code as last run using date/time specified
        /// </summary>
        /// <param name="paramLastRun"></param>
        /// <returns></returns>
        ISchedulerDalSetLastRunResult SetJobLastRun(ISchedulerDalSetLastRunParams paramLastRun);

        ISchedulerDalInitParams CreateInitParams();

        ISchedulerDalGetJobsParams CreateGetJobsParams();

        ISchedulerDalSetLastRunParams CreateSetLastRunParams();

    }
}
