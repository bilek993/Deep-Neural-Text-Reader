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
        public const int OUTPUTS_COUNT = 36;

        private IActivationFunction activationFunction;
        private ActivationNetwork network;
        private LevenbergMarquardtLearning teacher;

        private int learningLoopIterator;

        public int iterationCount { set; get; }
        public double error { set; get; }

        public Network(int inputsCount, int[] neuronsCount)
        {
            activationFunction = new BipolarSigmoidFunction();
            network = new ActivationNetwork(activationFunction, inputsCount, neuronsCount);
            teacher = new LevenbergMarquardtLearning(network)
            {
                UseRegularization = false
            };
            learningLoopIterator = 0;
        }

        public void Learn(double[][] input, int[] output)
        {
            double[][] y = output.ToDouble().ToArray();

            for (learningLoopIterator = 0; learningLoopIterator < iterationCount; ++learningLoopIterator)
            {
                error = teacher.RunEpoch(input, y);
            }
        }
        
        public double[][] CalculateAnswer(double[][] input)
        {
            return input.Apply(network.Compute);
        }

        public int CalculateProgress()
        {
            return (learningLoopIterator*100)/iterationCount;
        }
    }
}
