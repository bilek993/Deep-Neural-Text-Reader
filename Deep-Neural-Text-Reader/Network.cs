using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord.Math;
using Accord.Neuro.Learning;
using AForge.Neuro;
using Accord.Controls;
using Accord.Neuro;
using AForge.Neuro.Learning;
using System.Drawing;

namespace Deep_Neural_Text_Reader
{
    public class Network
    {
        public const int OUTPUTS_COUNT = 36;

        private IActivationFunction activationFunction;
        private ActivationNetwork network;
        private BackPropagationLearning teacher;

        private int learningLoopIterator;

        public int iterationCount { set; get; }
        private int inputsCount;
        private int[] neuronsCount;
        public double error { set; get; }

        public int InputsCount
        {
            get
            {
                return inputsCount;
            }
        }

        public int OutputsCount
        {
            get
            {
                return network.Output.Length;
            }
        }

        public int[] NeuronsCount
        {
            get
            {
                return neuronsCount;
            }
        }

        public Network()
        {

        }

        public Network(int inputsCount, int[] neuronsCount)
        {
            this.inputsCount = inputsCount;
            this.neuronsCount = neuronsCount;

            activationFunction = new HyperbolicTangentFunction();
            IActivationFunction activationFunction2 = new SigmoidFunction();
            network = new ActivationNetwork(activationFunction, inputsCount, neuronsCount);
            new NguyenWidrow(network).Randomize();
            teacher = new BackPropagationLearning(network);

            for (int i = 0; i < network.Layers.Length - 1; ++i)
            {
                ((ActivationLayer)network.Layers[i]).SetActivationFunction(activationFunction);
            }
            ((ActivationLayer)network.Layers[network.Layers.Length - 1]).SetActivationFunction(activationFunction2);

            learningLoopIterator = 0;
        }

        public void Learn(double[][] input, double[][] output)
        {
            //double[][] y = output.ToDouble();

            for (learningLoopIterator = 0; learningLoopIterator < iterationCount; ++learningLoopIterator)
            {
                error = teacher.RunEpoch(input, output);
            }
        }

        public double[][] CalculateAnswer(double[][] input)
        {
            return input.Apply(network.Compute);
        }

        public double[] CalculateAnswer(double[] input)
        {
            return network.Compute(input);
        }

        public int CalculateProgress()
        {
            return (learningLoopIterator*100)/iterationCount;
        }

        public void SaveNetwork(string fileName)
        {
            network.Save(fileName);
        }

        public void LoadNetwork(string fileName)
        {
            network = (ActivationNetwork)ActivationNetwork.Load(fileName);

            teacher = new BackPropagationLearning(network);

            inputsCount = network.Layers[0].InputsCount;
            neuronsCount = new int[network.Layers.Length];

            for (int i = 0; i < network.Layers.Length; ++i)
            {
                neuronsCount[i] = network.Layers[i].Neurons.Length;
            }
        }

        public double[] BitmapToNetworkInput(Bitmap image)
        {
            double[] input = new double[InputsCount];
            for (int i = 0; i < image.Height; ++i)
            {
                for (int j = 0; j < image.Width; ++j)
                {
                    System.Drawing.Color pixel = image.GetPixel(j, i);
                    input[i * image.Width + j] = (1 - (pixel.R + pixel.G + pixel.B) / 3.0 / 255.0) - 0.5;
                }
            }

            return input;
        }

        public char NetworkOutputToChar(double[] output)
        {
            int maxIndex = 0;
            for (int i = 1; i < output.Length; ++i)
            {
                if (output[i] > output[maxIndex])
                    maxIndex = i;
            }
            char c = IndexLetterToChar(maxIndex);

            return c;
        }

        public char IndexLetterToChar(int index)
        {
            char c = '?';
            if (index < 26)
            {
                c = (char)('A' + index);
            }
            else
            {
                c = (char)('0' + index - 26);
            }

            return c;
        }
    }
}
