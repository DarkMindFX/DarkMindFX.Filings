using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMFX.SchedulerInterfaces
{
    public interface ISchedulerJob
    {
        string Code
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        DateTime LastRun
        {
            get;
            set;
        }

        TimeSpan Interval
        {
            get;
            set;
        }

        string JobUrl
        {
            get;
            set;
        }

        int? Hour
        {
            get;
            set;
        }

        int? Minute
        {
            get;
            set;
        }
    }
}