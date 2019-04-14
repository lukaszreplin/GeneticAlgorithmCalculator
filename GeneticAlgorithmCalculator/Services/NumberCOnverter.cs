using GeneticAlgorithmCalculator.Contracts;
using GeneticAlgorithmCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Services
{
    public class NumberConverter : INumberConverter
    {
        private ParametersModel _parameters;
        private int _individualResolution;

        public int BinaryToIntConvert(string input)
        {
            throw new NotImplementedException();
        }

        public string IntToBinaryConvert(int input)
        {
            throw new NotImplementedException();
        }

        public double IntToRealConvert(int input)
        {
            throw new NotImplementedException();
        }

        public int RealToIntConvert(double input)
        {
            throw new NotImplementedException();
        }

        public void SetParameters(ParametersModel model)
        {
            _parameters = model;
        }

        private void SetIndividualResolution()
        {
            _individualResolution = int.Parse(Math.Ceiling(Math.Log(((_parameters.RangeTo - _parameters.RangeFrom)/
                _parameters.Precision.Value)+1, 2)).ToString());
        }
    }
}
