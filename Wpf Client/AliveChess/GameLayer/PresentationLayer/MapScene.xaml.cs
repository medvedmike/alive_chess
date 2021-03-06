﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AliveChess.GameLayer.LogicLayer;
using AliveChessLibrary.Commands.BigMapCommand;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.GameObjects.Objects;
using AliveChessLibrary.GameObjects.Resources;


namespace AliveChess.GameLayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for MapScene.xaml
    /// </summary>
    public partial class MapScene : GameScene
    {
        private readonly GameWorld _world = GameCore.Instance.World;
        private readonly Player _player = GameCore.Instance.Player;

        private Rectangle[,] _groundRectangles;
        private Rectangle[,] _landscapeRectangles;
        private Rectangle[,] _buildingRectangles;
        private Rectangle[,] _dynamicObjectRectangles;

        private const int _width = 50;
        private const int _height = _width;
        private bool _rectanglesInitialized;

        private bool _kingSelected;
        private bool _followingKing;

        private ImageBrush _brushKing;
        private ImageBrush _brushCastle;
        private Dictionary<LandscapeTypes, ImageBrush> _groundBrushes = new Dictionary<LandscapeTypes, ImageBrush>();
        private Dictionary<SingleObjectType, ImageBrush> _landscapeBrushes = new Dictionary<SingleObjectType, ImageBrush>();
        private Dictionary<ResourceTypes, ImageBrush> _mineBrushes = new Dictionary<ResourceTypes, ImageBrush>();
        private Dictionary<ResourceTypes, ImageBrush> _resourceBrushes = new Dictionary<ResourceTypes, ImageBrush>();
        private DropShadowEffect _enemyLighting = new DropShadowEffect();
        private DropShadowEffect _playerLighting = new DropShadowEffect();
        private DropShadowEffect _selectionLighting = new DropShadowEffect();

        DispatcherTimer timerUpdate = new DispatcherTimer();
        private BigMapCommandController _bigMapCommandController;

        public MapScene()
        {
            InitializeComponent();
            this.ShowsNavigationUI = false;
            scrollViewerMap.CanContentScroll = false;
            InitBrushes();
            _bigMapCommandController = GameCore.Instance.BigMapCommandController;
            /*Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(ConnectCallback), true);
            ResponceComplete.responceComplete += new ResponceCompleteDelegate(ResponceComplete_responceComplete);*/
            _bigMapCommandController.SendGetMapRequest();
            _bigMapCommandController.SendBigMapRequest();
            _bigMapCommandController.StartGameStateUpdate();
            _bigMapCommandController.StartObjectsUpdate();
            timerUpdate.Tick += new EventHandler(timerUpdate_Tick);
            timerUpdate.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timerUpdate.Start();
            /*scrollViewerMap.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerMap.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;*/
            //_bigMapCommandController.SendGetMapRequest();
        }
        /*private delegate void MyDelegate();
        private event MyDelegate MyEvent;

        void ResponceComplete_responceComplete()
        {
            if (MyEvent != null)
                MyEvent();
        }

        public void ConnectCallback(bool isConnected)
        {
            if(NavigationService != null)
                NavigationService.RemoveBackEntry();
            _bigMapCommandController.SendGetMapRequest();
        }

        private void SceneMap_Loaded(object sender, RoutedEventArgs e)
        {
            if (_rectanglesInitialized == false)
            {
                ConnectCallback(true);
            }
        }*/
        void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (_bigMapCommandController.KingInCastle)
            {
                EnterCastle();
                return;
            }
            if (_bigMapCommandController.MapModified)
            {
                InitRectangles();
                DrawGround();
                AddRectanglesToCanvas();
                DrawLandscape();
                _bigMapCommandController.MapModified = false;
                _followingKing = true;
                _kingSelected = true;
            }
            if (_bigMapCommandController.BuildingsModified)
            {
                DrawBuildings();
                _bigMapCommandController.BuildingsModified = false;
            }
            if (_bigMapCommandController.DynamicObjectsModified)
            {
                DrawDynamicObjects();
                _bigMapCommandController.DynamicObjectsModified = false;
            }
            if (_bigMapCommandController.ResourcesModified)
            {
                UpdateResources();
                _bigMapCommandController.ResourcesModified = false;
            }
            if (_followingKing)
                KingToFocus();
        }


        public void InitBrushes()
        {
            BitmapImage bmKing = new BitmapImage(new Uri(@"Resources\king.png", UriKind.RelativeOrAbsolute));
            _brushKing = new ImageBrush(bmKing);

            BitmapImage bmCastle = new BitmapImage(new Uri(@"Resources\castle.png", UriKind.RelativeOrAbsolute));
            _brushCastle = new ImageBrush(bmCastle);

            BitmapImage bmGrass = new BitmapImage(new Uri(@"Resources\grass.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmSnow = new BitmapImage(new Uri(@"Resources\snow.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmGround = new BitmapImage(new Uri(@"Resources\ground.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmNone = new BitmapImage(new Uri(@"Resources\none.png", UriKind.RelativeOrAbsolute));
            ImageBrush grass = new ImageBrush(bmGrass);
            grass.Stretch = Stretch.UniformToFill;
            _groundBrushes[LandscapeTypes.Grass] = grass;
            _groundBrushes[LandscapeTypes.Snow] = new ImageBrush(bmSnow);
            _groundBrushes[LandscapeTypes.Ground] = new ImageBrush(bmGround);
            _groundBrushes[LandscapeTypes.None] = new ImageBrush(bmNone);

            //Одиночное дерево
            BitmapImage bmTree = new BitmapImage(new Uri(@"Resources\tree.png", UriKind.RelativeOrAbsolute));
            //Скала (непроходимая)
            BitmapImage bmRock = new BitmapImage(new Uri(@"Resources\rock.png", UriKind.RelativeOrAbsolute));
            _landscapeBrushes[AliveChessLibrary.GameObjects.Objects.SingleObjectType.Obstacle] =
                new ImageBrush(bmRock);
            _landscapeBrushes[AliveChessLibrary.GameObjects.Objects.SingleObjectType.Tree] =
                new ImageBrush(bmTree);

            BitmapImage bmGoldMine = new BitmapImage(new Uri(@"Resources\goldmine.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmCoalMine = new BitmapImage(new Uri(@"Resources\coalmine.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmIronMine = new BitmapImage(new Uri(@"Resources\ironmine.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmQuarry = new BitmapImage(new Uri(@"Resources\quarry.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmSawMill = new BitmapImage(new Uri(@"Resources\sawmill.png", UriKind.RelativeOrAbsolute));
            _mineBrushes[ResourceTypes.Gold] = new ImageBrush(bmGoldMine);
            _mineBrushes[ResourceTypes.Coal] = new ImageBrush(bmCoalMine);
            _mineBrushes[ResourceTypes.Iron] = new ImageBrush(bmIronMine);
            _mineBrushes[ResourceTypes.Stone] = new ImageBrush(bmQuarry);
            _mineBrushes[ResourceTypes.Wood] = new ImageBrush(bmSawMill);

            BitmapImage bmGold = new BitmapImage(new Uri(@"Resources\gold.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmCoal = new BitmapImage(new Uri(@"Resources\coal.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmIron = new BitmapImage(new Uri(@"Resources\iron.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmStone = new BitmapImage(new Uri(@"Resources\stone.png", UriKind.RelativeOrAbsolute));
            BitmapImage bmWood = new BitmapImage(new Uri(@"Resources\wood.png", UriKind.RelativeOrAbsolute));
            _resourceBrushes[ResourceTypes.Gold] = new ImageBrush(bmGold);
            _resourceBrushes[ResourceTypes.Coal] = new ImageBrush(bmCoal);
            _resourceBrushes[ResourceTypes.Iron] = new ImageBrush(bmIron);
            _resourceBrushes[ResourceTypes.Stone] = new ImageBrush(bmStone);
            _resourceBrushes[ResourceTypes.Wood] = new ImageBrush(bmWood);

            _enemyLighting.BlurRadius = 10;
            _enemyLighting.ShadowDepth = 5;
            _enemyLighting.Color = Colors.OrangeRed;
            _playerLighting.BlurRadius = 10;
            _playerLighting.ShadowDepth = 5;
            _playerLighting.Color = Colors.Honeydew;
            _selectionLighting.BlurRadius = 10;
            _selectionLighting.ShadowDepth = 5;
            _selectionLighting.Color = Colors.Gold;
        }

        private void InitRectangles()
        {
            _groundRectangles = new Rectangle[_world.Map.SizeX, _world.Map.SizeY];
            _landscapeRectangles = new Rectangle[_world.Map.SizeX, _world.Map.SizeY];
            _buildingRectangles = new Rectangle[_world.Map.SizeX, _world.Map.SizeY];
            _dynamicObjectRectangles = new Rectangle[_world.Map.SizeX, _world.Map.SizeY];
            canvasGround.Width = _width * _groundRectangles.GetLength(0);
            canvasGround.Height = _height * _groundRectangles.GetLength(1);
            canvasLandscape.Width = _width * _landscapeRectangles.GetLength(0);
            canvasLandscape.Height = _height * _landscapeRectangles.GetLength(1);
            canvasBuildings.Width = _width * _buildingRectangles.GetLength(0);
            canvasBuildings.Height = _height * _buildingRectangles.GetLength(1);
            canvasDynamicObjects.Width = _width * _dynamicObjectRectangles.GetLength(0);
            canvasDynamicObjects.Height = _height * _dynamicObjectRectangles.GetLength(1);
            for (int i = 0; i < _world.Map.SizeX; i++)
            {
                for (int j = 0; j < _world.Map.SizeY; j++)
                {
                    _groundRectangles[i, j] = null;
                }
                for (int k = 0; k < _world.Map.SizeY; k++)
                {
                    Rectangle r = new Rectangle();
                    r.CacheMode = new BitmapCache();
                    r.Height = _height;
                    r.Width = _width;
                    r.Fill = Brushes.Transparent;
                    TranslateTransform t = new TranslateTransform();
                    t.X = i * _width;
                    t.Y = k * _height;
                    r.RenderTransform = t;
                    _landscapeRectangles[i, k] = r;
                }
                for (int k = 0; k < _world.Map.SizeY; k++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = _height;
                    r.Width = _width;
                    r.Fill = Brushes.Transparent;
                    TranslateTransform t = new TranslateTransform();
                    t.X = i * _width;
                    t.Y = k * _height;
                    r.RenderTransform = t;
                    _buildingRectangles[i, k] = r;
                }
                for (int k = 0; k < _world.Map.SizeY; k++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = _height;
                    r.Width = _width;
                    r.Fill = Brushes.Transparent;
                    TranslateTransform t = new TranslateTransform();
                    t.X = i * _width;
                    t.Y = k * _height;
                    r.RenderTransform = t;
                    _dynamicObjectRectangles[i, k] = r;
                }
            }
            _rectanglesInitialized = true;
        }

        private void AddRectanglesToCanvas()
        {
            for (int i = 0; i < _world.Map.SizeX; i++)
            {
                for (int j = 0; j < _world.Map.SizeY; j++)
                {
                    if (_groundRectangles[i, j] != null)
                        canvasGround.Children.Add(_groundRectangles[i, j]);
                    canvasLandscape.Children.Add(_landscapeRectangles[i, j]);
                    canvasBuildings.Children.Add(_buildingRectangles[i, j]);
                    canvasDynamicObjects.Children.Add(_dynamicObjectRectangles[i, j]);
                }
            }
        }

        public void ClearRectangles(Rectangle[,] rectangles)
        {
            for (int i = 0; i < rectangles.GetLength(0); i++)
            {
                for (int j = 0; j < rectangles.GetLength(1); j++)
                {
                    if (rectangles[i, j] != null)
                    {
                        rectangles[i, j].Fill = Brushes.Transparent;
                        rectangles[i, j].Effect = null;
                    }
                }
            }
        }

        private MapPoint _getPoint(int x, int y)
        {
            return this._world.Map[x, y];
        }

        private Rectangle _createLandscapeRectangle(int X, int Y, LandscapeTypes type)
        {
            Rectangle r = new Rectangle();
            r.Height = _height + 1;
            r.Width = _width + 1;
            r.Fill = _groundBrushes[type];
            TranslateTransform t = new TranslateTransform();
            t.X = X * _width;
            t.Y = Y * _height;
            r.RenderTransform = t;
            return r;
        }

        private void KingToFocus()
        {
            int deltaX = (int)(scrollViewerMap.ActualWidth / 10);
            int deltaY = (int)(scrollViewerMap.ActualHeight / 10);
            int delta = deltaX > deltaY ? deltaY : deltaX;
            int border = 50;
            if (_player.King.X * _width >= scrollViewerMap.HorizontalOffset + scrollViewerMap.ActualWidth - 2 * delta - border)
                scrollViewerMap.ScrollToHorizontalOffset(_player.King.X * _width - scrollViewerMap.ActualWidth + 4 * delta + border);
            if (_player.King.X * _width <= scrollViewerMap.HorizontalOffset + 2 * delta)
                scrollViewerMap.ScrollToHorizontalOffset(_player.King.X * _width - 4 * delta);
            if (_player.King.Y * _height >= scrollViewerMap.VerticalOffset + scrollViewerMap.ActualHeight - 2 * delta - border)
                scrollViewerMap.ScrollToVerticalOffset(_player.King.Y * _height - scrollViewerMap.ActualHeight + 4 * delta + border);
            if (_player.King.Y * _height <= scrollViewerMap.VerticalOffset + 2 * delta)
                scrollViewerMap.ScrollToVerticalOffset(_player.King.Y * _height - 4 * delta);
            /*{
                scrollViewerMap.ScrollToHorizontalOffset(_player.King.X * width + width / 2 - scrollViewerMap.ActualWidth / 2);
                scrollViewerMap.ScrollToVerticalOffset(_player.King.Y * width + width / 2 - scrollViewerMap.ActualHeight / 2);
                _followingKing = true;
            }*/
        }

        public void DrawGround()
        {
            if (!_rectanglesInitialized)
                InitRectangles();

            lock (GameCore.Instance.World.Map.BasePoints)
            {
                foreach (var basePoint in _world.Map.BasePoints)
                {
                    _groundRectangles[basePoint.X, basePoint.Y] = _createLandscapeRectangle(basePoint.X, basePoint.Y,
                                                                                            basePoint.LandscapeType);
                }

                foreach (var basePoint in _world.Map.BasePoints)
                {
                    Queue<MapPoint> cells = new Queue<MapPoint>();
                    cells.Enqueue(_getPoint(basePoint.X, basePoint.Y));

                    while (cells.Count > 0)
                    {
                        MapPoint landscape = cells.Dequeue();

                        if (landscape.X > 0 && landscape.X < _world.Map.SizeX &&
                            _groundRectangles[landscape.X - 1, landscape.Y] == null)
                        {
                            cells.Enqueue(_getPoint(landscape.X - 1, landscape.Y));
                            _groundRectangles[landscape.X - 1, landscape.Y] = _createLandscapeRectangle(landscape.X - 1, landscape.Y, basePoint.LandscapeType);
                        }
                        if (landscape.X < _world.Map.SizeX - 1 &&
                            _groundRectangles[landscape.X + 1, landscape.Y] == null)
                        {
                            cells.Enqueue(_getPoint(landscape.X + 1, landscape.Y));
                            _groundRectangles[landscape.X + 1, landscape.Y] = _createLandscapeRectangle(landscape.X + 1, landscape.Y, basePoint.LandscapeType);
                        }
                        if (landscape.Y > 0 && landscape.Y < _world.Map.SizeY &&
                            _groundRectangles[landscape.X, landscape.Y - 1] == null)
                        {
                            cells.Enqueue(_getPoint(landscape.X, landscape.Y - 1));
                            _groundRectangles[landscape.X, landscape.Y - 1] = _createLandscapeRectangle(landscape.X, landscape.Y - 1, basePoint.LandscapeType);
                        }
                        if (landscape.Y < _world.Map.SizeY - 1 &&
                            _groundRectangles[landscape.X, landscape.Y + 1] == null)
                        {
                            cells.Enqueue(_getPoint(landscape.X, landscape.Y + 1));
                            _groundRectangles[landscape.X, landscape.Y + 1] = _createLandscapeRectangle(landscape.X, landscape.Y + 1, basePoint.LandscapeType);
                        }
                    }
                }
            }
        }

        public void DrawLandscape()
        {
            if (!_rectanglesInitialized)
                InitRectangles();
            lock (GameCore.Instance.World.Map.SingleObjects)
            {
                foreach (var obj in _world.Map.SingleObjects)
                {
                    _landscapeRectangles[obj.X, obj.Y].Fill = _landscapeBrushes[obj.SingleObjectType];
                }
            }
        }

        public void DrawBuildings()
        {
            if (!_rectanglesInitialized)
                InitRectangles();
            //ClearRectangles(rectArrBuildings);
            lock (GameCore.Instance.World.Map.Mines)
            {
                foreach (var obj in _world.Map.Mines)
                {
                    _buildingRectangles[obj.X, obj.Y].Fill = _mineBrushes[obj.MineType];
                    _buildingRectangles[obj.X, obj.Y].Width = obj.Width * _width;
                    _buildingRectangles[obj.X, obj.Y].Height = obj.Height * _height;

                    if (obj.KingId == _player.King.Id)
                        _buildingRectangles[obj.X, obj.Y].Effect = _playerLighting;
                    else if (obj.KingId != null)
                        _buildingRectangles[obj.X, obj.Y].Effect = _enemyLighting;
                }
            }

            lock (GameCore.Instance.World.Map.Castles)
            {
                foreach (var obj in _world.Map.Castles)
                {
                    _buildingRectangles[obj.X, obj.Y].Fill = _brushCastle;
                    _buildingRectangles[obj.X, obj.Y].Width = obj.Width * _width;
                    _buildingRectangles[obj.X, obj.Y].Height = obj.Height * _height;

                    if (obj.KingId == _player.King.Id)
                        _buildingRectangles[obj.X, obj.Y].Effect = _playerLighting;
                    else if (obj.KingId != null)
                        _buildingRectangles[obj.X, obj.Y].Effect = _enemyLighting;
                }
            }
        }

        public void DrawDynamicObjects()
        {
            if (!_rectanglesInitialized)
                InitRectangles();
            ClearRectangles(_dynamicObjectRectangles);

            lock (GameCore.Instance.World.Map.Resources)
            {
                foreach (var obj in _world.Map.Resources)
                {
                    _dynamicObjectRectangles[obj.X, obj.Y].Fill = _resourceBrushes[obj.ResourceType];
                }
            }

            lock (GameCore.Instance.World.Map.Kings)
            {
                foreach (var obj in _world.Map.Kings)
                {
                    _dynamicObjectRectangles[obj.X, obj.Y].Fill = _brushKing;
                    if (obj.Id != _player.King.Id)
                        _dynamicObjectRectangles[obj.X, obj.Y].Effect = _enemyLighting;
                }
                if (_kingSelected)
                    _dynamicObjectRectangles[_player.King.X, _player.King.Y].Effect = _selectionLighting;
            }
        }

        public void DrawAll()
        {
            DrawGround();
            DrawLandscape();
            DrawBuildings();
            DrawDynamicObjects();
        }

        private void canvasLandscape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void canvasDynamicObjects_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition((IInputElement)sender);
            //индексы для ячейки карты в массиве ячеек rectArr
            int x = (int)(mousePosition.X / _width);
            int y = (int)(mousePosition.Y / _height);
            if (_kingSelected)
            {
                foreach (var castle in _world.Map.Castles)
                {
                    if (castle.X == x && castle.Y == y)
                    {
                        _bigMapCommandController.SendComeInCastleRequest(castle.Id);
                        return;
                    }
                }
                if (x != _player.King.X || y != _player.King.Y)
                {
                    _bigMapCommandController.SendMoveKingRequest(new Point(x, y));
                }
                else
                {
                    _followingKing = !_followingKing;
                }
            }
            else if (_player.King.X == x && _player.King.Y == y)
            {
                _kingSelected = true;
            }
        }

        private void canvasDynamicObjects_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _kingSelected = false;
            _followingKing = false;
        }


        /*public void ShowGetMapResult()
        {
            InitRectangles();
            DrawAll();
            AddRectanglesToCanvas();
            _followingKing = true;
        }*/

        public void UpdateResources()
        {
            foreach (var res in _player.King.ResourceStore.GetResourceListCopy())
            {
                switch (res.ResourceType)
                {
                    case ResourceTypes.Gold:
                        LabelGoldQuantity.Content = res.Quantity.ToString();
                        break;
                    case ResourceTypes.Stone:
                        LabelStoneQuantity.Content = res.Quantity.ToString();
                        break;
                    case ResourceTypes.Wood:
                        LabelWoodQuantity.Content = res.Quantity.ToString();
                        break;
                    case ResourceTypes.Iron:
                        LabelIronQuantity.Content = res.Quantity.ToString();
                        break;
                    case ResourceTypes.Coal:
                        LabelCoalQuantity.Content = res.Quantity.ToString();
                        break;
                }
            }
        }

        /* /// <summary>
         /// Отображение объектов в зоне видимости
         /// </summary>
         public void ShowGetObjectsResult()
         {
             DrawBuildings();
             DrawDynamicObjects();
             if (_followingKing)
                 KingToFocus();
         }

         public void ShowCaptureMineResult()
         {
             //DrawBuildings();
         }

         public void ShowGetKingResult()
         {
         }

         public void ShowMoveKingResult()
         {
         }

         public void ShowGetResourceMessageResult(GetResourceMessage message)
         {
         }*/

        public void EnterCastle()
        {
            timerUpdate.Stop();
            _bigMapCommandController.StopGameStateUpdate();
            _bigMapCommandController.StopObjectsUpdate();
            Uri uri = new Uri("/GameLayer/PresentationLayer/CastleScene.xaml", UriKind.Relative);
            base.MoveTo(uri);
            if ((NavigationService != null))
                NavigationService.Navigate(uri);
        }

        private void scrollViewerMap_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //_followingKing = false;
        }

        private void scrollViewerMap_KeyDown(object sender, KeyEventArgs e)
        {
            _followingKing = false;
            /*if (e.Key == Key.Up || e.Key == Key.W)
            {
                scrollViewerMap.ScrollToVerticalOffset(scrollViewerMap.VerticalOffset - height);
                _followingKing = false;
            } 
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                scrollViewerMap.ScrollToVerticalOffset(scrollViewerMap.VerticalOffset + height);
                _followingKing = false;
            } 
            else if (e.Key == Key.Left || e.Key == Key.A)
            {
                scrollViewerMap.ScrollToHorizontalOffset(scrollViewerMap.HorizontalOffset - width);
                _followingKing = false;
            } 
            else if (e.Key == Key.Right || e.Key == Key.D)
            {
                scrollViewerMap.ScrollToHorizontalOffset(scrollViewerMap.HorizontalOffset + width);
                _followingKing = false;
            } */
        }
    }

}
