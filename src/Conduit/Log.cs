using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Conduit
{    
    public enum LogLevel
    {
        None,
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    public class Log : ILog
    {
        public bool IsDebugEnabled { get; private set;}
        public bool IsErrorEnabled { get; private set;}
        public bool IsFatalEnabled { get; private set;}
        public bool IsInfoEnabled { get; private set;}
        public bool IsWarnEnabled { get; private set;}

        public Log()
        {
            if (Settings.Default.LogEnabled)
            {
                if (Settings.Default.LogLevel == LogLevel.Fatal)
                {
                    this.IsFatalEnabled = true;
                }
                if (Settings.Default.LogLevel == LogLevel.Error)
                {
                    this.IsFatalEnabled = true;
                    this.IsErrorEnabled = true;
                }
                if (Settings.Default.LogLevel == LogLevel.Warn)
                {
                    this.IsFatalEnabled = true;
                    this.IsErrorEnabled = true;
                    this.IsWarnEnabled = true;
                }
                if (Settings.Default.LogLevel == LogLevel.Info)
                {
                    this.IsFatalEnabled = true;
                    this.IsErrorEnabled = true;
                    this.IsWarnEnabled = true;
                    this.IsInfoEnabled = true;
                }
                if (Settings.Default.LogLevel == LogLevel.Debug)
                {
                    this.IsFatalEnabled = true;
                    this.IsErrorEnabled = true;
                    this.IsWarnEnabled = true;
                    this.IsInfoEnabled = true;
                    this.IsDebugEnabled = true;
                }

                Trace.AutoFlush = true;
                TextWriterTraceListener t;
                System.Diagnostics.Debug.Listeners.Add(new TextWriterTraceListener(Console.Out, "TEST"));
                System.Diagnostics.Debug.Listeners.Add(new TextWriterTraceListener(File.Create(Settings.Default.LogFile)));
            }
        }

        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                WriteLog(LogLevel.Debug, message.ToString());
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                WriteLog(LogLevel.Debug, message.ToString(), exception);
            }
        }

        public void Error(object message)
        {
            if (IsErrorEnabled)
            {
                WriteLog(LogLevel.Error, message.ToString());
            }
        }

        public void Error(object message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                WriteLog(LogLevel.Error, message.ToString(), exception);
            }
        }

        public void Fatal(object message)
        {
            if (IsFatalEnabled)
            {
                WriteLog(LogLevel.Fatal, message.ToString());
            }
        }

        public void Fatal(object message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                WriteLog(LogLevel.Fatal, message.ToString(), exception);
            }
        }

        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                WriteLog(LogLevel.Info, message.ToString());
            }
        }

        public void Info(object message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                Trace.TraceInformation("INFO: " + message.ToString() + exception.ToString());
                WriteLog(LogLevel.Info, message.ToString(), exception);
            }
        }

        public void Warn(object message)
        {
            if (IsWarnEnabled)
            {
                WriteLog(LogLevel.Warn, message.ToString());
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                WriteLog(LogLevel.Warn, message.ToString(), exception);
            }
        }

        private void WriteLog(LogLevel level, object message)
        {
            string prefix = level.ToString().ToUpperInvariant();
            string line = string.Format("{0}  {1}\t{2}",
                prefix,
                DateTime.Now.ToString(),
                message.ToString()
                );

            WriteTrace(level, line);
        }

        private void WriteLog(LogLevel level, object message, Exception exception)
        {
            string prefix = level.ToString().ToUpperInvariant();
            string line = string.Format("{0}  {1}\t{2} {3}",
                prefix,
                DateTime.Now.ToString(),
                message.ToString(),
                exception.ToString()
                );

            WriteTrace(level, line);
        }

        private void WriteTrace(LogLevel level, string line)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    Trace.TraceError(line);
                    break;
                case LogLevel.Error:
                    Trace.TraceError(line);
                    break;
                case LogLevel.Warn:
                    Trace.TraceWarning(line);
                    break;
                case LogLevel.Info:
                    Trace.TraceInformation(line);
                    break;
                case LogLevel.Debug:
                    Trace.TraceInformation(line);
                    break;
            }
            Trace.Flush();
        }
    }
}
