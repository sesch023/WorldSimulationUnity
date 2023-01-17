using Utils.Logging;

namespace Manager
{
    public class LoggingManager
    {
        private const string LOGPath = "Logs/";
        private static readonly LoggingManager Instance;
        
        private readonly SimpleLogger _logger;

        static LoggingManager()
        {
            Instance = new LoggingManager();
        }
        
        private LoggingManager()
        {
            _logger = new SimpleLogger(LOGPath);
        }
        
        public static LoggingManager GetInstance()
        {
            return Instance;
        }
        
        public void Log(object message, LogLevel level=LogLevel.Debug)
        {
            _logger.Log(message, level);
        }
        
        public void LogInfo(object message)
        {
            _logger.Log(message, LogLevel.Info);
        }
        
        public void LogWarning(object message)
        {
            _logger.Log(message, LogLevel.Warning);
        }
        
        public void LogError(object message)
        {
            _logger.Log(message, LogLevel.Error);
        }
        
        public void LogDebug(object message)
        {
            _logger.Log(message, LogLevel.Debug);
        }
        
        public void LogFatal(object message)
        {
            _logger.Log(message, LogLevel.Fatal);
        }
    }
}