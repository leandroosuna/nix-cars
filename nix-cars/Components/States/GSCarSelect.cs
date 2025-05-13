using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using nix_cars.Components.Cars;
using nix_cars.Components.GUI;
using nix_cars.Screens;
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

            CarManager.LoadColorsTo(carPlayers);
            carCount = carPlayers.Count;
        }
        CarSelect cs;
        public override void OnSwitch()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
            GumManager.SwitchTo(Screen.CARSELECT);
            cs = GumManager.GetCarSelect();
            
            

            SetSelectedColorPicker();

            for(int i = 0; i < carPlayers.Count; i++)
            {
                carPlayers[i].car.p = carPlayers[i];
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
                SelectAndEnter();
            }

            if (km.KeyDownOnce(km.Escape))
            {
                GameStateManager.SwitchTo(State.MAIN);
            }

            if (km.KeyDownOnce(km.Right) || km.KeyDownOnce(km.Right2))
                InFocusChangeBy(-1);
            if (km.KeyDownOnce(km.Left) || km.KeyDownOnce(km.Left2))
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

            FinishUpdate();
        }
        public void InFocusChangeBy(int v)
        {
            carInFocus += v;
            if(carInFocus < 0)
                carInFocus = carCount -1;
            else
                carInFocus %= carCount;

            SetSelectedColorPicker();
        }

        public void SetSelectedColorPicker()
        {
            var c = carPlayers[carInFocus].car.colors;

            cs.eventsEnabled = false;
            cs.R.Value = c[0].X * 100;
            cs.G.Value = c[0].Y * 100;
            cs.B.Value = c[0].Z * 100;
            cs.rgb0 = c[0];
            cs.SelectedColor.Color = new Color(c[0].X, c[0].Y, c[0].Z);
            cs.RVal.Text = $"{(int)(c[0].X * 255)}";
            cs.GVal.Text = $"{(int)(c[0].Y * 255)}";
            cs.BVal.Text = $"{(int)(c[0].Z * 255)}";

            cs.R1.Value = c[1].X * 100;
            cs.G1.Value = c[1].Y * 100;
            cs.B1.Value = c[1].Z * 100;
            cs.rgb1 = c[1]; 
            cs.SelectedColor1.Color = new Color(c[1].X, c[1].Y, c[1].Z);
            cs.RVal1.Text = $"{(int)(c[1].X * 255)}";
            cs.GVal1.Text = $"{(int)(c[1].Y * 255)}";
            cs.BVal1.Text = $"{(int)(c[1].Z * 255)}";
            
            if (c.Length > 2)
            {
                cs.R2.Value = c[2].X * 100;
                cs.G2.Value = c[2].Y * 100;
                cs.B2.Value = c[2].Z * 100;
                cs.ColorPicker3.Visible = true;
                cs.rgb2 = c[2]; 
                cs.SelectedColor2.Color = new Color(c[2].X, c[2].Y, c[2].Z);
                cs.RVal2.Text = $"{(int)(c[2].X * 255)}";
                cs.GVal2.Text = $"{(int)(c[2].Y * 255)}";
                cs.BVal2.Text = $"{(int)(c[2].Z * 255)}";
               
            }
            else
            {
                cs.ColorPicker3.Visible = false;
            }
            cs.eventsEnabled = true;
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
        public void SelectAndEnter()
        {
           
            GameStateManager.SwitchTo(State.RUN);
            CarManager.ChangePlayerCar(CarManager.localPlayer,carPlayers[carInFocus].car);
             
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
