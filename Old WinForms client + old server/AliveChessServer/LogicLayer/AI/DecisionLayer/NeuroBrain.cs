///////////////////////////////////////////////////////////
//  NeuroBrain.cs
//  Implementation of the Class NeuroBrain
//  Generated by Enterprise Architect
//  Created on:      16-���-2009 2:10:28
///////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using AliveChessLibrary.GameObjects.Buildings;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessServer.LogicLayer.AI.BehaviorLayer;
using AliveChessServer.LogicLayer.AI.BehaviorLayer.ComposeGoals;
using AliveChessServer.LogicLayer.AI.DecisionLayer.NeuralNetwork;
using AliveChessServer.LogicLayer.AI.PerceptionLayer;
using AliveChessServer.LogicLayer.AI.PerseptionLayer;
using AliveChessServer.Properties;

namespace AliveChessServer.LogicLayer.AI.DecisionLayer
{
    public class NeuroBrain : IBrain
    {
        private NeuroTeacher teacher;
        private INeuralNetwork network;
        private PriorityQueue memory = new PriorityQueue();        
        private BotKing king;
        private PoolingStation station;
        private GameSetting setting;
        private GoalFactory factory;
       
        public NeuroBrain(BotKing king, NeuroTeacher teacher, PoolingStation station)
        {
            this.king = king;
            this.teacher = teacher;
            this.station = station;
            this.setting = new GameSetting(king);
            this.factory = new GoalFactory();

            WanderGoal w = new WanderGoal(this.king);
            w.Priority = 10;
            memory.Enqueue(w);
        }

        public virtual void Dispose()
        {

        }

        public void Process()
        {
            Goal currentGoal = memory.Peek();
            GoalStatuses status = currentGoal.Process();
            if (status == GoalStatuses.Completed || status == GoalStatuses.Failed)
            {
                currentGoal.Terminate();
                memory.Dequeue();
            }
        }

        public virtual void Think()
        {
            // pool game setting and get needed parameters
            setting.CollectInfo(
                station.Kings,
                station.Castles,
                station.Mines,
                station.Resources);

            // set inputs
            PrepareData();

            // make decision
            network.FeedForward();

            if (setting.MinDistanceToEnemyKing < 10 && setting.MinDistanceToEnemyKing > 0)
            {
                //if (!factory.GetPriority(teacher, network, memory))
                //{
                Goal goal = factory.CreateGoal(king, teacher, network, setting, memory);
                if (goal != null)
                    memory.Enqueue(goal);
                //}
            }
        }

        private void PrepareData()
        {
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.DistanceToEnemyCastle),
               setting.MinDistanceToEnemyCastle);
            network.SetInput(
                teacher.GetInputNumber(NeuroTeacher.InputState.DistanceToEnemyKing),
                setting.MinDistanceToEnemyKing);
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.DistanceToPlayerCastle),
               setting.MinDistanceToPlayerCastle);
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.EnemyUnitsCountInsideCastle),
               setting.UnitsCountInsideEnemyCastle);
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.EnemyUnitsCountTogetherWithKing),
               setting.UnitsCountTogetherWithEnemyKing);
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.ResourceCountInVisibleArea),
               setting.ResourceCountInVisibleArea);
            network.SetInput(
               teacher.GetInputNumber(NeuroTeacher.InputState.ResourceCountOnHand),
               setting.ResourceCountOnHand);
        }

        public INeuralNetwork NN
        {
            get
            {
                return network;
            }
            set
            {
                network = value;
            }
        }

    }//end NeuroBrain

}//end namespace DecisionLayer