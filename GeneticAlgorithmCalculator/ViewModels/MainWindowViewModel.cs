﻿using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Extensions;
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
        private DataModel model;

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
        }

        private void InitializeData()
        {
            Parameters = new ParametersModel();
            Parameters.RangeFrom = -4;
            Parameters.RangeTo = 12;
            Parameters.PopulationSize = 10;
            Precisions = new List<PrecisionModel>()
            {
                new PrecisionModel() { Label = "0,001", Value = 0.001, IntValue = 3 },
                new PrecisionModel() { Label = "0,01", Value = 0.01, IntValue = 2 },
                new PrecisionModel() { Label = "0,1", Value = 0.1, IntValue = 1 }
            };
        }
    }
}
