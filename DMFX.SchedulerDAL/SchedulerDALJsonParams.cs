using DMFX.Interfaces;
using DMFX.SchedulerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SchedulerDAL
{
    public class SchedulerDalJsonInitParam : ISchedulerDalInitParams
    {
        public SchedulerDalJsonInitParam()
        {
            Params = new Dictionary<string, string>();
            Params.Add("RootFolder", string.Empty);
        }
        public IDictionary<string, string> Params 
        {
            get;
            set; 
        }
    }

    public class SchedulerDalJsonInitResult : ResultBase, ISchedulerDalInitResult
    {
    }

    public class SchedulerDalJsonGetJobsParams : ISchedulerDalGetJobsParams
    {
    }

    public class SchedulerJobJson : SchedulerJobBase
    {
        public string FileName
        {
            get;
            set;
        }
    }

    public class SchedulerDalJsonGetJobsResult : ResultBase, ISchedulerDalGetJobsResult
    {
        public SchedulerDalJsonGetJobsResult()
        {
            Jobs = new List<ISchedulerJob>();
        }
        public IList<ISchedulerJob> Jobs 
        {
            get;
            set;
        }
    }

    public class SchedulerDalJsonSetLastRunParams : ISchedulerDalSetLastRunParams
    {
        public string JobCode
        {
            get;
            set;
        }
        public DateTime LastRun
        {
            get;
            set;
        }
    }

    public class SchedulerDalJsonSetLastRunResult : ResultBase, ISchedulerDalSetLastRunResult
    {
    }
}
