using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Cars;
using System;
using System.Collections.Generic;

namespace nix_cars.Components.States
{
    public class GSCarSelect : GameState
    {
        List<Player> carPlayers = new List<Player>();
        public int carInFocus = 0;
        public int carCount;
        public GSCarSelect() : base()
        {
            var ep = new EnemyPlayer(0);
            Car car = new CarSport();
            car.Init(ep);
            ep.car = car;
            carPlayers.Add(ep);

            ep = new EnemyPlayer(0);
            car = new CarHatchback();
            car.Init(ep); 
            ep.car = car;
            carPlayers.Add(ep);

            ep = new EnemyPlayer(0);
            car = new CarRoadster();
            car.Init(ep); 
            ep.car = car;
            carPlayers.Add(ep);

            carCount = carPlayers.Count;
        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
        }

        float yawInFocus = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //game.camera.RotateBy(new Vector2(uDeltaTimeFloat, 0));
            game.camera.SmoothRotateTo(Vector3.Zero);
            game.camera.UpdatePosition(new Vector3(150, 5, -150));

            yawInFocus += uDeltaTimeFloat;
            yawInFocus %= MathHelper.TwoPi;

            carPlayers[carInFocus].position = Vector3.Zero;
            carPlayers[carInFocus].yaw = yawInFocus;
            carPlayers[carInFocus].CalculateWorld();

            
            carPlayers[(carInFocus + 1)%carCount ].position = new Vector3(10,0,0);
            carPlayers[(carInFocus + 1) % carCount].yaw =MathF.PI + MathHelper.PiOver4;
            carPlayers[(carInFocus + 1) % carCount].CalculateWorld();

            carPlayers[(carInFocus + 2) % carCount].position = new Vector3(-10, 0, 0);
            carPlayers[(carInFocus + 2) % carCount].yaw = MathF.PI - MathHelper.PiOver4;
            carPlayers[(carInFocus + 2) % carCount].CalculateWorld();


            FinishUpdate();
        }
        public void InFocusChangeBy(int v)
        {
            carInFocus += v;
            if(carInFocus < 0)
                carInFocus = carCount -1;
            else
                carInFocus %= carCount;
        }
        public void SetCarColor(int colIndex, Vector3 color)
        {
            carPlayers[carInFocus].car.colors[colIndex] = color;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.basicModelEffect.SetView(game.camera.view);
            game.basicModelEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetView(game.camera.view);
            game.deferredEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetCameraPosition(game.camera.position);

            game.GraphicsDevice.SetRenderTargets(game.colorTarget, game.normalTarget, game.positionTarget, game.bloomFilterTarget);
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            DrawCars();

            game.GraphicsDevice.SetRenderTargets(game.lightTarget, game.blurHTarget, game.blurVTarget);
            game.GraphicsDevice.BlendState = BlendState.Additive;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            game.deferredEffect.SetColorMap(game.colorTarget);
            game.deferredEffect.SetNormalMap(game.normalTarget);
            game.deferredEffect.SetPositionMap(game.positionTarget);
            game.deferredEffect.SetBloomFilter(game.bloomFilterTarget);
            game.lightsManager.DrawAmbient();

            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            game.GraphicsDevice.Clear(Color.Gray);
            game.deferredEffect.SetLightMap(game.lightTarget);
            game.deferredEffect.SetScreenSize(new Vector2(game.lightTarget.Width, game.lightTarget.Height));
            game.deferredEffect.SetTech("integrate");

            game.fullScreenQuad.Draw(game.deferredEffect.effect);


            FinishDraw();
        }
        
        void DrawCars()
        {
            foreach(var cp in carPlayers)
            {
                cp.car.Draw();
            }
        }
        public override void OnResolutionChange(int w, int h)
        {

        }
        public override void LostFocus()
        {
            
        }
        public override void Focused()
        {
            
        }

        
    }
}
