﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using AliveChess.GameLayer.LogicLayer.Executors;
using AliveChess.NetworkLayer;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Interfaces;

namespace AliveChess.GameLayer.LogicLayer
{
    public class RequestExecutor
    {
        private ILogger _logger;
        private readonly BackgroundWorker _executeWorker;
        private bool _running = false;
        private readonly CommandPool _commands;
        private readonly Dictionary<ExecutorType, IExecutor> _executors;

        public RequestExecutor(ILogger logger, CommandPool commands)
        {
            _logger = logger;
            _commands = commands;
            _executeWorker = new BackgroundWorker();
            _executors = new Dictionary<ExecutorType, IExecutor>();
            _executeWorker.DoWork += new DoWorkEventHandler(Execute);
            _executeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ExecutionStopped);

            CreateAuthorizeExecutors();
            CreateBigMapExecutors();
        }

        public void Start()
        {
            _running = true;
            _executeWorker.RunWorkerAsync();
        }

        private void Execute(object sender, DoWorkEventArgs e)
        {
            while (_running)
            {
                if (_commands.Count > 0)
                {
                    ICommand command = _commands.Dequeue();
                    _executors[(ExecutorType) command.Id].Execute(command);
                    Thread.Sleep(5);
                }
                else
                {
                    _commands.Wait();
                }
            }
        }

        private void ExecutionStopped(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void CreateAuthorizeExecutors()
        {
            _executors.Add(ExecutorType.AuthorizeResponse, new AuthorizeExecutor());
            _executors.Add(ExecutorType.GetGameStateResponse, new GameStateExecutor());
        }

        private void CreateBigMapExecutors()
        {
            _executors.Add(ExecutorType.GetMapResponse, new GetMapExecutor());
        }
    }
}
