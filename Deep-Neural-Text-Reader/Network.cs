using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord.Math;
using Accord.Neuro.Learning;
using AForge.Neuro;
using Accord.Controls;

namespace Deep_Neural_Text_Reader
{
    class Network
    {
        private IActivationFunction activationFunction;
        private ActivationNetwork network;
        private LevenbergMarquardtLearning teacher;

        public int iterationCount { set; get; }

        public double[][] input { set; get; }
        public int[] output { set; get; }

        public Network(int inputsCount, int[] neuronsCount)
        {
            activationFunction = new BipolarSigmoidFunction();
            network = new ActivationNetwork(activationFunction, inputsCount, neuronsCount);
            teacher = new LevenbergMarquardtLearning(network)
            {
                UseRegularization = false
            };
        }

        public void Learn()
        {
            double[][] y = output.ToDouble().ToArray();

            double error;
            for (int i = 0; i < iterationCount; ++i)
            {
                error = teacher.RunEpoch(input, y);
            }
        }
        
        public double[][] CalculateAnswer(double[][] input)
        {
            return input.Apply(network.Compute);
        }
    }
}
