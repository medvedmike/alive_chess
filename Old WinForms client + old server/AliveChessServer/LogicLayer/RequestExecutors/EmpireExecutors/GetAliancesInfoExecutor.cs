﻿using System.Collections.Generic;
using AliveChessLibrary.Commands.EmpireCommand;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.Environment.Aliances;
using AliveChessServer.LogicLayer.UsersManagement;
using AliveChessServer.NetLayer;

namespace AliveChessServer.LogicLayer.RequestExecutors.EmpireExecutors
{
    /// <summary>
    /// обработка запроса получения информации о союзах
    /// </summary>
    public class GetAliancesInfoExecutor : IExecutor
    {
        private GameWorld _environment;
        private PlayerManager _playerManager;
        private LevelRoutine _levelManager;

        public GetAliancesInfoExecutor(LogicEntryPoint gameLogic)
        {
            this._environment = gameLogic.Environment;
            this._playerManager = gameLogic.PlayerManager;
            this._levelManager = gameLogic.Environment.LevelManager;
        }

        public void Execute(Message msg)
        {
            GetAliancesInfoRequest request = (GetAliancesInfoRequest)msg.Command;
            Player player = msg.Sender;
            Level level = _levelManager.GetLevelById(player.LevelId);
            List<IAliance> aliances = level.EmpireManager.Aliances;
            player.Messenger.SendNetworkMessage(
                new GetAliancesInfoResponse(GetAliancesInfo(aliances, player.King.IsLeader)));
        }

        private List<GetAliancesInfoResponse.AlianceInfo> GetAliancesInfo(List<IAliance> aliances,
            bool withLeaders)
        {
            var result =
                new List<GetAliancesInfoResponse.AlianceInfo>();

            aliances.ForEach(
                x =>
                {
                    var a = new GetAliancesInfoResponse.AlianceInfo();

                    a.Id = x.Id;
                    a.Name = "";
                    if (withLeaders && x.Status == AlianceStatus.Empire)
                    {
                        Empire e = x as Empire;

                        var leader = new GetAlianceInfoResponse.MemberInfo();

                        leader.MemberId = e.Leader.Id;
                        leader.MemberName = e.Leader.Name;
                        a.Leader = leader;
                    }
                    result.Add(a);
                });
            return result;
        }
    }
}
