﻿using System.Collections.Generic;
using AliveChessClient.GameLayer;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Commands.BigMapCommand;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Resources;

namespace AliveChessClient.NetLayer.Executors.BigMapExecutors
{
    public class CaptureCastleExecutor : IExecutor
    {
        private Game context;
        private CastleHandler handler;

        public CaptureCastleExecutor(Game context)
        {
            this.context = context;
            this.handler = new CastleHandler(context.GameForm.BigMapControl.UpdateCastlesCount);
        }

        public void Execute(ICommand cmd)
        {
            CaptureCastleResponse response = (CaptureCastleResponse)cmd;

            response.Castle.Map = context.Player.Map;
            context.Player.King.AddCastle(response.Castle);
            response.Castle.ResourceStore = new ResourceStore();

            if (!context.Player.IsSuperUser)
            {
               VisibleSpace sector = context.VisibleSpaceManager.GetVisibleSpace(context.Player.King, context.Player.IsSuperUser);
                context.Player.UpdateVisibleSpace(sector);
                List<MapPoint> array = sector.Sector;
                foreach (MapPoint mo in array)
                    mo.Detected = true;
            }

            context.GameForm.Invoke(handler, context.Player.King.Castles.Count);
        }

        public delegate void CastleHandler(int count);
    }
}
