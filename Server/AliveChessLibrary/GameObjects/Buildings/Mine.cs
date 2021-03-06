﻿using System;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.GameObjects.Resources;
using AliveChessLibrary.Interfaces;
using AliveChessLibrary.Utility;
using ProtoBuf;
using System.Data.Linq;

namespace AliveChessLibrary.GameObjects.Buildings
{
    /// <summary>
    /// шахта
    /// </summary>
    [ProtoContract]
    public class Mine : IBuilding, IActive, IEquatable<int>, IEquatable<MapPoint>, IMultyPoint
    {
        #region Variables

        [ProtoMember(1)]
        private int _mineId;

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
        [ProtoMember(7)]
        private Resource _miningResource;
        [ProtoMember(8)]
        private int _capacity;
        [ProtoMember(9)]
        private int? _kingId;

        private int _imageId;
        private VisibleSpace _sector;
        private ResourceTypes _mineType;

        //[ProtoMember(2)]
        private MapSector _viewOnMap; // сектор на карте

        private string _messengerMine; // сообщение от шахты
        //private ResourceStore _valutResurs; // указатель на Хранилище ресурсов
        private double _miningRate; // скорость добычи ресурсов(среднее количество ресурсов, производимых в секунду)
        private bool _active; // активна ли шахта
        private DateTime _lastMiningTime; // дата последенй работы шахты
        private double _realQuantity; // реальное количество добытого ресурса (для предотвращения потери дробной части при округлении во время пересылки в хранилище)

        private int? _mapId;
        private EntityRef<Map> _map; // ссылка на карту
        private EntityRef<King> _king; // ссылка на короля
        private int _distance = 3;

        private const int DEFAULT_CAPACITY = 1000;

        #endregion

        #region Constructors

        public Mine()
        {
            this._map = default(EntityRef<Map>);
            this._king = default(EntityRef<King>);
            _realQuantity = 0;
            _sector = new VisibleSpace(this);

            if (OnLoad != null)
                OnLoad(this);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// добавления на карту занимаемого сектора
        /// </summary>
        /// <param name="sector"></param>
        public void AddView(MapSector sector)
        {
            ViewOnMap = sector;
            sector.SetOwner(this);
        }

        /// <summary>
        /// удаление с карты сектора
        /// </summary>
        public void RemoveView()
        {
            ViewOnMap.SetOwner(null);
        }

        /// <summary>
        /// инициализация
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="sector">область арты</param>
        /// <param name="type">тип ресурса</param>
        /// <param name="miningRate">объем добычи в секунду</param>
        public void Initialize(Map map, MapSector sector, ResourceTypes type,
            double miningRate)
        {
            Initialize(map, type, miningRate);

            if (sector != null)
                AddView(sector);
        }

        /// <summary>
        /// инициализация
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="typeRes">тип ресурса</param>
        /// <param name="miningRate">объем добычи в секунду</param>
        public void Initialize(Map map, ResourceTypes typeRes,
            double miningRate)
        {
            this._map.Entity = map;
            this._mapId = map.Id;
            this._lastMiningTime = DateTime.Now;
            this._active = false; // активировать шахту
            this.MineType = typeRes;
            this._miningResource = new Resource();
            this._miningResource.ResourceType = typeRes;
            this._miningRate = miningRate;
            this._capacity = DEFAULT_CAPACITY;
        }

        /// <summary>
        /// инициализация
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <param name="map">карта</param>
        /// <param name="typeRes">тип ресурса</param>
        /// <param name="size">размер</param>
        /// <param name="miningRate">объем добычи в секунду</param>
        public void Initialize(int id, Map map, ResourceTypes typeRes,
            int size, double miningRate)
        {
            Initialize(map, typeRes, miningRate);
            this._capacity = size;
            this._mineId = id;
            this._lastMiningTime = DateTime.Now;
            this._active = false; // активировать шахту
        }

        /*/// <summary>
        /// инициализация
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <param name="map">карта</param>
        /// <param name="typeRes">тип ресурса</param>
        /// <param name="size">размер</param>
        /// <param name="miningRate">коэффициент активности</param>
        /// <param name="vault">назначенно хранилище</param>
        public void Initialize(int id, Map map, ResourceTypes typeRes,
            int size, float miningRate, ResourceStore vault)
        {
            this._valutResurs = vault;
            Initialize(id, map, typeRes, size, miningRate);
        }*/

        #endregion

        #region Methods

        /// <summary>
        /// назначаем владельца шахты
        /// </summary>
        /// <param name="king"></param>
        public void SetOwner(King king)
        {
            if (_king.Entity != null && king != null)
                throw new AliveChessException("Owner isn't null");
            else
            {
                _king.Entity = king;
                _kingId = king != null ? king.Id : -1;
            }
        }

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
        /// сравненеи по ячейке
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MapPoint other)
        {
            return Id.CompareTo(other.Owner.Id) == 0 ? true : false;
        }

        /// <summary>
        /// активация шахты
        /// </summary>
        public void Activate()
        {
            //Установить флажок Activate в положение true
            this._active = true;
            _lastMiningTime = DateTime.Now;
        }

        /// <summary>
        /// процесс работы шахты
        /// </summary>
        /// <param name="tmpDateTime"></param>
        public void DoWork(DateTime tmpDateTime)
        {
            // если шахта находится в рабочем состоянии
            if (this._active)
            {
                // найти разницу между текущим временем и временем последней работы шахты 
                TimeSpan difference = tmpDateTime - this._lastMiningTime;
                // рассчитать добытое за это время количество ресурса
                _realQuantity += difference.TotalSeconds * this._miningRate;

                // извлечь можно только целое количество ресурса
                int quantity = (int)Math.Floor(_realQuantity);
                
                // если есть, что извлекать
                if (quantity > 0)
                {
                    // добавить необходимое количество ресурса
                    this.AddResource(quantity);

                    // если у шахты есть владелец, вызывается обработчик передачи ресурса в его хранилище
                    if (King != null && GetResourceEvent != null)
                    {
                        GetResourceEvent(this.King, _miningResource, true);
                        _miningResource.Quantity = 0;
                    }

                    // сохранить новое время последней добычи
                    this._lastMiningTime = tmpDateTime;
                    // вычесть извлеченное количество ресурсов из промежуточного значения
                    _realQuantity -= quantity;

                    // если шахта переполнена
                    if (this.IsOverstocked())
                    {
                        // остановить работу шахты
                        this.Deactivatе();
                    }
                }
            }
        }

        /// <summary>
        /// деактивация шахты
        /// </summary>
        public void Deactivatе()
        {
            this._active = false;
        }

        /*/// <summary>
        /// присоединение хранилища ресурсов
        /// </summary>
        /// <param name="vault"></param>
        public void JoinVault(ResourceStore vault)
        {
            // Если шахта еще не имеет хранилища ресурсв
            if (!this.PresenceValutResurs())
            {
                //присоединить хранилище к шахте
                this._valutResurs = vault;
                // произвести перевод ресурсов из шахты в Хранилище
                this.TranslationResource();

            }
            // Если шахта уже имеет хранилище 
            else
            {
                //сформировать соответствующие сообщение шахты
                this._messegerMine = "Шахта уже имеет одно хранилище ресурсов";
            }
        }

        /// <summary>
        /// отсоединение хранилища ресурсов
        /// </summary>
        public void DisconnectValut()
        {
            // Если у шахты есть Хранилище ресурсов
            if (this.PresenceValutResurs())
            {
                //отсоединить это хранилище
                this._valutResurs = null;
            }
            // Если хранилища у шахты нет
            else
            {
                //вернуть соответствующее сообщение
                this._messegerMine = "У данной шахты нет своего хранилища ресурсов";
            }
        }*/


        /// <summary>
        /// создание ресурса
        /// </summary>
        /// <param name="amountResource"></param>
        public void AddResource(int amountResource)
        {
            /*Resource tmpRes = null;

            // Если имеется Хранилище ресурсов
            if (this.PresenceValutResurs())
            {
                //создать ресурс
                tmpRes = new Resource();
                tmpRes.ResourceType = _gainingResource.ResourceType;
                tmpRes.CountResource = amountResource;
                //передать в хранилище
                this._valutResurs.AddResourceToRepository(tmpRes);

            }
            // Если Хранилища ресурсов нет
            else*/
            {
                //Увеличить счетчик количества добытого ресурса в Шахте
                this._miningResource.Quantity += amountResource;
            }

        }

        /// <summary>
        /// получение количества ресурсов, хранящихся в шахте
        /// </summary>
        /// <returns></returns>
        public int GetResourceQuantity()
        {
            /*// Если у шахты есть Хранилище ресурсов
            if (this.PresenceValutResurs())
            {
                //ошибка, невозможно просмотреть количество ресурса, т.к ресурс передается в Хран рес
                return -1;

            }
            // Если Хранилища ресурсов у шахты нет
            else*/
            {
                //вернуть количество ресурса в шахте
                return this._miningResource.Quantity;
            }
        }

        /// <summary>
        /// проверка шахты на переполнение
        /// </summary>
        /// <returns></returns>
        private bool IsOverstocked()
        {
            // Если размер ресурса превышает максимальный размер шахты
            if (this._miningResource.Quantity >= this._capacity)
                return true;
            else
                return false;
        }

        /*/// <summary>
        /// проверка наличия хранилища ресурсов
        /// </summary>
        /// <returns></returns>
        private bool PresenceValutResurs()
        {
            // Проверить присутствие Хранилища ресурсов
            if (this._valutResurs != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// отправка ресурсов из шахты в хранилище
        /// </summary>
        private void TranslationResource()
        {
            // передать ресурс из шахты в Хранилище 
            this._valutResurs.AddResourceToRepository(this._gainingResource);
            // обнулить количества ресурсов в шахте
            this._gainingResource.CountResource = 0;
        }*/

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
        /// тип ячейки
        /// </summary>
        public PointTypes Type
        {
            get { return PointTypes.Mine; }
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
        /// получаем игрока владеющего шахтой либо null
        /// </summary>
        public IPlayer Player
        {
            get
            {
                if (King != null)
                    return King.Player;
                else return null;
            }
        }

        /// <summary>
        /// тип здания
        /// </summary>
        public BuildingTypes BuildingType
        {
            get { return BuildingTypes.Mine; }
        }

        public string MessegerMine
        {
            get { return _messengerMine; }
            set { _messengerMine = value; }
        }

        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        public Resource MiningResource
        {
            get { return _miningResource; }
            set { _miningResource = value; }
        }

        /*public ResourceStore ValutResurs
        {
            get { return _valutResurs; }
            set { _valutResurs = value; }
        }*/

        public double MiningRate
        {
            get { return _miningRate; }
            set { _miningRate = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public DateTime DateLastWorkMine
        {
            get { return _lastMiningTime; }
            set { _lastMiningTime = value; }
        }

        /// <summary>
        /// область видимости
        /// </summary>
        public VisibleSpace VisibleSpace
        {
            get { return _sector; }
            set { _sector = value; }
        }

        public MapSector ViewOnMap
        {
            get { return _viewOnMap; }
            set { _viewOnMap = value; }
        }

        public int Id
        {
            get
            {
                return this._mineId;
            }
            set
            {
                if (this._mineId != value)
                {
                    this._mineId = value;
                }
            }
        }


        public int? MapId
        {
            get
            {
                return this._mapId;
            }
            set
            {
                if (this._mapId != value)
                {
                    if (this._map.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this._mapId = value;
                }
            }
        }

        public ResourceTypes MineType
        {
            get
            {
                return _miningResource.ResourceType;
            }
            set
            {
                if (_miningResource == null || _miningResource.ResourceType != value)
                {
                    this._miningResource = new Resource();
                    this._miningResource.ResourceType = value;
                    //this._mineType = value;
                }
            }
        }


        public int? KingId
        {
            get
            {
                return this._kingId;
            }
            set
            {
                if (this._kingId != value)
                {
                    if (this._king.HasLoadedOrAssignedValue)
                    {
                        throw new ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this._kingId = value;
                }
            }
        }

        public King King
        {
            get
            {
                return this._king.Entity;
            }
            set
            {
                if (_king.Entity != value)
                {
                    if (_king.Entity != null)
                    {
                        var previousKing = _king.Entity;
                        _king.Entity = null;
                        previousKing.Mines.Remove(this);
                    }
                    _king.Entity = value;
                    if (value != null)
                    {
                        value.Mines.Add(this);
                        _kingId = value.Id;
                    }
                    else
                    {
                        _kingId = null;
                    }
                }
            }
        }

        public Map Map
        {
            get
            {
                return this._map.Entity;
            }
            set
            {
                if (_map.Entity != value)
                {
                    if (_map.Entity != null)
                    {
                        var previousMap = _map.Entity;
                        _map.Entity = null;
                        previousMap.Mines.Remove(this);
                    }
                    _map.Entity = value;
                    if (value != null)
                    {
                        _mapId = value.Id;
                    }
                    else
                    {
                        _mapId = null;
                    }
                }
            }
        }
        #endregion

        #region Delegates

        public delegate void GetResourceHandler(King player, Resource r, bool fromMine);

        #endregion

        #region Events

        public static event LoadingHandler<Mine> OnLoad;
        public event GetResourceHandler GetResourceEvent;

        #endregion
    }
}
