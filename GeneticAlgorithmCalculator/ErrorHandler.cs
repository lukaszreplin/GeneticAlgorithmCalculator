using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GeneticAlgorithmCalculator
{
    public class ErrorHandler
    {
        public static void HandleException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.Message);
            Console.WriteLine(args.Exception.Message);
#if !DEBUG
            args.Handled = true;
#endif
        }
    }
}
