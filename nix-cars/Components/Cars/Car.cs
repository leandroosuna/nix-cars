using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.Lights;
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
        public Vector3 frontDirection;
        public Vector3 rightDirection;
        public Vector3 upDirection;

        public Model model;
        public CarType type;
        public Vector3[] colors;

        public float scale = 1f;
        public Matrix mxScale;
        public Matrix world;
        public Matrix frontWheelWorld;
        public Matrix backWheelWorld;

        public float speed;
        public float acceleration = 30.0f;
        public float maxSpeed = 100.0f;
        public float brakeForce = 50.0f;
        public float friction = 8.0f;
        public float baseTurnRate = 1.8f;
        public float turnSpeedFactor = .10f;

        public float wheelRadius = 0.5f;        
        public float maxSteeringAngle = MathHelper.PiOver4; 
        public float wheelRotationAngle;
        
        float turnInput;
        public float currentTurnRate;
        float targetTurnRate;
        float steeringSharpness = 4f;
        float returnSharpness = 8f;

        public PointLight brakeL;
        public PointLight brakeR;
        public ConeLight frontL;
        public ConeLight frontR;
        public Car(Model model, CarType type)
        {
            this.model = model;
            this.type = type;
            mxScale = Matrix.CreateScale(scale);

            brakeL = new PointLight(position, 1.5f, new Vector3(1, 0, 0), new Vector3(1, 0, 0));
            brakeR = new PointLight(position, 1.5f, new Vector3(1, 0, 0), new Vector3(1, 0, 0));
            frontL = new ConeLight(position, frontDirection, 3f, 7f, Vector3.One, Vector3.One);
            frontR = new ConeLight(position, frontDirection, 3f, 7f, Vector3.One, Vector3.One);


            var lm = NixCars.GameInstance().lightsManager;
            lm.Register(brakeL);
            lm.Register(brakeR);
            lm.Register(frontL);
            lm.Register(frontR);

            //frontL.hasLightGeo = true;
            //frontR.hasLightGeo = true;

            brakeL.enabled = false;
            brakeR.enabled = false;
        }
        public void Update(Vector3 position, float yaw, float pitch)
        {
            this.position = position;
            this.yaw = yaw;
            this.pitch = pitch;

            CalculateWorld();
            CalculateLightsPosition();
        }
        public void Update(bool f, bool b, bool l, bool r, float deltaTime)
        {
            Engine(f, b, deltaTime);
            Steering(l, r, deltaTime);
            CalculateNewPosition(deltaTime);
            CalculateWorld();
            HandleLights(b);
            
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

            speed = MathHelper.Clamp(speed, -maxSpeed / 4, maxSpeed);

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

        void CalculateLightsPosition()
        {
            brakeL.position = position - frontDirection * 2.65f - rightDirection * 0.75f + upDirection * 1f;
            brakeR.position = position - frontDirection * 2.65f + rightDirection * 0.75f + upDirection * 1f;

            frontL.position = position + frontDirection * 6f - rightDirection * .8f + upDirection * .90f;
            frontL.direction = frontDirection;
            frontR.position = position + frontDirection * 6f + rightDirection * .8f + upDirection * .90f;
            frontR.direction = frontDirection;
        }

        void HandleLights(bool b)
        {
            brakeL.enabled = false;
            brakeR.enabled = false;
            brakeL.color = Vector3.Zero;
            brakeR.color = Vector3.Zero;
            if (speed <= 0 || b)
            {


                if (speed >= 0)
                {
                    brakeL.color = Vector3.UnitX;
                    brakeL.specularColor = brakeL.color;
                    brakeR.color = Vector3.UnitX;
                    brakeR.specularColor = brakeR.color;
                    brakeL.enabled = true;
                    brakeR.enabled = true;
                }
                else if (b)
                {
                    brakeL.color = Vector3.One;
                    brakeL.specularColor = brakeL.color;
                    brakeR.color = Vector3.One;
                    brakeR.specularColor = brakeR.color;
                    brakeL.enabled = true;
                    brakeR.enabled = true;
                }
            }
            CalculateLightsPosition();
        }
        Vector3 tempFront;
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

            tempFront.X = MathF.Cos(-yaw + MathHelper.PiOver2) * MathF.Cos(pitch);
            tempFront.Y = MathF.Sin(pitch);
            tempFront.Z = MathF.Sin(-yaw + MathHelper.PiOver2) * MathF.Cos(pitch);

            frontDirection = Vector3.Normalize(tempFront);

            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, frontDirection));
        }
    }

    public enum CarType
    {
        Sport
    }

}
