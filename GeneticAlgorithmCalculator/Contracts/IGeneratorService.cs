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
        List<AlgorithmFirstStepModel> GenerateFirstStep(ParametersModel parameters);
    }
}
