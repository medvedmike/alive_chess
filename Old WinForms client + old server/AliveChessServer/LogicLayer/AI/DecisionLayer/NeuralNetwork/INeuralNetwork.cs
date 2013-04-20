///////////////////////////////////////////////////////////
//  INeuralNetwork.cs
//  Implementation of the Interface INeuralNetwork
//  Generated by Enterprise Architect
//  Created on:      16-���-2009 2:08:37
///////////////////////////////////////////////////////////


namespace AliveChessServer.LogicLayer.AI.DecisionLayer.NeuralNetwork
{
	public interface INeuralNetwork {

		void FeedForward();

		/// 
		/// <param name="i"></param>
		/// <param name="value"></param>
		void SetInput(int i, double value);

		/// 
		/// <param name="i"></param>
		double GetOutput(int i);

		/// 
		/// <param name="i"></param>
		/// <param name="value"></param>
		void SetDesiredOutput(int i, double value);

		/// 
		/// <param name="nNodesInput"></param>
		/// <param name="nNodesHidden"></param>
		/// <param name="nNodesOutput"></param>
		void Initialize(int nNodesInput, int nNodesHidden, int nNodesOutput);

		void BackPropagate();

	    double CalculateError();

        int MaxOutputId { get; }

	    int MaxOutputIDFake { get; }

	}//end INeuralNetwork

}//end namespace NeuralNetwork