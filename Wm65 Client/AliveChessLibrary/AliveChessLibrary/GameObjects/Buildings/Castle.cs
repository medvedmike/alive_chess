﻿using System;
using System.Collections.Generic;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessLibrary.Interfaces;
using AliveChessLibrary.Utility;
using ProtoBuf;


namespace AliveChessLibrary.GameObjects.Buildings
{
    /// <summary>
    /// замок
    /// </summary>
    [ProtoContract]
    public class Castle : IBuilding, IEquatable<int>, IEquatable<MapPoint>, IMultyPoint
    {
        #region Variables

        [ProtoMember(1)]
        private int _castleId;
        [ProtoMember(2)]
        private int _leftX;
        [ProtoMember(3)]
        private int _topY;
        [ProtoMember(4)]
        private int _width;
        [ProtoMember(5)]
        private int _height;
        [ProtoMember(6)]
        private float _wayCost;

        private int _imageId;
        private bool _kingInside;
        private VisibleSpace _visibleSector;
        private bool _isAttached = false;

        private MapSector _viewOnMap; // сектор на карте

        private int? _mapId;
        private int? _kingId;
        private int _resourceVaultId;
        private int _figureVaultId;


        private Map _map; // ссылка на карту
        private King _king; // ссылка на короля
        private Vicegerent _vicegerent; // наместник
        private List<InnerBuilding> _innerBuildings; // список внутренних строений
        private ResourceStore _resourceStore;
        private FigureStore _figureStore;
        private int _distance = 5;
       
        ///Slisarenko
        //фабрика юнитов
        private UnitFacrory _factory;
        //для призыва пешек
        private InnerBuilding _recruitmentOffice;
        //фабрика постройки зданий
        private InnerBuildingFactory Fabric = new InnerBuildingFactory();
        //Армия в замке
        //private List<Unit> _army = new List<Unit>();

        //список ресурсов для постройки  
        //private List<IInnerBuilding> list = new List<IInnerBuilding>();

        ///Slisarenko

        #endregion

        #region Constructors

        public Castle()
        {
            this.Map = null;
            this.King = null;
            this.Vicegerent = null;
            this.FigureStore = null;
            this.ResourceStore = null;
            this.InnerBuildings = new List<InnerBuilding>();
            _visibleSector = new VisibleSpace(this);

            if (OnLoad != null)
                OnLoad(this);
        }

        public Castle(Guid id, Map map)
            : this()
        {
            Initialize(id, map);
        }

        #endregion

        #region Initialization
       
        public void Initialize(Map map)
        {
            this.Map = map;
            this._factory = new UnitFacrory();
        }

        public void Initialize(Guid id, Map map)
        {
            this.Map = map;
            this._factory = new UnitFacrory();
            _recruitmentOffice = new InnerBuilding();
            _recruitmentOffice.ProducedUnitType = UnitType.Pawn;
            _recruitmentOffice.InnerBuildingType = InnerBuildingType.Voencomat;
            _recruitmentOffice.Name = "Recruitment office";
            InnerBuildings.Add(_recruitmentOffice);
        }

        /// <summary>
        /// добавление представления на карту
        /// </summary>
        /// <param name="sector"></param>
        public void AddView(MapSector sector)
        {
            this.ViewOnMap = sector;
            foreach (MapPoint mp in sector.MapPoints)
                mp.SetOwner(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// проверка принадлежности замка королю
        /// </summary>
        /// <param name="king"></param>
        /// <returns></returns>
        public bool IsBelongTo(King king)
        {
            return this.King == king;
        }

      
        public int SizeListbuilldingsInCastle()
        {
            return InnerBuildings.Count;
        }

       // Создание юнита и отправка в армию
        public void CreateUnitAndAddInArmy(int count, UnitType type)
        {
            for (int i = 0; i < InnerBuildings.Count; i++)
            {
                if (InnerBuildings[i].ProducedUnitType == type)
                {
                    AddInArmy(InnerBuildings[i].CreateUnit(count, type));
                }
            }
        }

        //Добавление в армию замка
        private void AddInArmy(Unit un)
        {
            bool t = false;
            for (int i = 0; i < FigureStore.Units.Count; i++)
            {
                if (FigureStore.Units[i].UnitType == un.UnitType)
                {
                    FigureStore.Units[i].UnitCount += un.UnitCount;
                    t = true;
                    break;
                }
            }
            if (!t) FigureStore.Units.Add(un);
        }

        public bool test_res(int[] f)
        {
            if (f[0] > Vicegerent.Castle.ResourceStore.GetResourceCountInRepository(Resources.ResourceTypes.Coal))
                return false;
            if (f[1] > Vicegerent.Castle.ResourceStore.GetResourceCountInRepository(Resources.ResourceTypes.Gold))
                return false;
            if (f[2] > Vicegerent.Castle.ResourceStore.GetResourceCountInRepository(Resources.ResourceTypes.Iron))
                return false;
            if (f[3] > Vicegerent.Castle.ResourceStore.GetResourceCountInRepository(Resources.ResourceTypes.Stone))
                return false;
            if (f[4] > Vicegerent.Castle.ResourceStore.GetResourceCountInRepository(Resources.ResourceTypes.Wood))
                return false;
            return true;
        }

        //передать армию замка королю
        public void GetArmyToKing()
        {
            bool t = false;
            for (int j = 0; j < FigureStore.Units.Count; j++)
            {
                for (int i = 0; i < King.Units.Count; i++)
                {
                    if (FigureStore.Units[j].UnitType == King.Units[i].UnitType)
                    {
                        King.Units[i].UnitCount += FigureStore.Units[j].UnitCount;
                        t = true;
                        break;

                    }
                }
                if (!t)
                {
                    King.Units.Add(FigureStore.Units[j]);
                    t = false;
                }
            }
            FigureStore.Units.Clear();
        }
       
        public InnerBuilding GetBuildings(int i)
        {
            return InnerBuildings[i];
        }

        public void AddBuildings(InnerBuildingType type)
        {
            //_innerBuildings.Add(Fabric.Build(pair.Guid, pair.Id, type, type.ToString(), _gameData));
        }

        //public List<Unit> ArmyInsideCastle
        //{
        //    get { return FigureStore.Units.ToList(); }
        //}

        public InnerBuilding RecruitmentOffice
        {
            get { return _recruitmentOffice; }
        }

        /// <summary>
        /// Создание начальной армии 
        /// </summary>
        public void CreatStartArmy()
        {
            //GuidIDPair pair = generator.Invoke();
            //int cost = Convert.ToInt32(CostUnit.One);
            //Unit p = _factory.Create(pair.Guid, pair.Id, 8, UnitType.Pawn);
            //AddUnit(p, _figureStore.Entity.Units);

            //pair = generator.Invoke();
            //cost = Convert.ToInt32(CostUnit.One);
            //p = _factory.Create(pair.Guid, pair.Id, 2, UnitType.Bishop);
            //AddUnit(p, _figureStore.Entity.Units);

            //pair = generator.Invoke();
            //cost = Convert.ToInt32(CostUnit.One);
            //p = _factory.Create(pair.Guid, pair.Id, 2, UnitType.Knight);
            //AddUnit(p, _figureStore.Entity.Units);

            //pair = generator.Invoke();
            //cost = Convert.ToInt32(CostUnit.One);
            //p = _factory.Create(pair.Guid, pair.Id, 1, UnitType.Queen);
            //AddUnit(p, _figureStore.Entity.Units);

            //pair = generator.Invoke();
            //cost = Convert.ToInt32(CostUnit.One);
            //p = _factory.Create(pair.Guid, pair.Id, 2, UnitType.Rook);
            //AddUnit(p, _figureStore.Entity.Units);
        }

        /// <summary>
        ///добавление юнита 
        /// </summary>
        /// <param name="un"></param>
        /// <param name="arm"></param>
        public void AddUnit(Unit un, IList<Unit> arm)
        {
            bool ok = true;
            for (int i = 0; i < FigureStore.Units.Count; i++)
            {
                if (un.Id == arm[i].Id)
                {
                    arm[i].UnitCount++;
                    ok = false;
                    break;
                }

            }
            if (ok) arm.Add(un);
        }
        //Slisarenko

        /// <summary>
        /// сравнение по идентификатору
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(int other)
        {
            return Id.CompareTo(other) == 0 ? true : false;
        }

        /// <summary>
        /// сравнение ячеек
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MapPoint other)
        {
            return Id.CompareTo(other.Owner.Id) == 0 ? true : false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// координала левого верхнего угла по X
        /// </summary>
        public int X
        {
            get { return _leftX; }
            set { _leftX = value; }
        }

        /// <summary>
        /// координала левого верхнего угла по Y
        /// </summary>
        public int Y
        {
            get { return _topY; }
            set { _topY = value; }
        }

        /// <summary>
        /// размер по X
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// размер по Y
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// тип ячейки
        /// </summary>
        public PointTypes Type
        {
            get { return PointTypes.Castle; }
        }

        /// <summary>
        /// идентификатор картинки
        /// </summary>
        public int ImageId
        {
            get { return _imageId; }
            set { _imageId = value; }
        }

        /// <summary>
        /// стоимость прохождения
        /// </summary>
        public float WayCost
        {
            get { return _wayCost; }
            set { _wayCost = value; }
        }

        /// <summary>
        /// проверка замка на свободность
        /// </summary>
        public bool IsFree
        {
            get { return !_kingId.HasValue; }
        }

        /// <summary>
        /// получаем игрока владеющего замком либо null
        /// </summary>
        public IPlayer Player
        {
            get 
            {
                return King != null ? King.Player : null;                   
            }
        }

        /// <summary>
        /// флаг нахождения короля внутри замка
        /// </summary>
        public bool KingInside
        {
            get { return _kingInside; }
            set { _kingInside = value; }
        }

        /// <summary>
        /// дистанция видимости
        /// </summary>
        public int Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        /// <summary>
        /// область видимости
        /// </summary>
        public VisibleSpace VisibleSpace
        {
            get { return _visibleSector; }
            set { _visibleSector = value; }
        }

        /// <summary>
        /// тип здания
        /// </summary>
        public BuildingTypes BuildingType
        {
            get { return BuildingTypes.Castle; }
        }

        /// <summary>
        /// сектор на карте
        /// </summary>
        public MapSector ViewOnMap
        {
            get { return _viewOnMap; }
            set { _viewOnMap = value; }
        }

        /// <summary>
        /// замок назначен королю
        /// </summary>
        public bool IsAttached
        {
            get { return _isAttached; }
            set { _isAttached = value; }
        }

        public Vicegerent Vicegerent
        {
            get { return _vicegerent; }
            set { _vicegerent = value; }
        }

        public ResourceStore ResourceStore
        {
            get { return _resourceStore; }
            set { _resourceStore = value; }
        }

        public FigureStore FigureStore
        {
            get { return _figureStore; }
            set { _figureStore = value; }
        }

        /// <summary>
        /// идентификатор замка
        /// </summary>
        public int Id
        {
            get
            {
                return this._castleId;
            }
            set
            {
                if (this._castleId != value)
                {
                    this._castleId = value;
                }
            }
        }

        public King King
        {
            get { return _king; }
            set { _king = value; }
        }

        public Map Map
        {
            get { return _map; }
            set { _map = value; }
        }

        public List<InnerBuilding> InnerBuildings
        {
            get { return _innerBuildings; }
            set { _innerBuildings = value; }
        }
        #endregion

        public static event LoadingHandler<Castle> OnLoad;
        public event DeferredLoadingHandler<Castle> OnDeferredLoadingFigureStore;
        public event DeferredLoadingHandler<Castle> OnDeferredLoadingResourceStore;
    }
}
