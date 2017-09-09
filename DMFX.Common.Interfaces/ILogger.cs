using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Interfaces
{
    public interface ILoggerParams
    {
        Dictionary<string, object> Parameters
        {
            get;
            set;
        }

    }
    public interface ILogger
    {
        ILoggerParams CreateParams();

        void Init(ILoggerParams loggerParams);

        void Log(EErrorType type, string msg);

        void Log(Exception ex);
    }
}
