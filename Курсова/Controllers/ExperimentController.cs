using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Курсова.Models;
using Курсова.Services;

public class ExperimentController : Controller
{
    public async Task<IActionResult> Index()
    {
        var results = await Task.Run(() => RunAllExperiments("in.txt"));
        System.IO.File.WriteAllLines("results.txt", new[] { "№,Розмір,Час послідовно,Час паралельно,Ефективність" }
            .Concat(results.Select(r => $"{r.ExperimentNumber},{r.ArraySize},{r.SequentialTimeMs:F6},{r.ParallelTimeMs:F6},{r.Efficiency:F2}")));
        AnalyzeResults(results);
        return View(results);
    }

    private List<ExperimentResult> RunAllExperiments(string path)
    {
        var inputArrays = LoadInputArrays(path);
        var results = new List<ExperimentResult>();
        int experimentNumber = 1;
        const int repetitions = 10; 

        foreach (var arr in inputArrays)
        {
            int[] copy1 = (int[])arr.Clone();
            int[] copy2 = (int[])arr.Clone();

            // Вимірювання часу для послідовного алгоритму
            double sequentialTimeMs;
            if (arr.Length <= 50)
            {
                var sw1 = Stopwatch.StartNew();
                for (int k = 0; k < repetitions; k++)
                {
                    int[] tempCopy = (int[])copy1.Clone();
                    BubbleSortService.SequentialBubbleSort(tempCopy);
                }
                sw1.Stop();
                sequentialTimeMs = (sw1.ElapsedTicks * 1000.0 / Stopwatch.Frequency) / repetitions;
            }
            else
            {
                var sw1 = Stopwatch.StartNew();
                BubbleSortService.SequentialBubbleSort(copy1);
                sw1.Stop();
                sequentialTimeMs = sw1.ElapsedTicks * 1000.0 / Stopwatch.Frequency;
            }

            // Вимірювання часу для паралельного алгоритму
            double parallelTimeMs;
            if (arr.Length <= 50)
            {
                var sw2 = Stopwatch.StartNew();
                for (int k = 0; k < repetitions; k++)
                {
                    int[] tempCopy = (int[])copy2.Clone();
                    BubbleSortService.ParallelBubbleSort(tempCopy);
                }
                sw2.Stop();
                parallelTimeMs = (sw2.ElapsedTicks * 1000.0 / Stopwatch.Frequency) / repetitions;
            }
            else
            {
                var sw2 = Stopwatch.StartNew();
                BubbleSortService.ParallelBubbleSort(copy2);
                sw2.Stop();
                parallelTimeMs = sw2.ElapsedTicks * 1000.0 / Stopwatch.Frequency;
            }

            results.Add(new ExperimentResult
            {
                ExperimentNumber = experimentNumber++,
                ArraySize = arr.Length,
                SequentialTimeMs = sequentialTimeMs,
                ParallelTimeMs = parallelTimeMs
            });
        }

        return results;
    }

    private List<int[]> LoadInputArrays(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return GenerateTestData();
        }

        var lines = System.IO.File.ReadAllLines(path);
        List<int[]> arrays = new();

        foreach (int size in new[] { 10, 20, 50, 100, 200 })
        {
            arrays.AddRange(
                lines
                    .Where(l => l.StartsWith(size + ":"))
                    .Select(l => l.Split(':')[1].Split(',').Select(int.Parse).ToArray())
            );
        }

        if (arrays.Count < 150)
        {
            return GenerateTestData();
        }

        return arrays;
    }

    private List<int[]> GenerateTestData()
    {
        var arrays = new List<int[]>();
        var rand = new Random();
        int[] sizes = { 10, 20, 50, 100, 200 };

        foreach (int size in sizes)
        {
            for (int i = 0; i < 30; i++)
            {
                int[] arr = new int[size];
                for (int j = 0; j < size; j++)
                    arr[j] = rand.Next(1, 1000);
                arrays.Add(arr);
            }
        }
        return arrays;
    }

    private void AnalyzeResults(List<ExperimentResult> results)
    {
        var grouped = results.GroupBy(r => r.ArraySize);
        var analysis = new List<string>();

        foreach (var group in grouped)
        {
            analysis.Add($"Розмір: {group.Key}, " +
                $"Середній час послідовно: {group.Average(r => r.SequentialTimeMs):F2} мс, " +
                $"Середній час паралельно: {group.Average(r => r.ParallelTimeMs):F2} мс, " +
                $"Середня ефективність: {group.Average(r => r.Efficiency):F2}");
        }

        analysis.Add("\nВисновки:");
        foreach (var group in grouped)
        {
            double avgEfficiency = group.Average(r => r.Efficiency);
            string conclusion = avgEfficiency > 1
                ? $"Для розміру масиву {group.Key} паралельний алгоритм ефективніший (ефективність: {avgEfficiency:F2})."
                : $"Для розміру масиву {group.Key} послідовний алгоритм ефективніший (ефективність: {avgEfficiency:F2}).";
            analysis.Add(conclusion);
        }

        System.IO.File.WriteAllLines("analysis.txt", analysis);
    }
}