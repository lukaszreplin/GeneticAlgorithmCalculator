using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class AlgorithmResultModel
    {
        public int Generation { get; set; }

        public int Id { get; set; }

        public double RealValue1 { get; set; }

        public double FuncResult1 { get; set; }

        public double Selection { get; set; }

        public string Crossover { get; set; }

        public string Mutation { get; set; }

        public double RealValue2 { get; set; }

        public double FuncResult2 { get; set; }
    }
}
