using GeneticAlgorithmCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Contracts
{
    public interface INumberConverter
    {
        int RealToIntConvert(double input);
        string IntToBinaryConvert(int input);
        int BinaryToIntConvert(string input);
        double IntToRealConvert(int input);
        void SetParameters(ParametersModel model);
    }
}
