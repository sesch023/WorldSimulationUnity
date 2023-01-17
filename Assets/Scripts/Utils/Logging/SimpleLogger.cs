using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.iOS;

namespace Utils.Logging
{
    public delegate void LoggingMethod(string message, LogLevel level=LogLevel.Debug);
    
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }
    
    public static class LogLevelInfo
    {
        public static readonly string[] Names = {"Debug", "Info", "Warning", "Error", "Fatal"};
        public static string GetName(LogLevel level)
        {
            return Names[(int)level];
        }
    }
    
    public class SimpleLogger
    {
        private readonly LoggingMethod _method;
        private readonly Dictionary<LogLevel, StreamWriter> _writers;

        public SimpleLogger(string targetPath="logs/", LoggingMethod loggingMethod=null)
        {
            if(targetPath == null)
                throw new NullReferenceException($"NullReferenceException: {GetType().Name}. Given Logging Path is null!");
            
            var path = Path.GetDirectoryName(targetPath);
            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            _writers = new Dictionary<LogLevel, StreamWriter>();
            foreach (var key in Enum.GetValues(typeof(LogLevel)))
            {
                var logLevel = (LogLevel) key;
                var logPath = Path.Combine(targetPath, $"{logLevel}.log");
                _writers.Add(logLevel, new StreamWriter(logPath, true));
            }
            
            if (loggingMethod == null)
            {
                loggingMethod = (msg, level) =>
                {
                    msg = $"[{level}] {DateTime.Now.ToLongTimeString()}: {msg}";
                    _writers[level].WriteLine(msg);
                    #if UNITY_STANDALONE
                         switch(level)
                         {
                             case LogLevel.Debug:
                                 Debug.Log(msg);
                                 break;
                             case LogLevel.Info:
                                 Debug.Log(msg);
                                 break;
                             case LogLevel.Warning:
                                 Debug.LogWarning(msg);
                                 break;
                             case LogLevel.Error:
                                 Debug.LogError(msg);
                                 break;
                             case LogLevel.Fatal:
                                 Debug.LogError(msg);
                                 break;
                         }               
                    #else
                        Console.WriteLine(msg);
                    #endif
                };
                
            }
            else
            {
                _method = loggingMethod;
            }
            loggingMethod("Logging to " + targetPath, LogLevel.Info);
        }
        
        ~SimpleLogger()
        {
            foreach(var writer in _writers)
            {
                writer.Value.Close();
            }
        }
        
        public void Log(string message, LogLevel level=LogLevel.Debug)
        {
            _method(message, level);
        }
    }
}