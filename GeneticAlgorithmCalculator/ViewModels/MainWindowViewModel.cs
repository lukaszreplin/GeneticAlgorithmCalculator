using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Extensions;
using GeneticAlgorithmCalculator.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GeneticAlgorithmCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IGeneratorService _generator;
        private DataModel model;
        private ChartModel _chartModel;

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

        public MainWindowViewModel(IGeneratorService generatorService)
        {
            _generator = generatorService;
            InitializeData();
        }

        private DelegateCommand _processCommand;
        public DelegateCommand ProcessCommand =>
            _processCommand ?? (_processCommand = new DelegateCommand(ExecuteProcessCommand));

        void ExecuteProcessCommand()
        {
            model = _generator.GetData(Parameters);
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
                    Values = minChartValues
                },
                new LineSeries
                {
                    Values = avgChartValues
                },
                new LineSeries
                {
                    Values = maxChartValues
                }
            };
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
