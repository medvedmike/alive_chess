﻿using System;
using System.Windows.Threading;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Commands.CastleCommand;
using AliveChess.GameLayer.PresentationLayer;

namespace AliveChess.GameLayer.LogicLayer.Executors.CastleExecutors
{
    class CreateBuildingExecutor : IExecutor
    {
        #region IExecutor Members

        public void Execute(ICommand command)
        {
            CreateBuildingResponse response = (CreateBuildingResponse)command;
            lock (GameCore.Instance.Player.King.CurrentCastle.BuildingManager.BuildingQueue)
            {
                if (response.BuildingQueue != null)
                {
                    GameCore.Instance.Player.King.CurrentCastle.BuildingManager.BuildingQueue = response.BuildingQueue;
                }
                else
                {
                    GameCore.Instance.Player.King.CurrentCastle.BuildingManager.BuildingQueue.Clear();
                }
            }
            GameCore.Instance.CastleCommandController.BuildingQueueModified = true;
        }

        #endregion
    }
}
