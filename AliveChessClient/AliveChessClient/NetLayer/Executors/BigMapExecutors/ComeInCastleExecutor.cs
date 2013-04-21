﻿using AliveChessClient.GameLayer;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Commands.BigMapCommand;
using AliveChessLibrary.GameObjects.Buildings;

namespace AliveChessClient.NetLayer.Executors.BigMapExecutors
{
    public class ComeInCastleExecutor : IExecutor
    {
        private Game context;
        private ComeInCastleHandler handler;
       // private DrawHandler draw;

        public ComeInCastleExecutor(Game context)
        {
            this.context = context;
            this.handler = new ComeInCastleHandler(context.GameForm.StartCastle);
           // this.draw = new DrawHandler(context.BigMap.GraphicManager.Draw);
        }

        public void Execute(ICommand cmd)
        {
            ComeInCastleResponse response = (ComeInCastleResponse)cmd;

            if (response.CastleId != 0)
            {
                context.Player.King.ComeInCastle(context.Player.King.GetCastleById(response.CastleId));
                context.GameForm.Invoke(handler, context.Player.King.GetCastleById(response.CastleId));
               // context.GameForm.Invoke(draw);
            }
        }

        public delegate void ComeInCastleHandler(Castle castle);
    }
}