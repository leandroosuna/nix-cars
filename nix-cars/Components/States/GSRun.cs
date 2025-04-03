using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.Cars;


namespace nix_cars.Components.States
{
    public class GSRun : GameState
    {
        Model plane;
        Vector3 playerPosition;
        float moveSpeed = 10f;
        Model map;
        Texture2D[] numTex;
        public GSRun() : base()
        {
            plane = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/plane");
            map = game.Content.Load<Model>(NixCars.ContentFolder3D + "maps/peach/peach");
            numTex = new Texture2D[101];
            for(int i = 0; i < 101; i++)
            {
                numTex[i] = game.Content.Load<Texture2D>(NixCars.ContentFolder3D + "basic/Tex/num/"+i);
            }
            NixCars.AssignEffectToModel(plane, game.basicModelEffect.effect);
            NixCars.AssignEffectToModel(map, game.basicModelEffect.effect);

            playerPosition = game.camera.position;
        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = false;
            mouseLocked = true; 
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
            
            if (km.Escape.IsDown() && !keysDown.Contains(km.Escape))
            {
                keysDown.Add(km.Escape);

                GameStateManager.SwitchTo(State.MAIN);
            }

            if (km.Enter.IsDown() && !keysDown.Contains(km.Enter))
            {
                keysDown.Add(km.Enter);

                GameStateManager.SwitchTo(State.RUN);
            }
            
            //game.camera.MoveBy(km.Forward.IsDown(), km.Backward.IsDown(), km.Left.IsDown(), km.Right.IsDown(),
            //   keyState.IsKeyDown(Keys.Space), keyState.IsKeyDown(Keys.LeftControl),
            //   keyState.IsKeyDown(Keys.LeftShift)?moveSpeed:moveSpeed*2, uDeltaTimeFloat);

            CarManager.UpdatePlayerCar(keyState.IsKeyDown(Keys.Up), keyState.IsKeyDown(Keys.Down), keyState.IsKeyDown(Keys.Left),
                keyState.IsKeyDown(Keys.Right), uDeltaTimeFloat);
            var c = CarManager.playerCar;

            game.camera.SmoothRotateTo(c.frontDirection);

            game.camera.SmoothMoveTo(c.position - c.frontDirection * 5
                + Vector3.Up * 5 
                - c.rightDirection * c.currentTurnRate * 1.35f);

            game.camera.Update(uDeltaTimeFloat);

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //DEFERRED RENDERING
            //G-BUFFERS
            game.basicModelEffect.SetView(game.camera.view);
            game.basicModelEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetView(game.camera.view);
            game.deferredEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetCameraPosition(game.camera.position);

            game.GraphicsDevice.SetRenderTargets(game.colorTarget, game.normalTarget, game.positionTarget, game.bloomFilterTarget);
            game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //game.skybox.Draw(game.camera.view, game.camera.projection, game.camera.position);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //DrawPlane();
            DrawMap();
            CarManager.DrawPlayerCar();

            game.lightsManager.DrawLightGeo();

            //game.gizmos.Draw();
            
            //LIGHT VOLUMES + BLUR
            game.GraphicsDevice.SetRenderTargets(game.lightTarget, game.blurHTarget, game.blurVTarget);
            game.GraphicsDevice.BlendState = BlendState.Additive;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            game.deferredEffect.SetColorMap(game.colorTarget);
            game.deferredEffect.SetNormalMap(game.normalTarget);
            game.deferredEffect.SetPositionMap(game.positionTarget);
            game.deferredEffect.SetBloomFilter(game.bloomFilterTarget);
            game.lightsManager.Draw();

            //game.hud.DrawMiniMapTarget(dDeltaTimeFloat);//TODO: shouldnt this be in final pass?

            //Final pass
            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            game.deferredEffect.SetLightMap(game.lightTarget);
            game.deferredEffect.SetScreenSize(new Vector2(game.lightTarget.Width, game.lightTarget.Height));
            game.deferredEffect.SetTech("integrate");
            game.deferredEffect.SetBlurH(game.blurHTarget);
            game.deferredEffect.SetBlurV(game.blurVTarget);

            game.spriteBatch.Begin();
            //game.spriteBatch.Draw(game.bloomFilterTarget, Vector2.Zero, Color.White);
            game.fullScreenQuad.Draw(game.deferredEffect.effect);

            //.ToString("F2")
            var str = $"{FPS}  {CarManager.playerCar.speed.ToString("F2")}";
                // p {game.camera.pitch.ToString("F2")} y {game.camera.yaw.ToString("F2")}";
            game.spriteBatch.DrawString(game.font25, str, Vector2.Zero, Color.White);
            game.spriteBatch.End();

        }

        void DrawPlane()
        {
            game.basicModelEffect.SetTech("basic_color");
            game.basicModelEffect.SetKA(0.3f);
            game.basicModelEffect.SetKD(0.9f);
            game.basicModelEffect.SetKS(0.8f);
            game.basicModelEffect.SetShininess(30f);
            game.basicModelEffect.SetColor(Color.White.ToVector3());
            game.basicModelEffect.SetTiling(Vector2.One);


            foreach (var mesh in plane.Meshes)
            {
                var w = mesh.ParentBone.Transform * Matrix.CreateScale(10f) * Matrix.CreateTranslation(0, 0, 0);
                game.basicModelEffect.SetWorld(w);
                game.basicModelEffect.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(w)));

                mesh.Draw();
            }
        }

        void DrawMap()
        {
            game.basicModelEffect.SetTech("number");
            game.basicModelEffect.SetKA(0.3f);
            game.basicModelEffect.SetKD(0.9f);
            game.basicModelEffect.SetKS(0.8f);
            game.basicModelEffect.SetShininess(30f);
            game.basicModelEffect.SetColor(Color.White.ToVector3());
            game.basicModelEffect.SetTiling(Vector2.One);


            foreach(var mesh in map.Meshes)
            {
                
                var w = mesh.ParentBone.Transform * Matrix.CreateScale(0.010f) * Matrix.CreateTranslation(0,-5,0);
                game.basicModelEffect.SetWorld(w);
                game.basicModelEffect.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(w)));

                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    game.basicModelEffect.SetColorTexture(numTex[i]);

                    foreach (var pass in game.basicModelEffect.effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        game.GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        game.GraphicsDevice.Indices = part.IndexBuffer;
                        game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        public override void LostFocus()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
        }
        public override void Focused()
        {
            game.IsMouseVisible = false;
            mouseLocked = true;
        }

    }
}
