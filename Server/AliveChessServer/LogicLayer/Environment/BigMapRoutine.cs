﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using AliveChessLibrary.Commands.BigMapCommand;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessLibrary.Interfaces;
using AliveChessServer.LogicLayer.UsersManagement;

namespace AliveChessServer.LogicLayer.Environment
{
    public class BigMapRoutine : ITimeRoutine
    {
        private Map _map;
        private PlayerManager _playerManager;
        private TimeManager _timeManager;
        private AliveChessLogger _logger;
        //private object _mapSync = new object();

        public BigMapRoutine(Level level, TimeManager timeManager, AliveChessLogger logger)
        {
            Debug.Assert(level.Map != null);

            this._map = level.Map;
            this._timeManager = timeManager;
            this._logger = logger;
        }

        public static void InitializeLandscape(Map map)
        {
            BasePoint basePoint = new BasePoint();
            basePoint.X = 10;
            basePoint.Y = 10;
            basePoint.LandscapeType = LandscapeTypes.Grass;

            map.AddBasePoint(basePoint);
            map.Fill();
        }

        public void Update(GameTime time)
        {
            if (time.Elapsed > TimeSpan.FromMilliseconds(50))
            {
                _map.Update(time.Now);
                time.SavePreviosTimestamp();
            }
        }

        public void UpdatePointState(IMapObject sender, UpdateWorldEventArgs eventArgs)
        {
            foreach (King k in eventArgs.Map.NextKing())
            {
                if (!k.Equals(sender.Id) && k.Player.VisibleSpace.Contains(
                    eventArgs.Location.X, eventArgs.Location.Y))
                {
                    if (!k.Player.Bot && k.Player.Ready)
                        k.Player.Messenger.SendNetworkMessage(
                            new UpdateWorldMessage(sender, eventArgs.Location, eventArgs.UpdateType));
                }
            }
        }

        public void UpdateSectorState(King king)
        {
            var kings = GetGameObjects<King>(king.VisibleSpace, PointTypes.King, king);
            var resources = GetGameObjects<Resource>(king.VisibleSpace, PointTypes.Resource, king);

            king.Player.Messenger.SendNetworkMessage(new GetObjectsResponse(resources, kings));
        }

        #region Executors

        public void CollectResource(King player, MapPoint point)
        {
            Resource resource = player.Map.SearchResourceById(point.Owner.Id);
            if (resource != null)
            {
                resource.Disappear();
                player.Map.RemoveResource(resource);
                player.Player.Messenger.SendNetworkMessage(new GetResourceMessage(resource, false));
            }
        }

        #endregion

        public List<T> GetGameObjects<T>(IVisibleSpace sector,
          PointTypes type, IObserver forObject) where T : IMapObject
        {
            List<T> objects = new List<T>();
            foreach (MapPoint @object in sector.Walk())
            {
                if (@object.Owner.Id != forObject.Id)
                {
                    if (@object.PointType == type)
                    {
                        T owner = (T)@object.Owner;
                        if (!objects.Contains(owner))
                        {
                            objects.Add(owner);
                        }
                    }
                }
            }
            return objects;
        }

        public List<T> GetGameObjects<T>(IVisibleSpace sector,
            PointTypes type, Player forPlayer) where T : IMapObject
        {
            List<T> objects = new List<T>();
            foreach (MapPoint @object in sector.Walk())
            {
                if (@object.Owner.Id != forPlayer.King.Id)
                {
                    if (@object.PointType == type)
                    {
                        T owner = (T) @object.Owner;
                        if (!objects.Contains(owner))
                        {
                            objects.Add(owner);
                        }
                    }
                }
            }
            return objects;
        }

        public PlayerManager PlayerManager
        {
            get { return _playerManager; }
            set { _playerManager = value; }
        }
    }
}
