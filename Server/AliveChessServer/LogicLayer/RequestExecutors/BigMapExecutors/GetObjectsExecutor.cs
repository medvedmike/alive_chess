﻿using System.Collections.Generic;
using AliveChessLibrary.Commands.BigMapCommand;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessLibrary.Interfaces;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.UsersManagement;
using AliveChessServer.NetLayer;

namespace AliveChessServer.LogicLayer.RequestExecutors.BigMapExecutors
{
    public class GetObjectsExecutor : IExecutor
    {
        private GameWorld _environment;
      
        public GetObjectsExecutor(GameLogic gameLogic)
        {
            this._environment = gameLogic.Environment;
        }

        public void Execute(Message msg)
        {
            GetObjectsRequest request = (GetObjectsRequest)msg.Command;

            List<King> kings = null;
            List<Resource> resources = null;

            Player player = msg.Sender;
            Level level = _environment.LevelManager.GetLevelById(player.LevelId);
           
            // получаем область видимости игрока)
            if (!request.ForConcreteObserver)
            {
                kings = level.BigMapRoutine.GetGameObjects<King>(player.VisibleSpace, PointTypes.King, player);
                resources = level.BigMapRoutine.GetGameObjects<Resource>(player.VisibleSpace, PointTypes.Resource, player);
            }
            else // получаем область видимости конкретного объекта
            {
                // получение объекта на карте
                IObserver observer = player.Level.Map.SearchObserverById(request.ObserverId, request.ObserverType);

                kings = level.BigMapRoutine.GetGameObjects<King>(observer.VisibleSpace, PointTypes.King, observer);
                resources = level.BigMapRoutine.GetGameObjects<Resource>(observer.VisibleSpace, PointTypes.Resource, observer);
            }

            if (kings != null || resources != null)
                player.Messenger.SendNetworkMessage(new GetObjectsResponse(resources, kings));
        }
    }
}
