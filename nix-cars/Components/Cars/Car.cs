using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nix_cars.Components.Cars
{
    public class Car
    {
        public Vector3 position = Vector3.Zero;
        public float yaw = 0f;
        public float pitch = 0f;
        public Model model;
        public CarType type;
        public Vector3[] colors;

        public float scale = 1f;
        public Matrix mxScale;
        public Matrix world;
        public Matrix frontWheelWorld;
        public Matrix backWheelWorld;


        public float speed;
        public float acceleration = 20.0f;
        public float maxSpeed = 100.0f;
        public float brakeForce = 50.0f;
        public float friction = 8.0f;
        public float baseTurnRate = 1.8f;
        public float turnSpeedFactor = .10f;

        public float wheelRadius = 0.5f;        
        public float maxSteeringAngle = MathHelper.PiOver4; 
        public float wheelRotationAngle;
        
        float turnInput;
        float currentTurnRate;
        float targetTurnRate;
        float steeringSharpness = 4f;
        float returnSharpness = 8f;

        public Car(Model model, CarType type)
        {
            this.model = model;
            this.type = type;
            mxScale = Matrix.CreateScale(scale);

        }

        public void Update(Vector3 position, float yaw, float pitch)
        {
            this.position = position;
            this.yaw = yaw;
            this.pitch = pitch;

            CalculateWorld();
        }

        public void Update(bool f, bool b, bool l, bool r, float deltaTime)
        {
            Engine(f, b, deltaTime);
            Steering(l, r, deltaTime);
            CalculateNewPosition(deltaTime);
            CalculateWorld();
        }

        void Engine(bool f, bool b, float deltaTime)
        {
            if (f)
            {
                speed += acceleration * deltaTime;
            }
            else if (b)
            {
                speed -= brakeForce * deltaTime;
            }
            else
            {
                float deceleration = friction * deltaTime;
                speed = speed > 0 ?
                    Math.Max(speed - deceleration, 0) :
                    Math.Min(speed + deceleration, 0);
            }

            speed = MathHelper.Clamp(speed, -maxSpeed / 2, maxSpeed);

            float distanceMoved = speed * deltaTime;
            wheelRotationAngle += distanceMoved / wheelRadius;
            wheelRotationAngle = MathHelper.WrapAngle(wheelRotationAngle);

        }




        void Steering(bool l, bool r, float deltaTime)
        {
            turnInput = 0;
            if (l) turnInput += 1;
            if (r) turnInput -= 1;

            if (turnInput != 0)
            {
                float speedPercentage = Math.Abs(speed) / maxSpeed;
                float turnMultiplier = 1.5f / (1 + speedPercentage * 0.5f);
                targetTurnRate = turnInput * baseTurnRate * turnMultiplier;

                if (speed < 0) targetTurnRate *= -1;
            }
            else
            {
                targetTurnRate = 0;
            }

            float activeSharpness = (turnInput != 0) ? steeringSharpness : returnSharpness;

            currentTurnRate = MathHelper.Lerp(
                currentTurnRate,
                targetTurnRate,
                activeSharpness * deltaTime
            );

            if (Math.Abs(speed) <= 1.5f) return;
            yaw += currentTurnRate * deltaTime;
            yaw = MathHelper.WrapAngle(yaw);
        }

        void CalculateNewPosition(float deltaTime)
        {
            Vector3 direction = new Vector3(
                (float)Math.Sin(yaw),
                0,
                (float)Math.Cos(yaw)
            );

            if (direction.LengthSquared() > 0.001f)
                direction.Normalize();


            position += direction * speed * deltaTime;
        }
        void CalculateWorld()
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
        }


    }

    public enum CarType
    {
        Sport
    }

}
