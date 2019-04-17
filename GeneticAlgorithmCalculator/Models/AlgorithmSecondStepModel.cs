using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class AlgorithmSecondStepModel
    {
        public int Id { get; set; }

        public double RealValue { get; set; }

        public double FunctionResult { get; set; }

        public double FitnessFunctionResult { get; set; }

        public double Probability { get; set; }

        public double DistributionFunctionResult { get; set; }

        public double DrawnValue { get; set; }

        public double AfterSelectionValue { get; set; }
    }
}
