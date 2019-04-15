using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Services
{
    public class GeneratorService : IGeneratorService
    {
        public List<AlgorithmFirstStepModel> GenerateFirstStep(ParametersModel parameters)
        {
            var model = new List<AlgorithmFirstStepModel>();
            var generatedNumbers = GetRandomNumbers(parameters.RangeFrom, parameters.RangeTo, parameters.PopulationSize, parameters.Precision.Value);
            return model;
        }

        private List<double> GetRandomNumbers(float from, float to, int count, int precision)
        {
            var list = new List<double>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                list.Add(Math.Round(random.NextDouble() * (to - from) + from, precision));
            }
            return list;
        }
    }
}
