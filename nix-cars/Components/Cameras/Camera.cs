
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.Cars;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Cameras
{
    public class Camera
    {
        public Vector3 position;
        public Vector3 frontDirection;
        public Vector3 rightDirection;
        public Vector3 upDirection;

        public Matrix view, projection;
        public Matrix viewProjection;

        public float fieldOfView;
        public float aspectRatio;
        public float nearPlaneDistance;
        public float farPlaneDistance;

        public float yaw;
        public float pitch;

        public BoundingFrustum frustum;
        public bool isFree = false;

        NixCars game;
        public Camera(float aspectRatio)
        {
            game = NixCars.GameInstance();
            frustum = new BoundingFrustum(Matrix.Identity);
            fieldOfView = MathHelper.ToRadians(95);
            this.aspectRatio = aspectRatio;
            position = new Vector3(0, 5f, -5);
            nearPlaneDistance = .1f;
            farPlaneDistance = 2000;
            yaw = 90;
            pitch = -45;

            UpdateVectors();
            CalculateView();

            CalculateProjection();

        }

        public void ChangeFOV(int deg)
        {
            fieldOfView = MathHelper.ToRadians(deg);
            CalculateProjection();
        }

        
        public void MoveBy(bool f, bool b, bool l, bool r, bool u, bool d, float speed, float delta)
        {
            Vector3 dir = Vector3.Zero;

            if (f)
                dir += frontDirection;
            if (b)
                dir -= frontDirection;

            if (r)
                dir += rightDirection;
            if (l)
                dir -= rightDirection;

            if (u)
                dir += Vector3.Up;
            if (d)
                dir -= Vector3.Up;

            if (dir != Vector3.Zero)
            {
                dir.Normalize();
                position += dir * speed * delta;
            }
            CalculateView();
        }
        
        public void RotateBy(Vector2 pitchYaw)
        {
            yaw += pitchYaw.X;
            if (yaw < 0)
                yaw += 360;
            yaw %= 360;

            pitch -= pitchYaw.Y;

            if (pitch > 89.0f)
                pitch = 89.0f;
            else if (pitch < -89.0f)
                pitch = -89.0f;

            UpdateVectors();
            CalculateView();

            
        }

        Vector3 targetDirection;
        Vector3 targetPosition;
        public float smoothRotateSpeed = 3f;
        //public float smoothMoveSpeed = 12f; // slow
        public float smoothMoveSpeed = 20f; // 

        public void SmoothRotateTo(Vector3 target)
        {
            targetDirection = target;
            
        }

        public void SmoothMoveTo(Vector3 target)
        {
            targetPosition = target;
        }

        public void Update(float deltaTime)
        {
            frontDirection = Vector3.Lerp(frontDirection, targetDirection, deltaTime * smoothRotateSpeed);
            frontDirection.Normalize();

            position = Vector3.Lerp(position, targetPosition, deltaTime * smoothMoveSpeed);

            UpdatePitchYawVectors();
            CalculateView();
        }
        
        public void UpdatePitchYawVectors()
        {
            yaw = MathHelper.ToDegrees(MathF.Atan2(frontDirection.X, frontDirection.Z));
            pitch = MathHelper.ToDegrees(MathF.Asin(frontDirection.Y));

            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, frontDirection));
        }
        public void UpdateVectors()
        {
            Vector3 tempFront;

            tempFront.X = MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            tempFront.Y = MathF.Sin(MathHelper.ToRadians(pitch));
            tempFront.Z = MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));

            frontDirection = Vector3.Normalize(tempFront);

            rightDirection = Vector3.Normalize(Vector3.Cross(frontDirection, Vector3.Up));
            upDirection = Vector3.Normalize(Vector3.Cross(rightDirection, frontDirection));
        }

        void CalculateView()
        {
            view = Matrix.CreateLookAt(position, position + frontDirection, upDirection);
            frustum.Matrix = view * projection;
        }
        void CalculateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
            frustum.Matrix = view * projection;
        }

        public void ToggleFree()
        {
            if(!isFree)
            {
                game.gameState.mouseLocked = true;
                game.IsMouseVisible = false;
                game.gameState.mouseDelta = Vector2.Zero;
            }
            else
            {
                game.gameState.mouseLocked = false;
                game.IsMouseVisible = true;

            }
            isFree = !isFree; 

        }
        public bool FrustumContains(BoundingSphere collider)
        {
            return !frustum.Contains(collider).Equals(ContainmentType.Disjoint);
        }
        public bool FrustumContains(BoundingBox collider)
        {
            return !frustum.Contains(collider).Equals(ContainmentType.Disjoint);
        }
        public bool FrustumContains(Vector3 point)
        {
            return !frustum.Contains(point).Equals(ContainmentType.Disjoint);
        }

    }
}