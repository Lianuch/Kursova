namespace Курсова.Services
{
    public class BubbleSortService
    {
        public static void SequentialBubbleSort(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
                for (int j = 0; j < array.Length - i - 1; j++)
                    if (array[j] > array[j + 1])
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
        }

        public static void ParallelBubbleSort(int[] array)
        {
            if (array.Length < 50)
            {
                SequentialBubbleSort(array);
                return;
            }

            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int swappedCount = 0;
                int start = i % 2;
                Parallel.For(start, n - 1, j =>
                {
                    if (j + 1 < n && array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        lock (array)
                        {
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                        }
                        Interlocked.Increment(ref swappedCount);
                    }
                });
                if (swappedCount == 0) break;
            }
        }
    }
}