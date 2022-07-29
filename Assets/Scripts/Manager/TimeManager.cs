using System.Diagnostics;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// Manager Singleton which can manage time and speed of the simulation. Uses ticks to represent a single
    /// step in the simulation. The speed between ticks can be changed during runtime.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        /// Instance of the TimeManager.
        public static TimeManager Instance { get; private set; }
        
        /// Count Tick at this percent of the Delta Milli.
        private const double MinDeltaRatio = 1.0;
        
        /// Stopwatch of the time manager which is used to measure the time.
        private readonly Stopwatch _timer = new Stopwatch();
        
        /// Current tps of the simulation.
        private uint _tps = 10;
        
        /// Current delta between ticks.
        private long _tpsDeltaMilli;
        
        /// Current total passed time in milliseconds.
        public long PassedTime => _timer.ElapsedMilliseconds;
        
        // Total ticks since start of the simulation.
        public long PassedTicks { get; private set; }
        
        /// Time the last tick was executed.
        public long LastTickTime { get; private set; }
        
        /// Time the last update was executed.
        public long LastUpdateTime { get; private set; }
        
        /// Time between last update.
        public long LastUpdateDuration { get; private set; }
        
        /// TPS (ticks per second) of the simulation.
        public uint Tps
        {
            get => _tps;
            set
            {
                _tps = value;
                _tpsDeltaMilli = CalculateMilliDelta();
            }
        }
        
        protected TimeManager(){}
        
        /// <summary>
        /// Enables the singleton with the current instance or destroys the game object if it already exists.
        /// </summary>
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the time manager by starting the stopwatch and setting the delta between tps.
        /// </summary>
        protected void Init()
        {
            _tpsDeltaMilli = CalculateMilliDelta();
            PassedTicks = 0;
            _timer.Start();
        }

        /// <summary>
        /// Update Method of the TimeManager. It calculates the time between the last update and the current update.
        /// If the tps delta is reached it passes a tick.
        /// </summary>
        void Update()
        {
            var time = PassedTime;
            LastUpdateDuration = time - LastUpdateTime;
            LastUpdateTime = time;
            if (time >= LastTickTime + _tpsDeltaMilli)
            {
                PassedTicks++;
                TickPassed(time);
                LastTickTime = time;
            }
        }
        
        /// <summary>
        /// Calculates the delta between ticks in milliseconds.
        /// </summary>
        /// <returns>Delta between ticks in milliseconds.</returns>
        private long CalculateMilliDelta()
        {
            return (long) (1000.0 / _tps * MinDeltaRatio);
        }
        
        /// <summary>
        /// Is called every time a tick is passed.
        /// </summary>
        /// <param name="time">Current time at which the tick was passed.</param>
        protected virtual void TickPassed(long time)
        {
        }
        
        /// <summary>
        /// Calculates the ticks between the current ticks and the given ticks.
        /// </summary>
        /// <param name="since">How many ticks passed since this parameter.</param>
        /// <returns>Amount of ticks passed.</returns>
        public long TicksPassedSince(long since)
        {
            return PassedTicks - since;
        }
        
        /// <summary>
        /// Calculate the time passed since the given time.
        /// </summary>
        /// <param name="since">How much time passed since this parameter.</param>
        /// <returns>Amount of time passed.</returns>
        public long TimePassedSince(long since)
        {
            return PassedTime - since;
        }
        
        /// <summary>
        /// Simple String representation of the TimeManager with the passed ticks.
        /// </summary>
        /// <returns>Simple String representation of the TimeManager.</returns>
        public override string ToString()
        {
            return "Tick Number: " + PassedTicks;
        }
    }
}