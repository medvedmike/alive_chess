﻿using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Buildings;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.GameObjects.Objects;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessServer.LogicLayer.AI;
using AliveChessServer.LogicLayer.Environment;
using AliveChessServer.LogicLayer.Environment.Alliances;
using AliveChessServer.LogicLayer.UsersManagement;
using BehaviorAILibrary;

#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4952
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AliveChessServer.DBLayer
{
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ComponentModel;
    using System;


    public partial class AliveChessDataContext : System.Data.Linq.DataContext
    {

        private static System.Data.Linq.Mapping.MappingSource mappingSource =
            XmlMappingSource.FromUrl(@"..\..\DBLayer\LINQ\AliveChessMapping.config");

        #region Extensibility Method Definitions
        partial void OnCreated();
        #endregion

        public AliveChessDataContext(string connection) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public AliveChessDataContext(System.Data.IDbConnection connection) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public AliveChessDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public AliveChessDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        [Function(Name = "[dbo].[InsertKing]")]
        public int InsertKing(
            [Parameter(Name = "@mapID", DbType = "INT")]        int @mapID,    // идентификатор карты
            [Parameter(Name = "@playerID", DbType = "INT")]     int @playerID, // идентификатор игрока
            [Parameter(Name = "@name", DbType = "VARCHAR(20)")] string @name,     // имя игрока
            [Parameter(Name = "@X", DbType = "INT")]            int @X,        // координата X
            [Parameter(Name = "@Y", DbType = "INT")]            int @Y,        // координата Y
            [Parameter(Name = "@wayCost", DbType = "FLOAT")]    float @wayCost,  // стоимость прохождения
            [Parameter(Name = "@castleID", DbType = "INT")]     int @castleID  // идентификатор замка
            )
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                @mapID, @playerID, @name, @X, @Y, @wayCost, @castleID);
            return (int)result.ReturnValue;
        }

        public System.Data.Linq.Table<Castle> Castles
        {
            get { return this.GetTable<Castle>(); }
        }

        public System.Data.Linq.Table<InnerBuilding> InnerBuildings
        {
            get { return this.GetTable<InnerBuilding>(); }
        }

        public System.Data.Linq.Table<King> Kings
        {
            get { return this.GetTable<King>(); }
        }

        public System.Data.Linq.Table<Map> Maps
        {
            get { return this.GetTable<Map>(); }
        }

        public System.Data.Linq.Table<Mine> Mines
        {
            get { return this.GetTable<Mine>(); }
        }

        public System.Data.Linq.Table<User> Users
        {
            get { return this.GetTable<User>(); }
        }

        public System.Data.Linq.Table<Resource> Resources
        {
            get { return this.GetTable<Resource>(); }
        }

        public System.Data.Linq.Table<Unit> Units
        {
            get { return this.GetTable<Unit>(); }
        }

        public System.Data.Linq.Table<MultyObject> MultyObjects
        {
            get { return this.GetTable<MultyObject>(); }
        }

        public System.Data.Linq.Table<SingleObject> SingleObjects
        {
            get { return this.GetTable<SingleObject>(); }
        }

        public System.Data.Linq.Table<ResourceStore> ResourceStores
        {
            get { return this.GetTable<ResourceStore>(); }
        }

        /*public System.Data.Linq.Table<FigureStore> FigureStores
        {
            get { return this.GetTable<FigureStore>(); }
        }*/

        public System.Data.Linq.Table<Vicegerent> Vicegerenrs
        {
            get { return this.GetTable<Vicegerent>(); }
        }

        public System.Data.Linq.Table<Level> Levels
        {
            get { return this.GetTable<Level>(); }
        }

        /*public System.Data.Linq.Table<Store> Stores
        {
            get { return this.GetTable<Store>(); }
        }*/

        public System.Data.Linq.Table<Empire> Empires
        {
            get { return this.GetTable<Empire>(); }
        }

        public System.Data.Linq.Table<Union> Unions
        {
            get { return this.GetTable<Union>(); }
        }

        public System.Data.Linq.Table<Successor> Successors
        {
            get { return this.GetTable<Successor>(); }
        }

        public System.Data.Linq.Table<BasePoint> BasePoints
        {
            get { return this.GetTable<BasePoint>(); }
        }

        public System.Data.Linq.Table<Animat> Animats
        {
            get { return this.GetTable<Animat>(); }
        }

        public System.Data.Linq.Table<Border> Borders
        {
            get { return this.GetTable<Border>(); }
        }
    }
}
#pragma warning restore 1591
