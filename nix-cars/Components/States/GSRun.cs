using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.FlotatingTextures;
using nix_cars.Components.Cars;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using nix_cars.Components.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace nix_cars.Components.States
{
    public class GSRun : GameState
    {
        Model plane;
        float moveSpeed = 50f;
        Model map;
        Texture2D[] numTex;
        Texture2D[] peachMapTex;

        public GSRun() : base()
        {
            plane = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/plane");
            map = game.Content.Load<Model>(NixCars.ContentFolder3D + "maps/peach/peach");
            numTex = new Texture2D[101];
            for(int i = 0; i < 101; i++)
            {
                numTex[i] = game.Content.Load<Texture2D>(NixCars.ContentFolder3D + "basic/Tex/num/"+i);
            }
            
            LoadPeachMapTex();
            NixCars.AssignEffectToModel(plane, game.basicModelEffect.effect);
            NixCars.AssignEffectToModel(map, game.basicModelEffect.effect);

            //playerPosition = game.camera.position;
            game.camera.position = new Vector3(150.8f, 15, -159.1f);
            CollisionHelper.BuildMapCollider(map);

            obb2 = new OrientedBoundingBox(new Vector3(230f, 10, -320) + new Vector3(0, 0.75f, 0), new Vector3(1.125f, .7f, 2.5f));
            obb2.Orientation = Matrix.Identity;

        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = false;
            mouseLocked = true; 
        }
        
        bool mb1Down = false;
        bool mb2Down = false;
        bool cDown = false;
        int scrollValue;
        int scrollLastValue;

        List<Vector3> selectedPoints = new List<Vector3>();
        
        OrientedBoundingBox obb2;
        bool collided = false;

        
        public override void Update(GameTime gameTime)
        {
            var lp = CarManager.localPlayer;
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

            if(km.TAB.IsDown() && !keysDown.Contains(km.TAB))
            {
                keysDown.Add(km.TAB);

                game.camera.ToggleFree();
            }
            //if (km.CAPS.IsDown() && !keysDown.Contains(km.CAPS))
            //{
            //    keysDown.Add(km.CAPS);

            //    lp.Collided(new Vector3(10, 0, 10));
                
            //}

            
            

            if (!game.camera.isFree)
            {
                lp.Update(km.ForwardDown(), km.BackwardDown(), km.LeftDown(), km.RightDown(), km.Boost.IsDown(), uDeltaTimeFloat);

                game.camera.SmoothRotateTo(Vector3.Normalize(lp.frontDirection - Vector3.Up * 0.5f));

                game.camera.SmoothMoveTo(lp.position - lp.frontDirection * 5
                    + Vector3.Up * 5
                    - lp.rightDirection * lp.currentTurnRate * 1.35f);

                game.camera.Update(uDeltaTimeFloat);

            }
            else
            {
                lp.Update(km.Forward2.IsDown(), km.Backward2.IsDown(), km.Left2.IsDown(), km.Right2.IsDown(), km.Boost.IsDown(), uDeltaTimeFloat);

                game.camera.MoveBy(km.Forward.IsDown(), km.Backward.IsDown(), km.Left.IsDown(), km.Right.IsDown(),
                   keyState.IsKeyDown(Keys.Space), keyState.IsKeyDown(Keys.LeftControl),
                   keyState.IsKeyDown(Keys.LeftShift) ? moveSpeed : moveSpeed * 2, uDeltaTimeFloat);
                game.camera.RotateBy(mouseDelta);

            }
            
            CarManager.UpdatePlayers();

            MapWallCollision();
            FloorMapCollision();

            //PlayersCollision();

            //HighlightClosestVertex();

            
            if(mouseState.LeftButton == ButtonState.Pressed && !mb1Down)
            {
                mb1Down = true;

            }


            if (mouseState.LeftButton == ButtonState.Released)
            {
                mb1Down = false;
            }
            if (mouseState.RightButton == ButtonState.Pressed && !mb2Down)
            {
                mb2Down = true;


            }
            if (mouseState.RightButton == ButtonState.Released)
            {
                mb2Down = false;

            }

            if(mouseState.ScrollWheelValue != scrollLastValue)
            {
                var diff = mouseState.ScrollWheelValue - scrollLastValue;

            }

            if (keyState.IsKeyDown(Keys.C) && !cDown)
            {
                cDown = true;
            }
            if (keyState.IsKeyUp(Keys.C))
            {
                cDown = false;
            }

            
            FinishUpdate();
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
            
            CarManager.DrawPlayers();

            game.lightsManager.DrawLightGeo();

            var bc = CarManager.localPlayer.car.collider;
            var cubeMx = Matrix.CreateScale(bc.Extents * 2) * bc.Orientation * Matrix.CreateTranslation(bc.Center);
            //bc = CarManager.enemyCar.boxCollider;
            //var cubeMx2 = Matrix.CreateScale(bc.Extents * 2) * bc.Orientation * Matrix.CreateTranslation(bc.Center);

            if (collided)
            {
                game.gizmos.DrawCube(cubeMx, Color.Red);
                //game.gizmos.DrawCube(cubeMx2, Color.Red);
            }
            else
            {
                game.gizmos.DrawCube(cubeMx, Color.White);
                //game.gizmos.DrawCube(cubeMx2, Color.White);
            }

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

            // TODO: shouldnt this be in final pass?
            //game.hud.DrawMiniMapTarget(dDeltaTimeFloat);


            FlotatingTextureDrawer.Draw();

            //Final pass
            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            game.deferredEffect.SetLightMap(game.lightTarget);
            game.deferredEffect.SetScreenSize(new Vector2(game.lightTarget.Width, game.lightTarget.Height));
            game.deferredEffect.SetTech("integrate");
            game.deferredEffect.SetBlurH(game.blurHTarget);
            game.deferredEffect.SetBlurV(game.blurVTarget);

            game.fullScreenQuad.Draw(game.deferredEffect.effect);
            game.spriteBatch.Begin();

            game.spriteBatch.Draw(FlotatingTextureDrawer.target, Vector2.Zero, Color.White);

            var lp = CarManager.localPlayer;
            var pos = lp.position;

            var b = lp.boosting ? "B" : "-";
            var str = $"{FPS} {(int)lp.speed} {b} {lp.boostTimeRemaining.ToString("F2")}  ";

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
                
                var w = mesh.ParentBone.Transform * Matrix.CreateScale(CollisionHelper.MapScale) * Matrix.CreateTranslation(0,0,0);
                game.basicModelEffect.SetWorld(w);
                game.basicModelEffect.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(w)));

                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    
                    game.basicModelEffect.SetColorTexture(peachMapTex[i]);
                    //game.basicModelEffect.SetColorTexture(numTex[i]);

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
        

        void MapWallCollision()
        {
            var pc = CarManager.localPlayer;
            var bc = pc.car.collider;
            foreach (var t in CollisionHelper.mapWallTriangles)
            {

                if(bc.Intersects(t))
                {

                    var normal = t.GetNormal();
                    Vector3 toObbCenter = bc.Center - t.v[0];
                    if (Vector3.Dot(normal, toObbCenter) < 0)
                        normal = -normal;
                    
                    var angleCorrection = 0f;
                    var flatNormal = new Vector2(normal.X, normal.Z);
                    angleCorrection = 1 - Vector2.Dot(flatNormal, Vector2.Normalize(pc.frameHorizontalVelocity));


                    pc.position = pc.position + normal * angleCorrection * pc.thisFrameHorizontalDistance * 0.4f;

                    if (pc.speed > 0)
                        pc.speed -= uDeltaTimeFloat * 40f;
                    if (pc.speed < 0)
                        pc.speed += uDeltaTimeFloat * 40f;

                    //break;
                }
            }
        }


        void FloorMapCollision()
        {
            var pc = CarManager.localPlayer;
            var bc = pc.car.collider;
            foreach (var t in CollisionHelper.mapFloorTriangles)
            {

                if (bc.Intersects(t))
                {
                    
                    var normal = t.GetNormal();

                    if (normal.Y < 0)
                        normal = -normal;

                    var mul = 0f;
                    if(normal.Y <= .999f)
                    {
                        mul = pc.thisFrameHorizontalDistance;
                    }

                    pc.position = pc.position + normal * (pc.thisFrameVerticalDistance + mul);

                    break;
                }
            }
        }
        
        // TODO: server side.
        //void PlayersCollision()
        //{
        //    // Get references to the player and enemy cars.
        //    var pc = CarManager.playerCar;
        //    var ec = CarManager.enemyCar;

        //    // Check if the bounding boxes of the two cars intersect.
        //    if (pc.boxCollider.Intersects(ec.boxCollider))
        //    {
        //        // Calculate the horizontal direction from the player to the enemy.
        //        // We ignore the vertical component by forcing Y = 0.
        //        Vector3 collisionDirection = new Vector3(
        //            ec.position.X - pc.position.X,
        //            0f,
        //            ec.position.Z - pc.position.Z
        //        );

        //        // Prevent division by zero in case the cars are exactly at the same position.
        //        if (collisionDirection.LengthSquared() > 0.0001f)
        //        {
        //            collisionDirection.Normalize();
        //        }
        //        else
        //        {
        //            // Fallback direction if needed.
        //            collisionDirection = Vector3.Zero;
        //        }

        //        // Extract the player's current horizontal velocity (ignoring vertical movement).
        //        Vector3 playerHorizontalVelocity = new Vector3(pc.velocity.X, 0f, pc.velocity.Z);

        //        // You might want to use the magnitude of the player's horizontal velocity as a basis
        //        // for the impulse. Here we use an impulse factor to amplify the collision effect.
        //        float impulseFactor = 1.25f;

        //        // Compute the impulse: we multiply the normalized collision direction by the magnitude
        //        // of the player's horizontal velocity, scaled by an impulse factor.
        //        Vector3 impulse = collisionDirection * playerHorizontalVelocity.Length() * impulseFactor;

        //        // Signal the enemy car that it has been collided with.
        //        // This will set its collisionVelocity to the impulse vector.
        //        ec.Collided(impulse);

        //        // As a response to the collision, stop or reduce the player's speed.
        //        // You can also choose to reduce it by a percentage if you do not want an immediate stop.
        //        pc.speed *= 0.5f;
        //    }
        //}



        List<PointLight> testLights = new List<PointLight>();
        
        Vector3 closestVertex = Vector3.Zero;
        Vector3 closestNormal= Vector3.Zero;
        void HighlightClosestVertex()
        {
            game.lightsManager.Destroy(testLights);
            testLights.Clear();
            var ts = CollisionHelper.mapFloorTriangles.OrderBy(t => Vector3.DistanceSquared(CollisionHelper.Vec3Avg(t), game.camera.position));
            foreach (var t in ts)
            {
                var cam = game.camera;
                
                var hitPos = BoundingVolumesExtensions.IntersectRayWithTriangle(cam.position, cam.frontDirection, t.v[0], t.v[1], t.v[2]);
                if(hitPos.HasValue)
                {
                    var val = hitPos.Value;
                    closestVertex = t.v.MinBy(v => Vector3.DistanceSquared(v, val));

                    var pl = new PointLight(val, 0.5f, Vector3.One, Vector3.One);
                    pl.skipDraw = true;
                    pl.hasLightGeo = true;

                    testLights.Add(pl);
                    pl = new PointLight(closestVertex, 0.5f, Color.Green.ToVector3(), Color.Green.ToVector3());
                    pl.skipDraw = true;
                    pl.hasLightGeo = true;
                    testLights.Add(pl);

                    for(int i = 0; i< t.v.Length; i++)
                    {
                        var v = t.v[i];
                        if(v != closestVertex)
                        {
                            pl = new PointLight(v, 0.5f, Color.Yellow.ToVector3(), Color.Yellow.ToVector3());
                            pl.skipDraw = true;
                            pl.hasLightGeo = true;
                            testLights.Add(pl);
                        }
                    }
                    var normal = t.GetNormal();

                    if (normal.Y < 0)
                        normal = -normal;
                    //    Vector3 toObbCenter = CarManager.playerCar.boxCollider.Center - t.v[0];
                    //if (Vector3.Dot(normal, toObbCenter) < 0)
                    //    normal = -normal;


                    closestNormal = normal;

                    break;
                }
            }

            game.lightsManager.Register(testLights);
            
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
        void LoadPeachMapTex()
        {
            peachMapTex = new Texture2D[27];
            for (int i = 0; i < 27; i++)
            {
                peachMapTex[i] = numTex[i];
            }
            var path = NixCars.ContentFolder3D + "maps/peach/";
            peachMapTex[4] = game.Content.Load<Texture2D>(path + "flower_r");
            peachMapTex[6] = game.Content.Load<Texture2D>(path + "waterC");
            peachMapTex[7] = game.Content.Load<Texture2D>(path + "back_yam");
            peachMapTex[9] = game.Content.Load<Texture2D>(path + "back2");
            peachMapTex[11] = game.Content.Load<Texture2D>(path + "hasi2");
            peachMapTex[14] = game.Content.Load<Texture2D>(path + "flower_y");
            peachMapTex[15] = game.Content.Load<Texture2D>(path + "saku");
            peachMapTex[16] = game.Content.Load<Texture2D>(path + "suna");
            peachMapTex[20] = game.Content.Load<Texture2D>(path + "grass");
            peachMapTex[22] = game.Content.Load<Texture2D>(path + "renga");
            peachMapTex[23] = game.Content.Load<Texture2D>(path + "grass");
            peachMapTex[24] = game.Content.Load<Texture2D>(path + "suna3");
            peachMapTex[25] = game.Content.Load<Texture2D>(path + "grass3_2");
            peachMapTex[26] = game.Content.Load<Texture2D>(path + "grass4");
        }
    }
}
