using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EncodingFixer
{
    sealed class ProgressReporter
    {
        private readonly int batchSize;
        public int currentProgress;
        private readonly Action incrementProgress;

        public ProgressReporter(int batchSize, Action incrementProgress)
        {
            this.batchSize = batchSize;
            this.incrementProgress = incrementProgress;
            this.currentProgress = 0;
        }

        public T DoWork<T>(Func<T> f)
        {
            var result = f();
            Interlocked.Increment(ref currentProgress);
            if (Interlocked.CompareExchange(ref currentProgress, 0, batchSize) == batchSize)
                incrementProgress();
            return result;
        }
    }
}
