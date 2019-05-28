using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class AlgorithmThirdStepModel
    {
        public int Id { get; set; }

        public double RealValue { get; set; }

        public string BinaryValue { get; set; }

        public string ChoosenParents { get; set; }

        public int CutPoint { get; set; }

        public string Children { get; set; }

        public string GenerativeGeneration { get; set; }

        public string MutationPositions { get; set; }

        public string MutatedGeneration { get; set; }

        public double RealValue2 { get; set; }

        public double FunctionResult { get; set; }
    }
}
