using GeneticAlgorithmCalculator.Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GeneticAlgorithmCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
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

        public MainWindowViewModel()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            Parameters = new ParametersModel();
            Precisions = new List<PrecisionModel>()
            {
                new PrecisionModel() { Label = "0,001", Value = 0.001 },
                new PrecisionModel() { Label = "0,01", Value = 0.01 },
                new PrecisionModel() { Label = "0,1", Value = 0.1 }
            };
        }
    }
}
