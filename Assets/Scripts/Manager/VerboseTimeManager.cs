using UnityEngine;
using Util;

namespace Manager
{
    public class VerboseTimeManager : TimeManager
    {
        private const int MavgSize = 50;
        private readonly MovingAverageLong _mavg;

        public VerboseTimeManager()
        {
            _mavg = new MovingAverageLong(MavgSize);
        }

        public decimal AverageTickMs => _mavg.Average;

        protected override void TickPassed(long time)
        {
            _mavg.ComputeAverage(time - LastTickTime);
            Debug.Log(this);
        }

        public float GetTps()
        {
            return 1000.0f / (float)_mavg.Average;
        }

        public override string ToString()
        {
            return base.ToString() + ", Average MS per Tick: " + $"{_mavg.Average:0.00}" + ", Target Tick Time: " + 1000.0 / Tps;
        }
    }
}