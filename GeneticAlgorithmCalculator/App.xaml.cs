using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Services;
using GeneticAlgorithmCalculator.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace GeneticAlgorithmCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            DispatcherUnhandledException
                += ErrorHandler.HandleException;

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IGeneratorService, GeneratorService>();
            containerRegistry.Register<INumberConverter, NumberConverter>();
        }
    }
}
