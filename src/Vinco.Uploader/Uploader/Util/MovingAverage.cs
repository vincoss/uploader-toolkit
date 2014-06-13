

namespace Vinco.Uploader.Util
{
    public sealed class MovingAverage
    {
        private long _sum;
        private int _itemCount;
        private int _nextValueIndex;

        private readonly int _windowSize;
        private readonly long[] _values;

        public MovingAverage(int windowSize)
        {
            if (windowSize < 1)
            {
                windowSize = 5;
            }
            _windowSize = windowSize;
            _values = new long[_windowSize];
            Reset();
        }

        public long NextValue(long nextValue)
        {
            _sum += nextValue;

            if (_itemCount < _windowSize)
            {
                _itemCount++;
            }
            else
            {
                // Remove oldest value
                _sum -= _values[0];
            }

            _values[_nextValueIndex] = nextValue;

            _nextValueIndex++;
            if (_nextValueIndex == _windowSize)
            {
                _nextValueIndex = 0;
            }

            return this.Sum;
        }

        public void Reset()
        {
            _itemCount = 0;
            _nextValueIndex = 0;
            _sum = 0;
        }

        public override string ToString()
        {
            return Sum.ToString();
        }

        public long Sum
        {
            get
            {
                if (_itemCount == 0)
                {
                    return 0;
                }
                return _sum / _itemCount;
            }
        }
    }
}
