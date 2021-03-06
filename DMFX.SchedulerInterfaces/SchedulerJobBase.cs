﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.SchedulerInterfaces
{
    public class SchedulerJobBase : ISchedulerJob
    {
        public SchedulerJobBase()
        {
            IsActive = true;
            JobUrls = new List<string>();
        }

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

        public IList<string> JobUrls
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

        public bool IsActive
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string RequestPayload
        {
            get;
            set;
        }
    }
}
