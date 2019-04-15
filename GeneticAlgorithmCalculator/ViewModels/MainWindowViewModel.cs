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

        private ICollection<AlgorithmFirstStepModel> firstStepModels;
        public ICollection<AlgorithmFirstStepModel> FirstStepModels
        {
            get { return firstStepModels; }
            set { SetProperty(ref firstStepModels, value); }
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
            FirstStepModels = _generator.GenerateFirstStep(Parameters);
        }

        private void InitializeData()
        {
            FirstStepModels = new List<AlgorithmFirstStepModel>();
            Parameters = new ParametersModel();
            Precisions = new List<PrecisionModel>()
            {
                new PrecisionModel() { Label = "0,001", Value = 3 },
                new PrecisionModel() { Label = "0,01", Value = 2 },
                new PrecisionModel() { Label = "0,1", Value = 1 }
            };
        }
    }
}
