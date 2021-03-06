﻿using AliveChessClient.GameLayer;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Commands.DialogCommand;
using AliveChessLibrary.Interaction;

namespace AliveChessClient.NetLayer.Executors.DialogExecutors
{
    public class MessageCastleExecutor : IExecutor
    {
        private Game context;
        private DisputeHandler handler;

        public MessageCastleExecutor(Game context)
        {
            this.context = context;
        }

        public void Execute(ICommand cmd)
        {
            CaptureCastleDialogMessage msg = (CaptureCastleDialogMessage)cmd;

            if (msg.State == DialogState.Offer)
            {
                handler = new DisputeHandler(context.GameForm.StartSimpleCaptureCastleDialog);
                context.GameForm.Invoke(handler);
            }
            else if (msg.State == DialogState.Refuse)
            {
                handler = new DisputeHandler(context.GameForm.CastleInfoControl.RefuseCaptureAnswer);
                context.GameForm.Invoke(handler);
                context.Dispute = null;
            }
            else if (msg.State == DialogState.Agree)
            {
                handler = new DisputeHandler(context.GameForm.CastleInfoControl.AgreeCaptureAnswer);
                context.GameForm.Invoke(handler);
                context.Dispute = null;
            }
        }

        private delegate void DisputeHandler();
    }
}
