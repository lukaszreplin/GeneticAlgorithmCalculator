using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Models;
using GeneticAlgorithmCalculator.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
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
        private double EliteNumber = 0;
        private ChartModel _chartModel;
        private static string filename1 = $"results1_{DateTime.Now.ToString("dd/MM/yyyy_HH/mm/ss")}.txt";
        private static string filename2 = $"results2_{DateTime.Now.ToString("dd/MM/yyyy_HH/mm/ss")}.txt";
        private static string filePath1 = Path.Combine("RESULTS", filename1);
        private static string filePath2 = Path.Combine("RESULTS", filename2);
        public GeneratorService(INumberConverter converter)
        {
            _converter = converter;
            _model = new DataModel();
            
        }

        public List<AlgorithmFirstStepModel> GenerateFirstStep()
        {
            _chartModel = new ChartModel();
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
                    FitnessFunctionResult = (firstSelection) ? (_model.FirstStepModels[i - 1].FunctionResult - GetMinValue(firstSelection)) + _parameters.Precision.Value :
                    _model.ThirdStepModels[i - 1].RealValue2 - GetMinValue(firstSelection) + _parameters.Precision.Value
                });
            }

            EliteNumber = model.Max(_ => _.RealValue);

            var fitnessSum =  model.Sum(_ => _.FitnessFunctionResult);
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

            #region CROSSOVER
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
                            item.Children = firstPart1 + secondPart2;
                            item2.Children = firstPart2 + secondPart1;
                            break;
                        }
                    }
                    if (!foundedPair)
                    {
                        item.Children = item.ChoosenParents;
                    }
                }
            }
            #endregion


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

            #region ELITISM
            if (_parameters.Elitism)
            {
                if (!model.Any(_ => _.RealValue2 == EliteNumber))
                {
                    while (true)
                    {
                        var positionToInsert = GetRandomInt(0, _parameters.PopulationSize - 1, randomGenerator);
                        if (model[positionToInsert].RealValue2 <= EliteNumber)
                        {
                            model[positionToInsert].RealValue2 = EliteNumber;
                            model[positionToInsert].FunctionResult = GetFunctionResult(EliteNumber);
                            break;
                        }
                    }
                    
                }
            }
            #endregion

            _chartModel.Mins.Add(model.Min(_ => _.FunctionResult));
            _chartModel.Maxs.Add(model.Max(_ => _.FunctionResult));
            _chartModel.Avgs.Add(model.Average(_ => _.FunctionResult));
            return model;
        }

        private List<AlgorithmResultModel> GetResults()
        {
            var model = new List<AlgorithmResultModel>();
            var uniqueValues = _model.ThirdStepModels.Distinct(new ThirdModelComparer()).ToList();
            for (int i = 1; i <= uniqueValues.Count(); i++)
            {
                model.Add(new AlgorithmResultModel()
                {
                    Id = i,
                    RealValue = uniqueValues[i - 1].RealValue2,
                    BinaryValue = _converter.IntToBinaryConvert(_converter.RealToIntConvert(uniqueValues[i - 1].RealValue2)),
                    FuncResult = uniqueValues[i - 1].FunctionResult,
                    Percent = (double)_model.ThirdStepModels.Count(_ => _.FunctionResult == uniqueValues[i - 1].FunctionResult) /
                    (double)_parameters.PopulationSize*100.0
                });
            }
            return model.OrderByDescending(_ => _.Percent).ToList();
        }

        public DataModel GetData(ParametersModel parameters, bool test = false)
        {
            
            _parameters = parameters;
            if (!test)
            {
                CreateFile();
                File.WriteAllLines(filePath1, new List<string>() { $"Parameters: <{_parameters.RangeFrom};{_parameters.RangeTo}>, N = {_parameters.PopulationSize}, " +
                $"Number of generations: {_parameters.NumberOfGenerations}, Crossover prob.: {_parameters.CrossoverProbability}, " +
                $"Mutation prob.: {_parameters.MutationProbability}" });
                File.WriteAllLines(filePath2, new List<string>() { $"Parameters: <{_parameters.RangeFrom};{_parameters.RangeTo}>, N = {_parameters.PopulationSize}, " +
                $"Number of generations: {_parameters.NumberOfGenerations}, Crossover prob.: {_parameters.CrossoverProbability}, " +
                $"Mutation prob.: {_parameters.MutationProbability}" });
            }
            _model.FirstStepModels = GenerateFirstStep();
            _model.ResultModel = new List<AlgorithmResultModel>();
            for (int i = 1; i <= _parameters.NumberOfGenerations; i++)
            {
                if (!test)
                 File.AppendAllLines(filePath1, new List<string>() { $"Generation {i}" });
                _model.SecondStepModels = GenerateSecondStep(i == 1);
                _model.ThirdStepModels = GenerateThirdStep();
                if (!test)
                {
                    foreach (var item in _model.ThirdStepModels)
                    {
                        File.AppendAllLines(filePath1, new List<string>() { $"{item.Id}   {item.RealValue2}   " +
                        $"{_converter.IntToBinaryConvert(_converter.RealToIntConvert(item.RealValue2))}   {item.FunctionResult}" });
                    }
                }
            }
            _model.ResultModel = GetResults();
            if (!test)
            {
                File.AppendAllLines(filePath2, new List<string>()
                {
                    "Lp   f(min)   f(avg)   f(max)"
                });
                for (int i = 0; i < _parameters.NumberOfGenerations; i++)
                {
                    File.AppendAllLines(filePath2, new List<string>()
                {
                    $"{i+1}   {_chartModel.Mins[i]}   {_chartModel.Avgs[i]}   {_chartModel.Maxs[i]}"
                });
                }
            }
            return _model;
        }

        public ChartModel GetChartModel()
        {
            return _chartModel;
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

        private double GetMinValue(bool firstSelection = true)
        {
            if (firstSelection)
            {
                return _model.FirstStepModels.Min(_ => _.FunctionResult);
            }
            else
            {
                return _model.ThirdStepModels.Min(_ => _.RealValue2);
            }
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

        private void CreateFile()
        {
            Directory.CreateDirectory("./RESULTS");

            var res1 = File.Create(filePath1);
            var res2 = File.Create(filePath2);
            res1.Close();
            res2.Close();
        }

    }
}
