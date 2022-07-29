using UnityEngine;
using Utils.BaseUtils;

namespace Manager
{
    /// <summary>
    /// Verbose Version of the Time Manager. Calculate average tick time and if in debug mode, prints the average tick time.
    /// </summary>
    public sealed class VerboseTimeManager : TimeManager
    {
        /// Size of the moving average window.
        private const int MavgSize = 50;
        /// Calculates average tick time.
        private readonly MovingAverageLong _mavg;
         
        /// Enables or disables the logging of the average tick time.
        [field: SerializeField]
        public bool EnableDebug { get; set; } = false;
        
        private VerboseTimeManager()
        {
            _mavg = new MovingAverageLong(MavgSize);
        }
        
        /// Returns the average tick time.
        public decimal AverageTickMs => _mavg.Average;
        
        /// <summary>
        /// If a tick is passed, the average tick time is updated and logged if in debug mode.
        /// </summary>
        /// <param name="time">Time at which the tick was passed.</param>
        protected override void TickPassed(long time)
        {
            _mavg.ComputeAverage(time - LastTickTime);
            if(EnableDebug)
                Debug.Log(this);
        }

        /// <summary>
        /// Returns the average tps.
        /// </summary>
        /// <returns>Current average tps.</returns>
        public float GetTps()
        {
            return 1000.0f / (float)_mavg.Average;
        }
        
        /// <summary>
        /// Returns a string representation of the VerboseTimeManager with the average tick time.
        /// </summary>
        /// <returns>A string representation of the VerboseTimeManager with the average tick time.</returns>
        public override string ToString()
        {
            return base.ToString() + ", Average MS per Tick: " + $"{_mavg.Average:0.00}" + ", Target Tick Time: " + 1000.0 / Tps;
        }
    }
}