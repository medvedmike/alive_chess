﻿using AliveChessLibrary.GameObjects.Characters;
using ProtoBuf;

namespace AliveChessLibrary.Commands.CastleCommand
{
    [ProtoContract]
    public class CreateUnitRequest : ICommand
    {
        [ProtoMember(1)]
        private UnitType _unitType;

        public Command Id
        {
            get { return  Command.CreateUnitRequest; }
            
        }

        public UnitType UnitType
        {
            get { return _unitType; }
            set { _unitType = value; }
        }
    }
}
