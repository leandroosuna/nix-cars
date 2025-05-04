using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using nix_cars.Components.FloatingPlanes;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using System;
using System.Threading;

namespace nix_cars.Components.Cars
{
    public class LocalPlayer : Player
    {
        
        public Car type;
        
        public Vector3 collisionVelocity = Vector3.Zero;
        public Vector2 frameHorizontalVelocity;
        
        public float acceleration = 30.0f;
        public float accelerationBoost = 50.0f;

        public float maxSpeed = 80.0f;
        public float maxSpeedBoost = 110.0f;
        public float boostTimeMax = 3;
        public float boostTimeRemaining = 0;

        public float brakeForce = 80.0f;
        public float friction = 8.0f;
        public float baseTurnRate = 1.8f;
        public float turnSpeedFactor = .10f;
        public float gravity = 20f;
        public float currentGravity;
 
        public float maxSteeringAngle = MathHelper.PiOver4; 
        
        float turnInput;
        
        float targetTurnRate;
        float steeringSharpness = 4f;
        float returnSharpness = 8f;

        float wheelYawSharpness = 2f;
        float wheelYawReturnSharpness = 4f;


        public float thisFrameHorizontalDistance;
        public float thisFrameVerticalDistance;

        public FloatingBoostMeter floatingBoost;
        public FloatingLightTrail lightTrailL;
        public FloatingLightTrail lightTrailR;

        public LocalPlayer(Car car) : base(car )
        {
            game = NixCars.GameInstance();
            name = game.CFG["PlayerName"].Value<string>();

            nameTag = new FloatingText();
            nameTag.SetText(name);
            FloatingPlaneDrawer.Add(nameTag);

            floatingBoost = new FloatingBoostMeter();
            FloatingPlaneDrawer.Add(floatingBoost);

            //lightTrailL = new FloatingLightTrail();
            //lightTrailR = new FloatingLightTrail();
            //FloatingPlaneDrawer.Add(lightTrailL);
            //FloatingPlaneDrawer.Add(lightTrailR);


        }
        public bool inF, inB, inL, inR, inBoost;
        public void Update(bool f, bool b, bool l, bool r, bool boost, float deltaTime)
        {
            inF = f;
            inB = b;
            inL = l;
            inR = r;
            inBoost = boost;

            HandleBoost(deltaTime);
            Engine(f, b, deltaTime);
            Steering(l, r, deltaTime);
            UpdateCollisionVelocity(deltaTime);
            CalculateNewPosition(deltaTime);
            car.HandleLights(b,boosting);
            
        }
        public void PostCollisionUpdate(float deltaTime)
        {
            CalculateWorld();
            car.CalculateLightsPosition();
            car.UpdateCollider();

            var camYaw = NixCars.GameInstance().camera.yaw;

            camYaw = MathHelper.ToRadians(camYaw);
            var mx = Matrix.CreateFromYawPitchRoll(camYaw + MathF.PI, pitch + MathHelper.PiOver2, 0f)
                * Matrix.CreateTranslation(position + Vector3.Up * 3f);

            nameTag.SetRT(mx);

            mx = Matrix.CreateFromYawPitchRoll(yaw, 0, 0f)
                * Matrix.CreateTranslation(position + Vector3.Up * 0.25f - frontDirection * 3.5f);

            floatingBoost.SetRT(mx);
            floatingBoost.SetBoostValue(boostTimeRemaining / boostTimeMax);

            //mx = Matrix.CreateFromYawPitchRoll(yaw + MathF.PI, 0, 0f)
            //    * Matrix.CreateTranslation(car.brakeL.position - frontDirection * 2f);

            //lightTrailL.SetRT(mx);


            //mx = Matrix.CreateFromYawPitchRoll(yaw + MathF.PI, 0, 0f)
            //    * Matrix.CreateTranslation(car.brakeR.position - frontDirection * 2f);

            //lightTrailR.SetRT(mx);

            //lightTrailL.Update(boosting, deltaTime);
            //lightTrailR.Update(boosting, deltaTime);
        }

        public void TP(Vector3 loc)
        {
            position = loc;
            yaw = 0;
            pitch = 0;
            game.camera.position = position - frontDirection * 5
                    + Vector3.Up * 5;

            CalculateWorld();
            car.HandleLights(inB, boosting);
            car.CalculateLightsPosition();
            car.UpdateCollider();
        }
        bool canStartBoosting = true;
        void HandleBoost(float deltaTime)
        {
            if (inF && inBoost && speed > 0 && canStartBoosting)
            {
                boostTimeRemaining -= deltaTime;
                if (boostTimeRemaining > 0)
                {
                    boosting = true;
                }
                else
                {
                    boostTimeRemaining = 0;
                    boosting = false;
                    canStartBoosting = false;
                }
            }
            else
            {
                boosting = false;

                if (boostTimeRemaining >= boostTimeMax / 3)
                {
                    canStartBoosting = true;
                }

                if (boostTimeRemaining <= boostTimeMax)
                {
                    boostTimeRemaining += deltaTime * 0.5f;
                }
                else
                {
                    boostTimeRemaining = boostTimeMax;
                    
                }
            }
        }
        void Engine(bool f, bool b, float deltaTime)
        {
            if (f)
            {

                speed += (boosting?accelerationBoost : acceleration) * deltaTime;
            }
            else if (b)
            {
                if (speed > 0)
                    speed -= brakeForce * deltaTime;
                else
                    speed -= acceleration * 0.5f * deltaTime;
            }
            else
            {
                float deceleration = friction * deltaTime;
                speed = speed > 0 ?
                    Math.Max(speed - deceleration, 0) :
                    Math.Min(speed + deceleration, 0);
            }

            speed = MathHelper.Clamp(speed, -maxSpeed / 4, boosting? maxSpeedBoost : maxSpeed);
            

            float distanceMoved = speed * deltaTime;
            wheelRotationAngle += distanceMoved / car.wheelRadius;
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

            
            float activeSharpness = (turnInput != 0) ? wheelYawSharpness: wheelYawReturnSharpness;

            steeringYaw = float.Lerp(steeringYaw, turnInput * .9F, deltaTime * activeSharpness);


            activeSharpness = (turnInput != 0) ? steeringSharpness : returnSharpness;
           
            currentTurnRate = MathHelper.Lerp(
                currentTurnRate,
                targetTurnRate,
                activeSharpness * deltaTime
            );

            if (Math.Abs(speed) <= 1.5f) return;
            yaw += currentTurnRate * deltaTime;
            yaw = MathHelper.WrapAngle(yaw);
        }

        public void Collided(Vector3 velocity)
        {
            collisionVelocity = velocity;
            collisionImpulse = true;
            collisionImpulseTime = 1f;
        }
        public bool collisionImpulse = false;
        public float collisionImpulseTime = 1f;
        
        void UpdateCollisionVelocity(float deltaTime)
        {
            if(collisionImpulse)
            {
                if (collisionImpulseTime >= 0)
                {
                    collisionVelocity *= collisionImpulseTime;
                    collisionImpulseTime -= deltaTime;
                }
                else
                {
                    collisionVelocity = Vector3.Zero;
                    collisionImpulseTime = 1f;
                    collisionImpulse = false;
                }
            }
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

            var horizontal = direction * speed;
            horizontalVelocity.X = horizontal.X;
            horizontalVelocity.Y = horizontal.Z;

            var vertical = -Vector3.Up * gravity;

            velocity = (horizontal + vertical + collisionVelocity);
            
            frameVelocity = velocity * deltaTime;

            frameHorizontalVelocity.X = frameVelocity.X;
            frameHorizontalVelocity.Y = frameVelocity.Z;

            thisFrameHorizontalDistance = frameHorizontalVelocity.Length();
            thisFrameVerticalDistance = Math.Abs(frameVelocity.Y);
            
            position += frameVelocity;
        }

        
        
    }
}
