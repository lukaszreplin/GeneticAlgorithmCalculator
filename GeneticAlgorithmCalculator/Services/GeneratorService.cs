﻿using GeneticAlgorithmCalculator.Contracts;
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
                data.FunctionResult = GetFunctionResult(data.RealValue);
            }
            return model;
        }

        public List<AlgorithmSecondStepModel> GenerateSecondStep(bool firstSelection = true)
        {
            var model = new List<AlgorithmSecondStepModel>();
            for (int i = 1; i <= _parameters.PopulationSize; i++)
            {
                model.Add(new AlgorithmSecondStepModel()
                {
                    Id = i,
                    RealValue = (firstSelection) ? 
                    _model.FirstStepModels[i - 1].RealValue : _model.ThirdStepModels[i-1].RealValue2,
                    FunctionResult = (firstSelection) ? 
                    _model.FirstStepModels[i - 1].FunctionResult : _model.ThirdStepModels[i - 1].FunctionResult,
                    FitnessFunctionResult = (_model.FirstStepModels[i - 1].FunctionResult - GetMinValue()) + _parameters.Precision.IntValue,
                });
            }
            if (firstSelection)
            {
                _model.Elite = model.OrderByDescending(_ => _.FunctionResult)
                .Take(_parameters.ElitismLevel).Select(_ => _.RealValue).ToList();
            }
            else
            {
                var tempElite = _model.Elite;
                _model.Elite = null;
                foreach (var eliteItem in tempElite.OrderBy(_ => _).ToList())
                {
                    var funcResult = GetFunctionResult(eliteItem);
                    if (model.Any(_ => _.FunctionResult != funcResult))
                    {
                        var idMin = model.OrderBy(_ => _.FunctionResult).First().Id;
                        model[idMin - 1].RealValue = eliteItem;
                        model[idMin - 1].FunctionResult = funcResult;
                    }
                }
                _model.Elite = model.OrderByDescending(_ => _.FunctionResult)
                .Take(_parameters.ElitismLevel).Select(_ => _.RealValue).ToList();
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

        public List<AlgorithmThirdStepModel> GenerateThirdStep()
        {
            var model = new List<AlgorithmThirdStepModel>();
            var randomGenerator = new Random();
            var count = 1;
            foreach (var originalItem in _model.SecondStepModels)
            {
                model.Add(new AlgorithmThirdStepModel()
                {
                    Id = count,
                    RealValue = originalItem.AfterSelectionValue,
                    BinaryValue = _converter.IntToBinaryConvert(_converter.RealToIntConvert(originalItem.AfterSelectionValue))
                });
                count++;
            }
            foreach (var item in model)
            {
                var randomNumber = GetRandomNumber(0, 1, randomGenerator);
                if (randomNumber <= _parameters.CrossoverProbability)
                {
                    item.ChoosenParents = item.BinaryValue;
                }
                else
                {
                    item.ChoosenParents = string.Empty;
                }
            }
            int rememberedCuttingPoint = 0;
            foreach (var item in model)
            {
                if (item.ChoosenParents != string.Empty)
                {
                    if (rememberedCuttingPoint > 0)
                    {
                        item.CutPoint = rememberedCuttingPoint;
                        rememberedCuttingPoint = 0;
                    }
                    else
                    {
                        item.CutPoint = GetRandomInt(1, item.ChoosenParents.Length - 1, randomGenerator);
                        rememberedCuttingPoint = item.CutPoint;
                    }
                }
            }
            foreach (var item in model)
            {
                if (item.ChoosenParents != string.Empty && string.IsNullOrEmpty(item.Children))
                {
                    int id1 = item.Id;
                    bool foundedPair = false;
                    foreach (var item2 in model)
                    {
                        if (item2.ChoosenParents != string.Empty && string.IsNullOrEmpty(item2.Children) &&
                            item2.Id > id1)
                        {
                            foundedPair = true;
                            var firstPart1 = item.ChoosenParents.Substring(0, item.CutPoint);
                            var firstPart2 = item2.ChoosenParents.Substring(0, item2.CutPoint);
                            var secondPart1 = item.ChoosenParents.Substring(item.CutPoint, item.ChoosenParents.Length - item.CutPoint);
                            var secondPart2 = item2.ChoosenParents.Substring(item2.CutPoint, item2.ChoosenParents.Length - item2.CutPoint);
                            item.Children = secondPart1 + firstPart2;
                            item2.Children = secondPart2 + firstPart1;
                            break;
                        }
                    }
                    if (!foundedPair)
                    {
                        item.Children = item.ChoosenParents;
                    }
                }
            }
            foreach (var item in model)
            {
                if (string.IsNullOrEmpty(item.Children))
                {
                    item.GenerativeGeneration = item.BinaryValue;
                    continue;
                }
                item.GenerativeGeneration = item.Children;
            }
            foreach (var item in model)
            {
                item.MutationPositions = "";
                var stringArray = item.GenerativeGeneration.ToCharArray();
                for (int i = 0; i < item.GenerativeGeneration.Length; i++)
                {
                    if (GetRandomNumber(0,1,randomGenerator) < _parameters.MutationProbability)
                    {
                        int number = i + 1;
                        item.MutationPositions += $"{number.ToString()}; ";
                        if (int.Parse(stringArray[i].ToString()) == 0)
                        {
                            stringArray[i] = '1';
                        } else
                        {
                            stringArray[i] = '0';
                        }
                    }
                }
                item.MutatedGeneration = string.Join("", stringArray);
                item.RealValue2 = _converter.IntToRealConvert(_converter.BinaryToIntConvert(item.MutatedGeneration));
                item.FunctionResult = GetFunctionResult(item.RealValue2);
            }
            return model;
        }

        public DataModel GetData(ParametersModel parameters)
        {
            _parameters = parameters;
            _model.FirstStepModels = GenerateFirstStep();
            for (int i = 1; i <= _parameters.NumberOfGenerations; i++)
            {
                _model.SecondStepModels = GenerateSecondStep(i == 1);
                _model.ThirdStepModels = GenerateThirdStep();
            }
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

        private double GetRandomNumber(float from, float to, int precision = 0)
        {
            double number;
            Random random = new Random();
            number = random.NextDouble() * (to - from) + from;
            if (precision != 0)
            {
                number = Math.Round(number, precision);
            }
            return number;
        }

        private double GetRandomNumber(float from, float to, Random random, int precision = 0)
        {
            double number;
            number = random.NextDouble() * (to - from) + from;
            if (precision != 0)
            {
                number = Math.Round(number, precision);
            }
            return number;
        }

        private int GetRandomInt(int from, int to, Random random)
        {
            return random.Next(from, to);
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

        private double GetFunctionResult(double input)
        {
            return (input % 1) * (Math.Cos(20 * Math.PI * input) - Math.Sin(input));
        }
        
    }
}
