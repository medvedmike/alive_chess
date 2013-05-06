﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using AliveChess.GameLayer.PresentationLayer;
using AliveChessLibrary.Commands.CastleCommand;
using AliveChessLibrary.GameObjects.Buildings;

namespace AliveChess.GameLayer.LogicLayer
{
    public class CastleCommandController
    {
        private GameCore _gameCore;

        private Castle _castle;

        public Castle Castle
        {
            get { return _castle; }
            set
            { _castle = value; }
        }

        private CastleScene _castleScene;

        public CastleScene CastleScene
        {
            get { return _castleScene; }
            set { _castleScene = value; }
        }

        public CastleCommandController(GameCore gameCore)
        {
            _gameCore = gameCore;
        }

        public void SendGetBuildingsRequest()
        {
            GetBuildingsRequest request = new GetBuildingsRequest();
            _gameCore.Network.Send(request);
        }

        public void ReceiveGetBuildingsResponce(GetBuildingsResponse response)
        {
            _castle.InnerBuildings = CustomConverter.ListToEntitySet(response.Buildings);
            _castleScene.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(_castleScene.ShowGetBuildingsResult));
        }

        public void SendCreateBuildingRequest(InnerBuildingType type)
        {
            CreateBuildingRequest request = new CreateBuildingRequest();
            request.InnerBuildingType = type;
            _gameCore.Network.Send(request);
        }

        public void ReceiveCreateBuildingResponce(CreateBuildingResponse response)
        {
            _castle.InnerBuildings = CustomConverter.ListToEntitySet(response.Buildings);
            _castleScene.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(_castleScene.ShowGetBuildingsResult));
        }
    }
}
