using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.FlotatingTextures;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using System;
using System.Collections.Generic;
using System.Threading;


namespace nix_cars.Components.Cars
{
    public abstract class Player
    {
        public uint id;
        public string name;
        public bool connected;
        public Vector3 position = new Vector3(232f,15,-323);
        public float yaw = 0f;
        public float pitch = 0f;

        public Vector3 frontDirection;
        public Vector3 rightDirection;
        public Vector3 upDirection;

        public Car car;

        public float scale = 1f;
        public Matrix mxScale;
        public Matrix world;
        public Matrix frontWheelWorld;
        public Matrix backWheelWorld;

        public Vector3 velocity;
        public Vector2 horizontalVelocity;
        public Vector3 frameVelocity;

        public float speed;

        public float wheelRotationAngle;
        
        public float currentTurnRate;

        public LinkedList<PlayerCache> netDataCache = new LinkedList<PlayerCache>();
        public FlotatingText nameBanner;
        public NixCars game;
        // TODO: server side.
        //public void Collided(Vector3 velocity) 
        //{
        //    collisionVelocity = velocity;
        //    collisionImpulse = true;
        //    collisionImpulseTime = 1f;
        //}
        //public bool collisionImpulse = false;
        //public float collisionImpulseTime = 1f;

        //void UpdateCollisionVelocity(float deltaTime)
        //{
        //    if(collisionImpulse)
        //    {
        //        if (collisionImpulseTime >= 0)
        //        {
        //            collisionVelocity *= collisionImpulseTime;
        //            collisionImpulseTime -= deltaTime;
        //        }
        //        else
        //        {
        //            collisionVelocity = Vector3.Zero;
        //            collisionImpulseTime = 1f;
        //            collisionImpulse = false;
        //        }
        //    }
        //}

        public Player()
        {
            mxScale = Matrix.CreateScale(scale);
        }
        public Player(Car car)
        {
            this.car = car;
            car.Init(this);
            mxScale = Matrix.CreateScale(scale);

        }
        
        public void CalculateWorld()
        {
            world = mxScale *
                Matrix.CreateFromYawPitchRoll(yaw, pitch, 0) *
                Matrix.CreateTranslation(position);

            float steeringYaw = currentTurnRate * 0.35f;
            if (speed < 0)
                steeringYaw = -steeringYaw;

            float wheelPitch = wheelRotationAngle; 

            Matrix steering = Matrix.CreateRotationZ(steeringYaw);
            Matrix rotation = Matrix.CreateRotationX(wheelPitch);

            frontWheelWorld = rotation * steering;
            backWheelWorld = rotation;

            CalculateDirectionVectors();
        }
        Vector3 tempFront;
        void CalculateDirectionVectors()
        {
            tempFront.X = MathF.Cos(-yaw + MathHelper.PiOver2) * MathF.Cos(pitch);
            tempFront.Y = MathF.Sin(pitch);
            tempFront.Z = MathF.Sin(-yaw + MathHelper.PiOver2) * MathF.Cos(pitch);

            frontDirection = Vector3.Normalize(tempFront);

            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, frontDirection));
        }
    }
}
