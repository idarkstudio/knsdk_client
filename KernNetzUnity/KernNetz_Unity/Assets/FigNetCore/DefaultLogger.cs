using System;
using UnityEngine;

namespace FigNet.Core
{
    public class DefaultLogger : ILogger
    {
        public void SetUp(bool enableFileLogging, string fileName)
        {
            FN.LogMessageReceived += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string message, string exception, LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    Info(message);
                    break;
                case LogType.Warning:
                    Warning(message);
                    break;
                case LogType.Error:
                    Error(message);
                    break;
                case LogType.Exception:
                    Error(exception);
                    break;
            }
        }

        public void Error(string info)
        {
            Debug.LogError(info);
        }

        public void Error(string info, params object[] args)
        {
            Error(info);
        }

        public void Exception(Exception exception, string info)
        {
            Debug.LogException(exception);
        }

        public void Exception(Exception exception, string info, params object[] args)
        {
            Debug.LogException(exception);
        }

        public void Info(string info)
        {
            Debug.Log(info);
        }

        public void Info(string info, params object[] args)
        {
            Info(info);
        }

        public void Warning(string info)
        {
            Debug.LogWarning(info);
        }

        public void Warning(string info, params object[] args)
        {
            Warning(info);
        }
    }
}
