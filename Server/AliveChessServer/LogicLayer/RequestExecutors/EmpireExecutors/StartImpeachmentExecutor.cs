﻿using AliveChessLibrary.Commands.EmpireCommand;
using AliveChessLibrary.Interaction;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.Environment.Alliances;
using AliveChessServer.LogicLayer.UsersManagement;
using AliveChessServer.NetLayer;

namespace AliveChessServer.LogicLayer.RequestExecutors.EmpireExecutors
{
    public class StartImpeachmentExecutor : IExecutor
    {
        private GameWorld _environment;
        private PlayerManager _playerManager;
        private LevelRoutine _levelManager;

        public StartImpeachmentExecutor(GameLogic gameLogic)
        {
            this._environment = gameLogic.Environment;
            this._playerManager = gameLogic.PlayerManager;
            this._levelManager = gameLogic.Environment.LevelManager;
        }

        public void Execute(Message msg)
        {
            StartImpeachmentRequest request = (StartImpeachmentRequest)msg.Command;

            Player player = msg.Sender;
            Level level = _levelManager.GetLevelById(player.LevelId);
            IAlliance aliance = level.EmpireManager.GetAlianceByMember(player.King);
            Empire empire = aliance as Empire;
            if (empire != null && empire.AllowStartImpeachment)
            {
                // Start impeachment procedure
                empire.StartExile();

                aliance.PublishNews(player, NewsType.ImpeachmentStarted, request.Message);
            }
        }
    }
}
