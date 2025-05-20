namespace Курсова.Models
{
    public class ExperimentResult
    {
        public int ExperimentNumber { get; set; }
        public int ArraySize { get; set; }
        public double SequentialTimeMs { get; set; }
        public double ParallelTimeMs { get; set; }
        public double Efficiency => ParallelTimeMs == 0 ? 0 : (double)SequentialTimeMs / ParallelTimeMs;
    }

}
