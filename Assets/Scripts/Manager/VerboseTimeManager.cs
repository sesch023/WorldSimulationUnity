using UnityEngine;
using Utils.BaseUtils;

namespace Manager
{
    public sealed class VerboseTimeManager : TimeManager
    {
        private const int MavgSize = 50;
        private readonly MovingAverageLong _mavg;
        
        [field: SerializeField]
        public bool EnableDebug { get; set; } = false;
        
        private VerboseTimeManager()
        {
            _mavg = new MovingAverageLong(MavgSize);
        }

        public decimal AverageTickMs => _mavg.Average;

        protected override void TickPassed(long time)
        {
            _mavg.ComputeAverage(time - LastTickTime);
            if(EnableDebug)
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