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
        private ParametersModel _parameters;
        private INumberConverter _converter;

        public GeneratorService(INumberConverter converter)
        {
            _converter = converter;
        }

        public List<AlgorithmFirstStepModel> GenerateFirstStep()
        {
            var model = new List<AlgorithmFirstStepModel>();
            _converter.SetParameters(_parameters);
            var generatedNumbers = GetRandomNumbers(_parameters.RangeFrom, _parameters.RangeTo, _parameters.PopulationSize, _parameters.Precision.IntValue);
            for (int i = 1; i <= _parameters.PopulationSize; i++)
            {
                model.Add(new AlgorithmFirstStepModel() { Id = i, RealValue = generatedNumbers[i - 1] });
            }
            foreach (AlgorithmFirstStepModel data in model)
            {
                data.IntValue = _converter.RealToIntConvert(data.RealValue);
                data.BinaryValue = _converter.IntToBinaryConvert(data.IntValue);
                data.Int2Value = _converter.BinaryToIntConvert(data.BinaryValue);
                data.Real2Value = _converter.IntToRealConvert(data.Int2Value);
            }
            return model;
        }

        public DataModel GetData(ParametersModel parameters)
        {
            _parameters = parameters;
            var model = new DataModel();
            model.FirstStepModels = GenerateFirstStep();
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
