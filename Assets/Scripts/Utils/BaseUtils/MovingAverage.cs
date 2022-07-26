using System.Collections.Generic;

namespace Utils
{
    public class MovingAverageLong
    {
        private readonly Queue<long> _samples = new Queue<long>();
        private readonly int _windowSize;
        private decimal _sampleAccumulator;

        public MovingAverageLong(int windowSize = 16)
        {
            _windowSize = windowSize;
        }

        public decimal Average { get; private set; }

        /// <summary>
        ///     Computes a new windowed average each time a new sample arrives
        /// </summary>
        /// <param name="newSample"></param>
        public decimal ComputeAverage(long newSample)
        {
            _sampleAccumulator += newSample;
            _samples.Enqueue(newSample);

            if (_samples.Count > _windowSize) _sampleAccumulator -= _samples.Dequeue();

            Average = _sampleAccumulator / _samples.Count;
            return Average;
        }
    }
}