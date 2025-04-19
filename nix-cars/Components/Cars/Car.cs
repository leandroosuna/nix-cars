using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Cars
{
    public abstract class Car
    {
        public PointLight brakeL;
        public PointLight brakeR;
        public ConeLight frontL;
        public ConeLight frontR;

        public float offsetBrakeFrontDir = -2.65f;
        public float offsetBrakeRightDir = 0.75f;
        public float offsetBrakeUpDir = 1f;

        public float offsetHeadFrontDir = 5f;
        public float offsetHeadRightDir = 0.8f;
        public float offsetHeadUpDir = 0.9f;

        public OrientedBoundingBox collider;
        public Vector3 colliderExtents = new Vector3(1.125f, .85f, 2.5f);
        public Vector3 colliderCenterOffset = new Vector3(0, 0.75f, 0);

        public NixCars game;
        
        public Model model;

        public Vector3[] colors;

        public Player p;
        
        public void Init(Player p)
        {
            this.p = p;
            brakeL = new PointLight(Vector3.Zero, 2.5f, new Vector3(1, 0, 0), new Vector3(1, 0, 0));
            brakeR = new PointLight(Vector3.Zero, 2.5f, new Vector3(1, 0, 0), new Vector3(1, 0, 0));
            frontL = new ConeLight(Vector3.Zero, Vector3.Zero, 10f, 7f, Vector3.One, Vector3.One);
            frontR = new ConeLight(Vector3.Zero, Vector3.Zero, 10f, 7f, Vector3.One, Vector3.One);

            game = NixCars.GameInstance();
            var lm = game.lightsManager;
            lm.Register(brakeL);
            lm.Register(brakeR);
            lm.Register(frontL);
            lm.Register(frontR);

            collider = new OrientedBoundingBox(Vector3.Zero, colliderExtents);

            LoadModel();
        }
        public abstract void LoadModel();

        public void HandleLights(bool b)
        {
            brakeL.enabled = false;
            brakeR.enabled = false;
            brakeL.color = Vector3.Zero;
            brakeR.color = Vector3.Zero;
            if (p.speed <= 0 || b)
            {
                if (p.speed >= 0)
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
        }
        public void CalculateLightsPosition()
        {
            brakeL.position = p.position + p.frontDirection * offsetBrakeFrontDir - p.rightDirection * offsetBrakeRightDir + p.upDirection * offsetBrakeUpDir;
            brakeR.position = p.position + p.frontDirection * offsetBrakeFrontDir + p.rightDirection * offsetBrakeRightDir + p.upDirection * offsetBrakeUpDir;

            frontL.position = p.position + p.frontDirection * offsetHeadFrontDir - p.rightDirection * offsetHeadRightDir + p.upDirection * offsetHeadUpDir;
            frontL.direction = p.frontDirection;
            frontR.position = p.position + p.frontDirection * offsetHeadFrontDir + p.rightDirection * offsetHeadRightDir + p.upDirection * offsetHeadUpDir;
            frontR.direction = p.frontDirection;
        }

        public void UpdateCollider()
        {
            collider.Center = p.position + colliderCenterOffset;
            collider.Orientation = Matrix.CreateFromYawPitchRoll(p.yaw, p.pitch, 0);
        }

        public abstract void Draw();
    }
}
