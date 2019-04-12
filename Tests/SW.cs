using System.Diagnostics;

namespace pcysl5edgo.Collections.LINQ.Test
{
    public struct SW
    {
        private Stopwatch stopwatch;

        public static SW Create() => new SW()
        {
            stopwatch = new Stopwatch(),
        };

        public void Start() => stopwatch.Restart();

        public long Stop()
        {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}