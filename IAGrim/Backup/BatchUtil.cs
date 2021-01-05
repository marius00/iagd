using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup {
    static class BatchUtil {

        public static List<List<T>> ToBatches<T>(IList<T> items) {
            List<T> currentBatch = new List<T>();
            List<List<T>> batches = new List<List<T>>();

            // Max 100 items per batch, no mix of partitions in a batch.
            foreach (var item in items) {
                if (currentBatch.Count > 99) {
                    batches.Add(currentBatch);
                    currentBatch = new List<T>();
                }

                currentBatch.Add(item);
            }

            if (currentBatch.Count > 0) {
                batches.Add(currentBatch);
            }

            return batches;
        }

    }
}
