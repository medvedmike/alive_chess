﻿using System.Collections.Generic;
using AliveChessLibrary.Commands.EmpireCommand;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.Utility;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.Environment.Aliances;
using AliveChessServer.LogicLayer.UsersManagement;
using AliveChessServer.NetLayer;

namespace AliveChessServer.LogicLayer.RequestExecutors.EmpireExecutors
{
    public class GetAlianceInfoExecutor : IExecutor
    {
        private GameWorld _environment;
        private PlayerManager _playerManager;
        private LevelRoutine _levelManager;

        public GetAlianceInfoExecutor(LogicEntryPoint gameLogic)
        {
            this._environment = gameLogic.Environment;
            this._playerManager = gameLogic.PlayerManager;
            this._levelManager = gameLogic.Environment.LevelManager;
        }

        public void Execute(Message msg)
        {
            GetAlianceInfoRequest request = (GetAlianceInfoRequest) msg.Command;
            Level level = _levelManager.GetLevelById(msg.Sender.LevelId);
            IAliance aliance = level.EmpireManager.GetAlianceByMember(msg.Sender.King);
            msg.Sender.King.State = KingState.GetInfo;
            msg.Sender.Messenger.SendNetworkMessage(
                new GetAlianceInfoResponse(aliance.Id, GetAlianceInfo(aliance)));
        }

        private List<GetAlianceInfoResponse.MemberInfo> GetAlianceInfo(IAliance aliance)
        {
            var result =
                new List<GetAlianceInfoResponse.MemberInfo>();

            aliance.Kings.ForEach(
                x =>
                    {
                        GetAlianceInfoResponse.MemberInfo member =
                            new GetAlianceInfoResponse.MemberInfo();
                        member.MemberId = x.Id;
                        member.MemberName = x.Name;
                        result.Add(member);
                    });
            return result;
        }

    }
}
