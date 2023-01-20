using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Utils.Logging
{
    public delegate void LogString(string message, LogLevel level, StreamWriter levelWriter, StreamWriter fullWriter);
    
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
        private const string FileTimeFormat = "yy-MM-dd-hh-mm-ss";
        private readonly LogString _logString;
        private readonly Dictionary<LogLevel, string> _writerPaths;
        private readonly string _fullLogWriterPath;
        
        public SimpleLogger(string targetPath="Logs/", bool useTimeSignature=false, LogString logString=null, bool clearLogsAfterRun=true)
        {
            if(targetPath == null)
                throw new NullReferenceException($"NullReferenceException: {GetType().Name}. Given Logging Path is null!");
            
            var newPath = Path.GetDirectoryName(targetPath);
            if(newPath == null || new FileInfo(targetPath).Extension != "")
                throw new ArgumentException($"ArgumentException: {GetType().Name}. Given Logging Path is illegal!");
            targetPath = newPath;

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            else if (clearLogsAfterRun)
            {
                foreach(FileInfo file in new DirectoryInfo(targetPath).GetFiles())
                {
                    if(file.Extension == ".log" && (LogLevelInfo.Names.Any(file.Name.Contains) || file.Name.Contains("Full")))
                        file.Delete();
                }
            
            }

            _writerPaths = new Dictionary<LogLevel, string>();
            var subpath = (useTimeSignature) ? $"Full-{DateTime.Now.ToString(FileTimeFormat)}.log" : "Full.log";
            _fullLogWriterPath = Path.Join(targetPath,subpath);
            foreach (var key in Enum.GetValues(typeof(LogLevel)))
            {
                var logLevel = (LogLevel) key;
                subpath = (useTimeSignature)
                    ? $"{logLevel}-{DateTime.Now.ToString(FileTimeFormat)}.log"
                    : $"{logLevel}.log";
                var logPath = Path.Join(targetPath, subpath);
                _writerPaths.Add(logLevel, logPath);
                Debug.Log(logLevel);
                using (StreamWriter fulLWriter = new StreamWriter(logPath, true))
                {
                    fulLWriter.WriteLine($"Start Logging: {DateTime.Now.ToShortTimeString()}");
                }
            }
            
            if (logString == null)
            {
                logString = (msg, level, writer, fulLWriter) =>
                {
                    msg = $"[{level}] {DateTime.Now.ToLongTimeString()}: {msg}";
                    
                    writer.WriteLine(msg);
                    fulLWriter.WriteLine(msg);
                    
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
            _logString = logString;
        }
        
        public void Log(string message, LogLevel level=LogLevel.Debug)
        {
            using (StreamWriter currentWriter = new StreamWriter(_writerPaths[level], true))
            {
                using (StreamWriter fulLWriter = new StreamWriter(_fullLogWriterPath, true))
                {
                    _logString(message, level, currentWriter, fulLWriter);
                }
            }
        }
        
        public void Log(object obj, LogLevel level=LogLevel.Debug)
        {
            Log(obj.ToString(), level);
        }
    }
}