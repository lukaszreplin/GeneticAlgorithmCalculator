using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class ParametersModel
    {
        public float RangeFrom { get; set; }

        public float RangeTo { get; set; }

        public PrecisionModel Precision { get; set; }

        public int PopulationSize { get; set; }

        public int NumberOfGenerations { get; set; }
    }
}
