using Utils.Logging;

namespace Manager
{
    public class LoggingManager
    {
        private const string LOGPath = "logs/log.txt";
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
        
        public void Log(string message)
        {
            _logger.Log(message);
        }
    }
}