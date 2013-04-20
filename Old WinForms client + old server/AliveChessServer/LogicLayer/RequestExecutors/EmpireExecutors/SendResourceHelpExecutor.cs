﻿using AliveChessLibrary.Commands.EmpireCommand;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.UsersManagement;
using AliveChessServer.NetLayer;

namespace AliveChessServer.LogicLayer.RequestExecutors.EmpireExecutors
{
    public class SendResourceHelpExecutor : IExecutor
    {
        private GameWorld _environment;
        private PlayerManager _playerManager;

        public SendResourceHelpExecutor(LogicEntryPoint gameLogic)
        {
            this._environment = gameLogic.Environment;
            this._playerManager = gameLogic.PlayerManager;
        }

        public void Execute(Message msg)
        {
            SendResourceHelpMessage request = (SendResourceHelpMessage)msg.Command;
            Player player = msg.Sender;
            King receiver = player.Level.Map.SearchKingById(request.ReceiverId);
            if (receiver != null)
            {
                foreach (Resource u in request.Resources)
                {
                    if (player.King.StartCastle.ResourceStore.DeleteResourceFromRepository(u.ResourceType, u.CountResource))
                        receiver.StartCastle.ResourceStore.AddResourceToRepository(u);
                }
                if (!receiver.Player.Bot)
                    receiver.Player.Messenger.SendNetworkMessage(new GetHelpResourceResponse(request.Resources));
            }
        }
    }
}
