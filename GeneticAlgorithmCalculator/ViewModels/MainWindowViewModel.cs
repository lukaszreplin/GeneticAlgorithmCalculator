using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GeneticAlgorithmCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IGeneratorService _generator;

        private ParametersModel _parameters;
        public ParametersModel Parameters
        {
            get { return _parameters; }
            set { SetProperty(ref _parameters, value); }
        }

        private ICollection<PrecisionModel> _precisions;
        public ICollection<PrecisionModel> Precisions
        {
            get { return _precisions; }
            set { SetProperty(ref _precisions, value); }
        }

        private DataModel dataModel;
        public DataModel DataModel
        {
            get { return dataModel; }
            set { SetProperty(ref dataModel, value); }
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
            DataModel = _generator.GetData(Parameters);
        }

        private void InitializeData()
        {
            Parameters = new ParametersModel();
            Precisions = new List<PrecisionModel>()
            {
                new PrecisionModel() { Label = "0,001", Value = 0.001, IntValue = 3 },
                new PrecisionModel() { Label = "0,01", Value = 0.01, IntValue = 2 },
                new PrecisionModel() { Label = "0,1", Value = 0.1, IntValue = 1 }
            };
        }
    }
}
