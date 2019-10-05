using DMFX.Interfaces;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading;
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
    public class FileLogger : ILogger
    {
        private object _lock = new object();
        private List<string> _msgBuffer = new List<string>();
        private string _filePath = null;
        private int _bufferSize = 5;
        private Task _flushTask = null;
        private bool _isFlushThreadRun = false;

        public void Init(ILoggerParams loggerParams)
        {
            FileLoggerParams flParams = loggerParams as FileLoggerParams;
            if (flParams != null)
            {
                // reading parameters
                string folder = flParams.Parameters["LogFolder"].ToString();
                string template = flParams.Parameters["NameTemplate"].ToString();
                _bufferSize = flParams.Parameters.ContainsKey("BufferSize") ? Int32.Parse(flParams.Parameters["BufferSize"].ToString()) : 100;

                string fileName = string.Format(template, DateTime.UtcNow.ToString("dd-MM-yyyy HH-mm-ss"));

                _filePath = Path.Combine(folder, fileName);

                _flushTask = new Task(FlushThread);
                _isFlushThreadRun = true;
                _flushTask.Start();

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
            string message = string.Format("[{0}] [Trd:{1}]\t[{2}]\t{3}",
                        DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        Thread.CurrentThread.ManagedThreadId,
                        type.ToString(),
                        msg);

            lock (_lock)
            {
                _msgBuffer.Add(message);
            }

            FlushBuffer();


        }

        public void Dispose()
        {
            _isFlushThreadRun = false;
            FlushBuffer(true);
        }

        public void Log(Exception ex)
        {
            string message = string.Empty;

            if (ex is WebServiceException)
            {
                message = string.Format("[{0}] [Trd:{1}]\t[{2}]\t{3}",
                        DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        Thread.CurrentThread.ManagedThreadId,
                        EErrorType.Error,
                        "Message:\t" + ex.Message +
                        "\r\nBody:\r\n"  + (ex as WebServiceException).ResponseBody +
                        "\r\nStackTrace:\t" + ex.StackTrace);
            }
            else
            {

                message = string.Format("[{0}] [Trd:{1}]\t[{2}]\t{3}",
                        DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        Thread.CurrentThread.ManagedThreadId,
                        EErrorType.Error,
                        "Message:\t" + ex.Message +
                        "\r\nInner Exception:\t" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty) +
                        "\r\nStackTrace:\t" + ex.StackTrace);
            }


            lock (_lock)
            {
                _msgBuffer.Add(message);
            }

            FlushBuffer();

        }

        private void FlushThread()
        {
            while (_isFlushThreadRun)
            {
                try
                {
                    Thread.Sleep(30000);
                    FlushBuffer(true);
                }
                catch
                {
                }
            }

        }

        private void FlushBuffer(bool force = false)
        {
            if (_msgBuffer.Count >= _bufferSize || force)
            {
                lock (_lock)
                {
                    using (StreamWriter sw = CreateWriter())
                    {
                        // preparing string to flush
                        StringBuilder sb = new StringBuilder();
                        foreach (var s in _msgBuffer)
                        {
                            sb.Append(s + "\r\n");
                        }
                        // flushing
                        sw.WriteLine(sb.ToString().Trim());
                        _msgBuffer.Clear();
                        sw.Flush();
                        sw.Close();
                    }
                }

            }
        }

        private StreamWriter CreateWriter()
        {
            FileStream fs = null;
            if (!File.Exists(_filePath))
            {
                lock (_lock)
                {
                    if (!File.Exists(_filePath))
                    {
                        fs = File.Create(_filePath);                        
                    }
                }
            }
            else
            {
                fs = new FileStream(_filePath, FileMode.Append);
            }
            StreamWriter sw = new StreamWriter(fs);
            return sw;
        }
    }
}
