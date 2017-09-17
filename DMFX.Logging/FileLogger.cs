using DMFX.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMFX.Logging
{
    public class FileLoggerParams : ILoggerParams
    {
        public FileLoggerParams()
        {
            Parameters = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Parameters
        {
            get;
            set;
        }
    }

    [Export("FileLogger", typeof(ILogger))]
    public class FileLogger : ILogger, IDisposable
    {
        private StreamWriter _sw = null;
        private object _lock = new object();
        public void Init(ILoggerParams loggerParams)
        {
            FileLoggerParams flParams = loggerParams as FileLoggerParams;
            if (flParams != null)
            {
                if (_sw != null)
                {
                    _sw.Close();
                    _sw = null;
                }
                // reading parameters
                string folder = flParams.Parameters["LogFolder"].ToString();
                string template = flParams.Parameters["NameTemplate"].ToString();

                string fileName = string.Format(template, DateTime.UtcNow.ToString("dd-MM-yyyy HH-mm-ss"));

                string fullPath = Path.Combine(folder, fileName);
                
                // preparing stream
                _sw = new StreamWriter(new FileStream(fullPath, FileMode.CreateNew));
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid parameters provided: unexpected type '{0}'", loggerParams.GetType().ToString()));
            }
        }
        public ILoggerParams CreateParams()
        {
            var result = new FileLoggerParams();
            result.Parameters.Add("LogFolder", null);
            result.Parameters.Add("NameTemplate", null);

            return result;
        }

        public void Log(EErrorType type, string msg)
        {
            if(_sw != null && _sw.BaseStream.CanWrite)
            {
                lock (_lock)
                {
                    _sw.WriteLine(string.Format("[{0}]\t[{1}]\t{2}",
                        DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        type.ToString(),
                        msg));
                    _sw.Flush();
                }
            }
        }

        public void Dispose()
        {
            if (_sw != null)
            {
                _sw.Close();
                _sw = null;
            }
        }

        public void Log(Exception ex)
        {
            if (_sw != null && _sw.BaseStream.CanWrite)
            {
                lock (_lock)
                {
                    _sw.WriteLine(string.Format("[{0}]\t[{1}]\t{2}",
                        DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        EErrorType.Error,
                        "Message:\t" + ex.Message + 
                        "\r\nInner Exception:\t" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty) +
                        "\r\nStackTrace:\t" + ex.StackTrace));
                    _sw.Flush();
                }
            }

        }
    }
}
