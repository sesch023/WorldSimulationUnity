using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Manager
{
    public class TimeManager : MonoBehaviour
    {
        // Count Tick at this percent of the Delta Milli.
        private const double MinDeltaRatio = 1.0;

        private readonly Stopwatch _timer = new Stopwatch();

        private uint _tps = 10;

        private long _tpsDeltaMilli;
        public static TimeManager Instance { get; private set; }
        public long PassedTime => _timer.ElapsedMilliseconds;
        public long PassedTicks { get; private set; }
        public long LastTickTime { get; private set; }
        public long LastUpdateTime { get; private set; }
        public long LastUpdateDuration { get; private set; }

        public uint Tps
        {
            get => _tps;
            set
            {
                _tps = value;
                _tpsDeltaMilli = CalculateMilliDelta();
            }
        }
        
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
        
        protected void Init()
        {
            _tpsDeltaMilli = CalculateMilliDelta();
            PassedTicks = 0;
            _timer.Start();
        }

        void Update()
        {
            var time = PassedTime;
            LastUpdateDuration = time - LastUpdateTime;
            LastUpdateTime = time;
            Debug.Log("Hello");
            if (time >= LastTickTime + _tpsDeltaMilli)
            {
                PassedTicks++;
                TickPassed(time);
                LastTickTime = time;
                Debug.Log("Hello");
            }
        }

        private long CalculateMilliDelta()
        {
            return (long) (1000.0 / _tps * MinDeltaRatio);
        }

        protected virtual void TickPassed(long time)
        {
        }

        public long TicksPassedSince(long since)
        {
            return PassedTicks - since;
        }

        public long TimePassedSince(long since)
        {
            return PassedTime - since;
        }

        public override string ToString()
        {
            return "Tick Number: " + PassedTicks;
        }
    }
}