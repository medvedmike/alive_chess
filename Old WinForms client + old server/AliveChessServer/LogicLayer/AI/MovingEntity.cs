﻿using System;
using AliveChessLibrary.GameObjects.Abstract;
using AliveChessLibrary.GameObjects.Characters;
using AliveChessServer.LogicLayer.AI.MotionLayer.GeometryUtils;

namespace AliveChessServer.LogicLayer.AI
{
    public abstract class MovingEntity : King
    {
        private Vector2D m_vVelocity;
        //a normalized vector pointing in the direction the entity is heading. 
        private Vector2D m_vHeading;
        //a vector perpendicular to the heading vector
        private Vector2D m_vSide;
        private double m_dMass;
        //the maximum speed this entity may travel at.
        private double m_dMaxSpeed;
        //the maximum force this entity can produce to power itself 
        //(think rockets and thrust)
        private double m_dMaxForce;

        public MovingEntity(MapPoint view)
            : base(view)
        {
            this.Position = new Vector2D(this.X, this.Y);
            this.Velocity = new Vector2D();
            this.MaxSpeed = 1;
            this.Heading = new Vector2D(1, 0);
            this.m_dMass = 1;
            this.MaxForce = 1;
        }

        public MovingEntity(Vector2D position,
            Vector2D velocity,
            double max_speed,
            Vector2D heading,
            double mass,
            double max_force)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.MaxSpeed = max_speed;
            this.Heading = heading;
            this.m_dMass = mass;
            this.MaxForce = max_force;
        }

        private double Clamp(double arg, double minVal, double maxVal)
        {
            if (arg < minVal)
            {
                arg = minVal;
            }

            if (arg > maxVal)
            {
                arg = maxVal;
            }
            return arg;
        }
        //--------------------------- RotateHeadingToFacePosition ---------------------
        //
        //  given a target position, this method rotates the entity's heading and
        //  side vectors by an amount not greater than m_dMaxTurnRate until it
        //  directly faces the target.
        //
        //  returns true when the heading is facing in the desired direction
        //-----------------------------------------------------------------------------
        public bool RotateHeadingToFacePosition(Vector2D target)
        {
            Vector2D toTarget = Vector2D.Vec2DNormalize(target - Position);

            double dot = Heading.Dot(toTarget);

            //some compilers lose acurracy so the value is clamped to ensure it
            //remains valid for the acos
            dot = Clamp(dot, -1, 1);

            //first determine the angle between the heading vector and the target
            double angle = Math.Acos(dot);

            //return true if the player is facing the target
            if (angle < 0.00001) return true;

            ////clamp the amount to turn to the max turn rate
            //if (angle > m_dMaxTurnRate) angle = m_dMaxTurnRate;

            //The next few lines use a rotation matrix to rotate the player's heading
            //vector accordingly
            Matrix2D RotationMatrix = new Matrix2D();

            //notice how the direction of rotation has to be determined when creating
            //the rotation matrix
            RotationMatrix.Rotate(angle * (int)Heading.Sign(toTarget));
            RotationMatrix.TransformVector2Ds(m_vHeading);
            RotationMatrix.TransformVector2Ds(m_vVelocity);

            //finally recreate m_vSide
            m_vSide = m_vHeading.Perp();
            return false;
        }

        public Vector2D Position
        {
            get { return new Vector2D(this.X, this.Y); }
            set { this.X = (int)value.X; this.Y = (int)value.Y; }
        }

        public Vector2D Velocity
        {
            get { return m_vVelocity; }
            set { m_vVelocity = value; }
        }

        public Vector2D Heading
        {
            get { return m_vHeading; }
            set { m_vHeading = value; }
        }

        public Vector2D Side
        {
            get
            {
                if (m_vSide == null && m_vHeading != null)
                    m_vSide = m_vHeading.Perp();
                return m_vSide;
            }
            set { m_vSide = value; }
        }

        public double Mass
        {
            get { return m_dMass; }
        }

        public double MaxSpeed
        {
            get { return m_dMaxSpeed; }
            set { m_dMaxSpeed = value; }
        }

        public double MaxForce
        {
            get { return m_dMaxForce; }
            set { m_dMaxForce = value; }
        }

        public bool IsSpeedMaxedOut() { return m_dMaxSpeed * m_dMaxSpeed >= m_vVelocity.LengthSq(); }
        public double Speed { get { return m_vVelocity.Length(); } }
        public double SpeedSq { get { return m_vVelocity.LengthSq(); } }
    }
}
