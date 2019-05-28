using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class DataModel
    {
        public List<AlgorithmFirstStepModel> FirstStepModels { get; set; }

        public List<AlgorithmSecondStepModel> SecondStepModels { get; set; }

        public List<AlgorithmThirdStepModel> ThirdStepModels { get; set; }
    }
}
