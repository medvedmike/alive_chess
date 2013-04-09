// Static Model

using System;

namespace BehaviorAILibrary.DecisionLayer.NeuralNetwork
{
    [Serializable]
    public class NeuralNetwork : INeuralNetwork
    {
        private NeuralNetworkLayer inputLayer = null;
        private NeuralNetworkLayer hiddenLayer = null;
        private NeuralNetworkLayer outputLayer = null;

        public NeuralNetwork(int nNodesInput, int nNodesHidden, int nNodesOutput)
        {
            Initialize(nNodesInput, nNodesHidden, nNodesOutput);
        }

        public void Initialize(int nNodesInput, int nNodesHidden, int nNodesOutput)
        {
            inputLayer = new NeuralNetworkLayer();
            hiddenLayer = new NeuralNetworkLayer();
            outputLayer = new NeuralNetworkLayer();

            inputLayer.NumberOfNodes = nNodesInput;
            inputLayer.NumberOfChildNodes = nNodesHidden;
            inputLayer.NumberOfParentNodes = 0;
            inputLayer.Initialize(nNodesInput, null, hiddenLayer);
            inputLayer.RandomizeWeights();
            hiddenLayer.NumberOfNodes = nNodesHidden;
            hiddenLayer.NumberOfChildNodes = nNodesOutput;
            hiddenLayer.NumberOfParentNodes = nNodesInput;
            hiddenLayer.Initialize(nNodesHidden, inputLayer, outputLayer);
            hiddenLayer.RandomizeWeights();
            outputLayer.NumberOfNodes = nNodesOutput;
            outputLayer.NumberOfChildNodes = 0;
            outputLayer.NumberOfParentNodes = nNodesHidden;
            outputLayer.Initialize(nNodesOutput, hiddenLayer, null);
        }

        public void FeedForward()
        {
            inputLayer.CalculateNeuronValues();
            hiddenLayer.CalculateNeuronValues();
            outputLayer.CalculateNeuronValues();
        }

        public void BackPropagate()
        {
            outputLayer.CalculateErrors();
            hiddenLayer.CalculateErrors();
            hiddenLayer.AdjustWeights();
            inputLayer.AdjustWeights();
        }

        public void SetMomentum(bool useMomentum, double factor)
        {
            inputLayer.UseMomentum = useMomentum;
            hiddenLayer.UseMomentum = useMomentum;
            outputLayer.UseMomentum = useMomentum;
            inputLayer.MomentumFactor = factor;
            hiddenLayer.MomentumFactor = factor;
            outputLayer.MomentumFactor = factor;
        }

        public void SetInput(int i, double value)
        {
            if ((i >= 0) && (i < inputLayer.NumberOfNodes))
            {
                inputLayer.NeuronValues[i] = value;
            }
        }

        public double GetOutput(int i)
        {
            if ((i >= 0) && (i < outputLayer.NumberOfNodes))
            {
                return outputLayer.NeuronValues[i];
            }
            return (double)Double.PositiveInfinity; // to indicate an error
        }

        public void SetDesiredOutput(int i, double value)
        {
            if ((i >= 0) && (i < outputLayer.NumberOfNodes))
            {
                outputLayer.DesiredValues[i] = value;
            }
        }

        public int MaxOutputId
        {
            get
            {
                int id = 0;
                double maxval = outputLayer.NeuronValues[0];
                for (int i = 1; i < outputLayer.NumberOfNodes; i++)
                {
                    if (outputLayer.NeuronValues[i] > maxval)
                    {
                        maxval = outputLayer.NeuronValues[i];
                        id = i;
                    }
                }
                return id;
            }
        }

        public int MaxOutputIDFake { get { return 0; } }

        public double LearningRate
        {
            set
            {
                inputLayer.LearningRate = value;
                hiddenLayer.LearningRate = value;
                outputLayer.LearningRate = value;
            }
        }

        public bool LinearOutput
        {
            set
            {
                inputLayer.LinearOutput = value;
                hiddenLayer.LinearOutput = value;
                outputLayer.LinearOutput = value;
            }
        }

        public double CalculateError()
        {
            double error = 0.0;
            for (int i = 0; i < outputLayer.NumberOfNodes; i++)
            {
                error += Math.Pow(outputLayer.NeuronValues[i] - outputLayer.DesiredValues[i], 2);
            }
            return error / outputLayer.NumberOfNodes;
        }

    }// END CLASS DEFINITION NeuralNetwork

} // AliveChess.Logic.GameLogic.BotsLogic.DecisionLayer.NeuralNetwork
