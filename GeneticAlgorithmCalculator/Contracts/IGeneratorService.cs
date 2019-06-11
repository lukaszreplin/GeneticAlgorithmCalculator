using GeneticAlgorithmCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Contracts
{
    public interface IGeneratorService
    {
        DataModel GetData(ParametersModel parameters, bool test = false);
        List<AlgorithmFirstStepModel> GenerateFirstStep();

        List<AlgorithmSecondStepModel> GenerateSecondStep(bool firstSelection);

        List<AlgorithmThirdStepModel> GenerateThirdStep();

        ChartModel GetChartModel();
    }
}
