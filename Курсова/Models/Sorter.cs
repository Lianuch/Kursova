namespace Курсова.Models
{
    public static class Sorter
    {
        public static void BubbleSort(int[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - i - 1; j++)
                    if (array[j] > array[j + 1])
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
        }

        public static void ParallelBubbleSort(int[] array)
        {
            int n = array.Length;
            bool swapped = true;

            while (swapped)
            {
                swapped = false;

                Parallel.For(0, n / 2, i =>
                {
                    if (2 * i + 1 < n && array[2 * i] > array[2 * i + 1])
                    {
                        (array[2 * i], array[2 * i + 1]) = (array[2 * i + 1], array[2 * i]);
                        swapped = true;
                    }
                });

                Parallel.For(0, n / 2 - 1, i =>
                {
                    if (2 * i + 2 < n && array[2 * i + 1] > array[2 * i + 2])
                    {
                        (array[2 * i + 1], array[2 * i + 2]) = (array[2 * i + 2], array[2 * i + 1]);
                        swapped = true;
                    }
                });
            }
        }
    }

}
