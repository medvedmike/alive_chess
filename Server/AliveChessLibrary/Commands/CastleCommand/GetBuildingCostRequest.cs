﻿using AliveChessLibrary.GameObjects.Buildings;
using ProtoBuf;

namespace AliveChessLibrary.Commands.CastleCommand
{
    [ProtoContract]
    public class GetBuildingCostRequest : ICommand
    {
        [ProtoMember(1)]
        private InnerBuildingType _type;

        public InnerBuildingType InnerBuildingType
        {
            get { return _type; }
            set { _type = value; }
        }

        public Command Id
        {
            get { return Command.GetBuildingCostRequest; }
        }
    }
}
