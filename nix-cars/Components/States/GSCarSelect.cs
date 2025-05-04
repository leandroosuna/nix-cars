using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Cars;
using nix_cars.Components.GUI;
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

            ep = new EnemyPlayer(0);
            car = new CarMuscle();
            car.Init(ep);
            ep.car = car;
            carPlayers.Add(ep);

            
            carCount = carPlayers.Count;
        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
            GumManager.SwitchTo(Screen.CARSELECT);
            //game.camera.LockFromTo(new Vector3(2, 5, -5), Vector3.Zero);

            for(int i = 0; i < carPlayers.Count; i++)
            {
                carPlayers[i].position = Vector3.Zero + Vector3.UnitX* 10 * i;
                carPlayers[i].CalculateWorld();

            }

        }

        float yawInFocus = 0;


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(km.KeyDownOnce(km.Enter))
            {
                // TODO: assign car
                GameStateManager.SwitchTo(State.RUN);
            }

            if (km.KeyDownOnce(km.Escape))
            {
                GameStateManager.SwitchTo(State.MAIN);
            }

            if (km.KeyDownOnce(km.Right2))
                InFocusChangeBy(-1);
            if (km.KeyDownOnce(km.Left2))
                InFocusChangeBy(+1);

            var ifPos = carPlayers[carInFocus].position;
            var camPos = ifPos + new Vector3(0, 5, -5);

            game.camera.SmoothMoveTo(camPos);
            game.camera.SmoothRotateTo(new Vector3(0,-.5f,.5f));
            game.camera.Update(uDeltaTimeFloat);

            
            for(int i = 0; i < carPlayers.Count;i++)
            {
                if (i == carInFocus)
                {
                    carPlayers[i].yaw += uDeltaTimeFloat;
                    carPlayers[i].yaw %= MathHelper.TwoPi;
                }
                else
                {
                    carPlayers[i].yaw = MathF.PI;
                    
                }
                carPlayers[i].CalculateWorld();

            }

            carPlayers[carInFocus].yaw += uDeltaTimeFloat;
            carPlayers[carInFocus].yaw %= MathHelper.TwoPi;

            

            //carPlayers[(carInFocus + 1)%carCount ].position = new Vector3(10,0,0);
            //carPlayers[(carInFocus + 1) % carCount].yaw =MathF.PI + MathHelper.PiOver4;
            //carPlayers[(carInFocus + 1) % carCount].CalculateWorld();

            //carPlayers[(carInFocus + 2) % carCount].position = new Vector3(-10, 0, 0);
            //carPlayers[(carInFocus + 2) % carCount].yaw = MathF.PI - MathHelper.PiOver4;
            //carPlayers[(carInFocus + 2) % carCount].CalculateWorld();


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


            //var str = $"{game.camera.position}";
            //game.spriteBatch.Begin();
            //game.spriteBatch.DrawString(game.font25, str, Vector2.Zero, Color.White);
            //game.spriteBatch.End();
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
