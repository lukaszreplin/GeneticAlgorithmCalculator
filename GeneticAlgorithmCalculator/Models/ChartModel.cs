using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmCalculator.Models
{
    public class ChartModel
    {
        public List<double> Maxs { get; set; }

        public List<double> Mins { get; set; }

        public List<double> Avgs { get; set; }

        public ChartModel()
        {
            Maxs = new List<double>();
            Mins = new List<double>();
            Avgs = new List<double>();
        }
    }
}
