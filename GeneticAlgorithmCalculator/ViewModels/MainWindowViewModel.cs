using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Extensions;
using GeneticAlgorithmCalculator.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IGeneratorService _generator;
        private DataModel model;
        private ChartModel _chartModel;
        private static string filename = $"test_{DateTime.Now.ToString("dd..MM.yyyy_HH.mm.ss")}.txt";

        private string executionTime;
        public string ExecutionTime
        {
            get { return executionTime; }
            set { SetProperty(ref executionTime, value); }
        }

        private int nNumber;
        public int NNumber
        {
            get { return nNumber; }
            set { SetProperty(ref nNumber, value); }
        }

        private int tNumber;
        public int TNumber
        {
            get { return tNumber; }
            set { SetProperty(ref tNumber, value); }
        }

        private ParametersModel _parameters;
        public ParametersModel Parameters
        {
            get { return _parameters; }
            set { SetProperty(ref _parameters, value); }
        }

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return seriesCollection; }
            set { SetProperty(ref seriesCollection, value); }
        }

        private ICollection<PrecisionModel> _precisions;
        public ICollection<PrecisionModel> Precisions
        {
            get { return _precisions; }
            set { SetProperty(ref _precisions, value); }
        }

        private ObservableCollection<AlgorithmFirstStepModel> firstStepDataModel;
        public ObservableCollection<AlgorithmFirstStepModel> FirstStepDataModel
        {
            get { return firstStepDataModel; }
            set
            {
                SetProperty(ref firstStepDataModel, value);
            }
        }

        private ObservableCollection<AlgorithmSecondStepModel> secondStepDataModel;
        public ObservableCollection<AlgorithmSecondStepModel> SecondStepDataModel
        {
            get { return secondStepDataModel; }
            set
            {
                SetProperty(ref secondStepDataModel, value);
            }
        }

        private ObservableCollection<AlgorithmThirdStepModel> thirdStepDataModel;
        public ObservableCollection<AlgorithmThirdStepModel> ThirdStepDataModel
        {
            get { return thirdStepDataModel; }
            set
            {
                SetProperty(ref thirdStepDataModel, value);
            }
        }

        private ObservableCollection<AlgorithmResultModel> resultModel;
        public ObservableCollection<AlgorithmResultModel> ResultModel
        {
            get { return resultModel; }
            set
            {
                SetProperty(ref resultModel, value);
            }
        }

        private bool tests;
        public bool Tests
        {
            get { return tests; }
            set { SetProperty(ref tests, value); }
        }

        public MainWindowViewModel(IGeneratorService generatorService)
        {
            _generator = generatorService;
            Tests = false;
            InitializeData();
        }

        private DelegateCommand _processCommand;
        public DelegateCommand ProcessCommand =>
            _processCommand ?? (_processCommand = new DelegateCommand(ExecuteProcessCommand));

        async void ExecuteProcessCommand()
        {
            if (!Tests)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                model = _generator.GetData(Parameters);
                watch.Stop();
                ExecutionTime = $"{watch.ElapsedMilliseconds}ms";
                FirstStepDataModel = model.FirstStepModels.ToObservableCollection();
                SecondStepDataModel = model.SecondStepModels.ToObservableCollection();
                ThirdStepDataModel = model.ThirdStepModels.ToObservableCollection();
                ResultModel = model.ResultModel.ToObservableCollection();
                _chartModel = _generator.GetChartModel();
                ChartValues<double> minChartValues = new ChartValues<double>();
                ChartValues<double> maxChartValues = new ChartValues<double>();
                ChartValues<double> avgChartValues = new ChartValues<double>();
                foreach (var min in _chartModel.Mins)
                {
                    minChartValues.Add(min);
                }
                foreach (var avg in _chartModel.Avgs)
                {
                    avgChartValues.Add(avg);
                }
                foreach (var max in _chartModel.Maxs)
                {
                    maxChartValues.Add(max);
                }
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Values = minChartValues,
                        Title = "MIN",
                        Name = "MIN"
                    },
                    new LineSeries
                    {
                        Values = avgChartValues,
                        Title = "AVG",
                        Name = "AVG"
                    },
                    new LineSeries
                    {
                        Values = maxChartValues,
                        Title = "MAX",
                        Name = "MAX"
                    }
                };
            }
            else
            {
                var pks = new List<double>() { 0.5, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85, 0.9 };
                var pms = new List<double>() { 0.0001, 0.0005, 0.001, 0.0015, 0.002, 0.0025, 0.003, 0.0035, 0.004, 0.00045, 0.005,
                0.0055, 0.006, 0.0065, 0.007, 0.0075, 0.008, 0.0085, 0.009, 0.0095, 0.01 };
                File.AppendAllLines(filename, new List<string>() { $"N\tT\tpk\tpm\tf(x)" });
                for (int N = 30; N <= 80; N+=10)
                {
                    NNumber = N;
                    for (int T = 50; T <= 150; T+=10)
                    {
                        TNumber = T;
                        foreach (var pk in pks)
                        {
                            foreach (var pm in pms)
                            {
                                double sum = 0;
                                for (int i = 0; i < 100; i++)
                                {
                                    sum += await Task.Run(() => {

                                        return _generator.GetData(new ParametersModel()
                                        {
                                            NumberOfGenerations = N,
                                            PopulationSize = T,
                                            CrossoverProbability = (float)pk,
                                            MutationProbability = (float)pm,
                                            RangeFrom = Parameters.RangeFrom,
                                            RangeTo = Parameters.RangeTo,
                                            Precision = Parameters.Precision
                                        }, true).ResultModel.Max(_ => _.FuncResult);
                                    });
                                }
                                File.AppendAllLines(filename, new List<string>() { $"{N}\t{T}\t{pk}\t" +
                                    $"{pm}\t{(double)sum/100}"});
                            }
                        }
                    }
                }
            }
        }

        private void InitializeData()
        {
            Parameters = new ParametersModel
            {
                RangeFrom = -4,
                RangeTo = 12,
                PopulationSize = 10,
                NumberOfGenerations = 1,
                CrossoverProbability = 0.75f,
                MutationProbability = 0.005f,
                Elitism = false
            };
            Precisions = new List<PrecisionModel>()
            {
                new PrecisionModel() { Label = "0,001", Value = 0.001, IntValue = 3 },
                new PrecisionModel() { Label = "0,01", Value = 0.01, IntValue = 2 },
                new PrecisionModel() { Label = "0,1", Value = 0.1, IntValue = 1 }
            };
            SeriesCollection = new SeriesCollection();
        }
    }
}
