using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Models;
using GeneticAlgorithmCalculator.Models.Enums;
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
        private DataModel _model;

        public GeneratorService(INumberConverter converter)
        {
            _converter = converter;
            _model = new DataModel();
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
                data.FunctionResult = (data.RealValue % 1) * (Math.Cos(20 * Math.PI * data.RealValue) - Math.Sin(data.RealValue));
            }
            return model;
        }

        public List<AlgorithmSecondStepModel> GenerateSecondStep()
        {
            var model = new List<AlgorithmSecondStepModel>();
            for (int i = 1; i <= _parameters.PopulationSize; i++)
            {
                model.Add(new AlgorithmSecondStepModel()
                {
                    Id = i,
                    RealValue = _model.FirstStepModels[i - 1].RealValue,
                    FunctionResult = _model.FirstStepModels[i - 1].FunctionResult,
                    FitnessFunctionResult = (_model.FirstStepModels[i - 1].FunctionResult - GetMinValue()) + _parameters.Precision.IntValue,
                });
            }
            var fitnessSum = GetSumOfFitness(model);
            for (int i = 1; i <= _parameters.PopulationSize; i++)
            {
                model[i - 1].Probability = model[i - 1].FitnessFunctionResult / fitnessSum;
            }
            for (int i = 1; i <= _parameters.PopulationSize; i++)
            {
                if (i == 1)
                {
                    model[i - 1].DistributionFunctionResult = model[i - 1].Probability;
                    continue;
                }
                model[i - 1].DistributionFunctionResult = model[i - 1].Probability + model[i-2].DistributionFunctionResult;
            }
            Random random = new Random();
            foreach (AlgorithmSecondStepModel item in model)
            {
                var drawnValue = random.NextDouble();
                item.DrawnValue = drawnValue;
                item.AfterSelectionValue = SelectValue(model, drawnValue);
            }
            return model;
        }

        public DataModel GetData(ParametersModel parameters)
        {
            _parameters = parameters;
            _model.FirstStepModels = GenerateFirstStep();
            _model.SecondStepModels = GenerateSecondStep();
            return _model;
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

        private double GetMaxValue()
        {
            double max = 0;
            foreach (AlgorithmFirstStepModel item in _model.FirstStepModels)
            {
                if (item.FunctionResult > max)
                {
                    max = item.FunctionResult;
                }
            }
            return max;
        }

        private double GetMinValue()
        {
            double min = _parameters.RangeTo;
            foreach (AlgorithmFirstStepModel item in _model.FirstStepModels)
            {
                if (item.FunctionResult < min)
                {
                    min = item.FunctionResult;
                }
            }
            return min;
        }

        private double GetProbability()
        {
            Random random = new Random();
            return Math.Round(random.NextDouble(), _parameters.Precision.IntValue);
        }

        private double GetSumOfFitness(List<AlgorithmSecondStepModel> param)
        {
            var sum = 0.0;
            foreach (AlgorithmSecondStepModel item in param)
            {
                sum += item.FitnessFunctionResult;
            }
            return sum;
        }

        private double SelectValue(List<AlgorithmSecondStepModel> list, double randomNumber)
        {
            double value = 0;
            foreach (AlgorithmSecondStepModel item in list)
            {
                if (randomNumber < item.DistributionFunctionResult)
                {
                    return item.RealValue;
                }
            }
            return value;
        }
    }
}
