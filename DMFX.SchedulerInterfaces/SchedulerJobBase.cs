using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SchedulerInterfaces
{
    public class SchedulerJobBase : ISchedulerJob
    {
        public string Code
        {
            get;
            set;
        }
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
}
