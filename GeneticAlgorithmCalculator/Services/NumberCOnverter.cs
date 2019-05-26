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
            return Convert.ToInt32(input, 2);
        }

        public string IntToBinaryConvert(int input)
        {
            var binaryValue = Convert.ToString(input, 2);
            if (binaryValue.Length < _individualResolution)
            {
                var builder = new StringBuilder();
                for (int i = 0; i < _individualResolution - binaryValue.Length; i++)
                {
                    builder.Append("0");
                }
                builder.Append(binaryValue);
                return builder.ToString();
            }
            return binaryValue;
        }

        public double IntToRealConvert(int input)
        {
            return Math.Round((input * (_parameters.RangeTo - _parameters.RangeFrom) / (Math.Pow(2, _individualResolution) - 1)) 
                + _parameters.RangeFrom, _parameters.Precision.IntValue);
        }

        public int RealToIntConvert(double input)
        {
            return Convert.ToInt32(Math.Round((1 / (_parameters.RangeTo - _parameters.RangeFrom)) * 
                (input - _parameters.RangeFrom) * (Math.Pow(2, _individualResolution) - 1)));
        }

        public void SetParameters(ParametersModel model)
        {
            _parameters = model;
            SetIndividualResolution();
        }

        private void SetIndividualResolution()
        {
            _individualResolution = int.Parse(Math.Ceiling(Math.Log(((_parameters.RangeTo - _parameters.RangeFrom)/
                _parameters.Precision.Value)+1, 2)).ToString());
        }
    }
}
