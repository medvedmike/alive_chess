// Static Model
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Drawing;
using AliveChessLibrary.GameObjects;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Buildings;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessLibrary.GameObjects.Landscapes;
using AliveChessLibrary.Mathematic.GeometryUtils;
using AliveChessServer.LogicLayer.AI.DecisionLayer;
using AliveChessServer.LogicLayer.AI.DecisionLayer.NeuralNetwork;
using AliveChessServer.LogicLayer.AI.MotionLayer;
using AliveChessServer.LogicLayer.AI.PerceptionLayer;

namespace AliveChessServer.LogicLayer.AI
{
    [Table(Name = "dbo.king")]
	public class BotKing : King
	{
        private Animat animat;
        private Steering steering;
        private IBrain brain;
        private PoolingStation poolingStation;

        private Rectangle square;
        private bool obstacleFound = false;

        private TimeSpan _thinkPeriod = TimeSpan.Zero;
        private TimeSpan _maxThinkPeriod = TimeSpan.FromSeconds(1);
       
        public BotKing(Map map, MapPoint view, GameData data, NeuroTeacher teacher)
            : base(view)
        {
            this.Map = map;

            steering = new Steering(this);
            poolingStation = new PoolingStation(this);
            brain = new NeuroBrain(this, teacher, poolingStation);
            square = new Rectangle(0, 0, 30, 30);
        }

        public BotKing(Animat animat, Map map, MapPoint view, GameData data, NeuroTeacher teacher)
            : this(map, view, data, teacher)
        {
            this.animat = animat;
            this.animat.Kings.Add(this);
        }

        private void UpdateVelocity()
        {
            Vector2D force = this.Steering.Calculate();

            if (Steering.SteeringForce.IsZero())
            {
                const double BrakingRate = 0.8;
                Velocity = Velocity * BrakingRate;
            }
            Vector2D accel = force / Mass;
            Velocity += accel;
            Velocity.Truncate(MaxSpeed);
        }

        private Point GetCoordinates()
        {
            return new Point(X + (int)Math.Round(Velocity.X), Y + (int)Math.Round(Velocity.Y));
        }

        public void UpdateMovement()
        {
            //Vector2D force = this.Steering.Calculate();

            //if (Steering.SteeringForce.IsZero())
            //{
            //    const double BrakingRate = 0.8;
            //    Velocity = Velocity * BrakingRate;
            //}
            //Vector2D accel = force / Mass;
            //Velocity += accel;
            //Velocity.Truncate(MaxSpeed);

            UpdateVelocity();

            Point coordinates = GetCoordinates();

            if (!Velocity.IsZero())
            {
                if (!Check(coordinates.X, coordinates.Y, 1))
                {
                    if (steering.WanderIsOn())
                    {
                        Heading = Vector2D.Vec2DNormalize(
                            Direction.ChooseRandomDirection(CheckPosition, X, Y, 1));
                    }
                    if (steering.FleeIsOn())
                    {
                        ObstacleFound = true;
                    }
                    if (steering.SeekIsOn())
                    {
                        ObstacleFound = true;
                    }
                    if (steering.PursuitIsOn())
                    {
                        ObstacleFound = true;
                    }
                    if (steering.EvadeIsOn())
                    {
                        ObstacleFound = true;
                    }

                    Side = Heading.Perp();
                }
                else
                {
                    Heading = Vector2D.Vec2DNormalize(Velocity);
                    Side = Heading.Perp();
                    Position = new Vector2D(coordinates.X, coordinates.Y);
                    poolingStation.Position = coordinates;
                }
            }
        }

        public override void Update()
        {
            if (_thinkPeriod > _maxThinkPeriod)
            {
                brain.Process();
                brain.Think();
                _thinkPeriod = TimeSpan.Zero;
            }
            else _thinkPeriod += TimeSpan.FromMilliseconds(10);

            if (time > TimeSpan.FromMilliseconds(300))
            {
                if (IsMove)
                {
                    base.Update();
                }
                else
                {
                    if (this.State == KingState.BigMap)
                    {
                        UpdateMovement();
                        DoStep(Position);
                    }
                }

                time = TimeSpan.Zero;
            }
            else time += TimeSpan.FromMilliseconds(10);
        }

        private bool Check(int x, int y, int costLimit)
        {
            if (steering.WanderIsOn())
                return CheckPosition(x, y, costLimit);
            else return Map.CheckPoint(x, y, costLimit);
        }

        private bool CheckPosition(int x, int y, int costLimit)
        {
            return Map.CheckPoint(x, y, costLimit) && CheckSquare(x, y);
        }

        private bool CheckSquare(int x, int y)
        {
            return x >= square.Left && x <= square.Right && y >= square.Top && y <= square.Bottom;
        }

        public override IPlayer Player
        {
            get { return this.animat; }
            set { this.animat = (Animat)value; }
        }

        public Steering Steering
        {
            get { return steering; }
            set { steering = value; }
        }

        public bool ObstacleFound
        {
            get { return obstacleFound; }
            set { obstacleFound = value; }
        }

        public void AttachNeuralNetwork(INeuralNetwork network)
        {
            this.brain.NN = network;
        }
	}// END CLASS DEFINITION BotKing

} // AliveChess.Logic.GameLogic.BotsLogic
