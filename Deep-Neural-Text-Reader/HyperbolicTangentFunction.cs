using AForge.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deep_Neural_Text_Reader
{
    public class HyperbolicTangentFunction : IActivationFunction
    {
        public double Derivative(double x)
        {
            return 1 / Math.Cosh(x);
        }

        public double Derivative2(double y)
        {
            return 1 - y * y;
        }

        public double Function(double x)
        {
            return Math.Tanh(x);
        }
    }
}
