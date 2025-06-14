﻿using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.FloatingPlanes;
using nix_cars.Components.Cars;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using nix_cars.Components.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

using nix_cars.Components.GUI;
using nix_cars.Screens;
using Riptide;
using nix_cars.Components.Sound;


namespace nix_cars.Components.States
{
    public class GSRun : GameState
    {
        Model plane;
        float moveSpeed = 50f;
        Model map;
        Texture2D[] numTex;
        Texture2D[] peachMapTex;
        Model rocket;
        Texture2D rocketTex;
        RaceHud rh;
        LocalPlayer lp;
        public string gameMode;
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

            rocket = game.Content.Load<Model>(NixCars.ContentFolder3D + "other/rocket");
            rocketTex = game.Content.Load<Texture2D>(NixCars.ContentFolder3D + "other/1001_Base_Color");

            NixCars.AssignEffectToModel(plane, game.basicModelEffect.effect);
            NixCars.AssignEffectToModel(map, game.basicModelEffect.effect);
            NixCars.AssignEffectToModel(rocket, game.basicModelEffect.effect);

            //playerPosition = game.camera.position;
            game.camera.position = new Vector3(150.8f, 15, -159.1f);
            CollisionHelper.BuildMapCollider(map);

            obb2 = new OrientedBoundingBox(new Vector3(230f, 10, -320) + new Vector3(0, 0.75f, 0), new Vector3(1.125f, .7f, 2.5f));
            obb2.Orientation = Matrix.Identity;

            var sp = LoadPositions("Files/map-spline.xml");
            //sp.Add(sp[0]);
            mapSpline = sp.ToArray();

            lp = CarManager.localPlayer;

        }
        public override void OnSwitch()
        {
            //game.IsMouseVisible = false;
            //mouseLocked = true; 
            //GumManager.Clear();
            GumManager.SwitchTo(Screen.RACEHUD);
            rh = GumManager.GetRaceHud();
            NetworkManager.SendCarChange();
            rh.TitleMsg.Visible = true;
        }
        
        bool mb1Down = false;
        bool mb2Down = false;
        bool cDown = false;
        int scrollValue;
        int scrollLastValue;

        List<Vector3> selectedPoints = new List<Vector3>();
        
        OrientedBoundingBox obb2;
        bool collided = false;

        float timerS = 0f;

        bool mappingSpline = false;
        bool prevMappingSpline = false;

        bool endMapping = false;
        Vector3[] mapSpline;
        float secTimer = 0f;

        bool commandMode = false;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            secTimer += uDeltaTimeFloat;

            if (km.KeyDownOnce(km.Escape))
            {
                if (GumManager.CurrentScreenIs(Screen.RACEHUD))
                    GumManager.SwitchTo(Screen.OPTIONS);
                else
                {
                    GumManager.SwitchTo(Screen.RACEHUD);
                    rh = GumManager.GetRaceHud();
                }
                //TODO: fix racehud restore after options
            }

            //if (km.KeyDownOnce(km.CAPS))
            //{
            //    game.camera.ToggleFree();
               
            //}

            if(km.KeyDownOnce(km.Enter))
            {
                commandMode = !commandMode;
                
                var cb = rh.CommandBox;
                var svr = rh.ServerResponse;
                if (commandMode)
                {
                    cb.Text = "";
                    //cb.IsVisible = true;
                    cb.Visual.Visible = true;
                    cb.IsFocused = true;
                    svr.Text = "";
                    svr.Visible = true;
                }
                else
                {
                    //cb.IsVisible = true;
                    cb.Visual.Visible = false;
                    svr.Visible = false;
                    
                    if(cb.Text != "")
                        NetworkManager.SendCommand(cb.Text);
                    //Debug.WriteLine(cb.Text);
                }
            }
            
            if(km.KeyDownOnce(km.Reset) && !commandMode)
            {
                ResetToClosestInSpline();
            }
            //if(mappingSpline)
            //{
            //    if (secTimer >= 1)
            //    {
            //        secTimer = 0f;
            //        positions.Add(lp.position);

            //        var pl = new PointLight(lp.position + Vector3.Up * 1, 3f, Vector3.One, Vector3.One);
            //        pl.skipDraw = true;
            //        pl.hasLightGeo = true;

            //        game.lightsManager.Register(pl);
            //    }

            //}

            if(!commandMode)
            {
                if (!game.camera.isFree)
                {

                    lp.Update(km.ForwardDown() || mappingSpline, km.BackwardDown(), km.LeftDown(), km.RightDown(), km.Boost.IsDown(), uDeltaTimeFloat);
                }
                else
                {
                    lp.Update(km.Forward2.IsDown(), km.Backward2.IsDown(), km.Left2.IsDown(), km.Right2.IsDown(), km.Boost.IsDown(), uDeltaTimeFloat);
                }
            }
            else
            {
                lp.Update(false,false,false,false,false, uDeltaTimeFloat);
            }
            CarManager.UpdatePlayers();

            MapWallCollision();
            FloorMapCollision();
            lp.PostCollisionUpdate(uDeltaTimeFloat);
            LapProgress();
            UpdateLapPositions();
            SoundManager.EngineSound(lp, uDeltaTimeFloat);
            if (!game.camera.isFree)
            {
                game.camera.SmoothRotateTo(Vector3.Normalize(lp.frontDirection - Vector3.Up * 0.5f));

                game.camera.SmoothMoveTo(lp.position - lp.frontDirection * 5
                    + Vector3.Up * 5
                    - lp.rightDirection * lp.currentTurnRate * 1.35f);

                game.camera.Update(uDeltaTimeFloat);
            }
            else
            {
                
                game.camera.MoveBy(km.Forward.IsDown(), km.Backward.IsDown(), km.Left.IsDown(), km.Right.IsDown(),
                   keyState.IsKeyDown(Keys.Space), keyState.IsKeyDown(Keys.LeftControl),
                   keyState.IsKeyDown(Keys.LeftShift) ? moveSpeed : moveSpeed * 2, uDeltaTimeFloat);
                game.camera.RotateBy(mouseDelta);
            }

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
            timerS += uDeltaTimeFloat;


            //var a1 = NixCars.GumRoot.GetGraphicalUiElementByName("HitIndicator") as SpriteRuntime;
            //if(timerS >= .08f)
            //{
            //    timerS = 0;


            //    a1.TextureLeft += 32;

            //    if (a1.TextureLeft == 128)
            //        a1.TextureLeft = 0;

            //}
            //var screenPos = game.GraphicsDevice.Viewport.Project(lp.position, game.camera.projection, game.camera.view, Matrix.Identity);
            //a1.X = Math.Clamp(screenPos.X,25,game.screenWidth -25);
            //a1.Y = Math.Clamp(screenPos.Y, 25, game.screenHeight -25);


            //var cb = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<TextBox>(gue, "CommandBox");
            //var cb = NixCars.GumRoot.GetGraphicalUiElementByName("CommandBox");
            //var textBox = cb. as TextBox;

            // TODO: position lock after race end, spin around and show end position. spectate?

            FinishUpdate();
        }

        private void ResetToClosestInSpline()
        {
            var sp = mapSpline[mapSplineCurrentIndex];
            var spN = mapSpline[getSplineIndex(mapSplineCurrentIndex, +1)];
            var dir = Vector3.Normalize(spN - sp);
            lp.TP(sp + Vector3.Up, dir);
            lp.speed = 0;
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
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            DrawMap();
            
            //DrawRocket();
            CarManager.DrawPlayers();


            game.lightsManager.DrawLightGeo();

            var bc = CarManager.localPlayer.car.collider;
            var cubeMx = Matrix.CreateScale(bc.Extents * 2) * bc.Orientation * Matrix.CreateTranslation(bc.Center);
            
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


            FloatingPlaneDrawer.DrawToPlaneTexs();

            //Final pass
            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Color.Transparent);
            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            game.deferredEffect.SetLightMap(game.lightTarget);
            game.deferredEffect.SetScreenSize(new Vector2(game.lightTarget.Width, game.lightTarget.Height));
            game.deferredEffect.SetTech("integrate");
            game.deferredEffect.SetBlurH(game.blurHTarget);
            game.deferredEffect.SetBlurV(game.blurVTarget);

            game.fullScreenQuad.Draw(game.deferredEffect.effect);
            FloatingPlaneDrawer.DrawPlanes();
            game.spriteBatch.Begin();

            var lp = CarManager.localPlayer;
            var pos = lp.position;
            var cpos = game.camera.position;

            var th = triHitID == uint.MaxValue ? "" : $"{triHitID}";

            
            rh.FPS.Text = $"{FPS}";
            if (FPS < 30)
                rh.FPS.Color = Color.Red;
            else if (FPS < 60)
                rh.FPS.Color = Color.Yellow;
            else if (FPS < 100)
                rh.FPS.Color = Color.Green;
            else
                rh.FPS.Color = Color.Cyan;

            var rtt = NetworkManager.Client.RTT;

            if (rtt == -1)
            {
                rh.RTT.Text = "offline";
                rh.RTT.Color = Color.Gray;
            }
            else
            {
                rh.RTT.Text = $"{rtt} ms";
                if(rtt < 20)
                    rh.RTT.Color = Color.Cyan;
                else if(rtt < 40)
                    rh.RTT.Color = Color.Green;
                else if (rtt < 100)
                    rh.RTT.Color = Color.Yellow;
                else 
                    rh.RTT.Color = Color.Red;


            }
            var e1 = SoundManager.soundEngine1Instance;
            var e2 = SoundManager.soundEngine2Instance;

            var topLeft = $"";
            //var topLeft = $"{e1.State.ToString()} {e1.Volume} {e1.Pitch}  {e2.State.ToString()} {e2.Volume} {e2.Pitch}";


            game.spriteBatch.DrawString(game.font25, topLeft, Vector2.Zero, Color.White);

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
            game.basicModelEffect.SetKS(0.3f);
            game.basicModelEffect.SetShininess(5f);
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

        void DrawRocket()
        {
            game.basicModelEffect.SetTech("colorTex_lightEn");
            game.basicModelEffect.SetKA(0.3f);
            game.basicModelEffect.SetKD(0.9f);
            game.basicModelEffect.SetKS(0.8f);
            game.basicModelEffect.SetShininess(30f);
            game.basicModelEffect.SetColorTexture(rocketTex);

            var lp = CarManager.localPlayer;
            foreach (var mesh in rocket.Meshes)
            {
                var w = mesh.ParentBone.Transform * Matrix.CreateScale(1f) *
                    Matrix.CreateFromYawPitchRoll(lp.yaw, +MathHelper.PiOver2, 0) * 
                    Matrix.CreateTranslation(lp.position - lp.frontDirection * 4f + Vector3.Up * 1f);
                var itw = Matrix.Invert(Matrix.Transpose(w));

                game.basicModelEffect.SetWorld(w);
                game.basicModelEffect.SetInverseTransposeWorld(itw);

                mesh.Draw();
            }
        }

        int mapSplineCurrentIndex = -1;
        float progressFrom;
        float progressTo;
        bool lastFrameDeadzone = false;
        bool dzInFacingForward = false;
        bool dzInGoingForward = false;
        float facingFrontYaw = -1.38f;

        void LapProgress()
        {
            var lp = CarManager.localPlayer;
            var pos = lp.position;

            var mapLength = mapSpline.Length;
            if(mapSplineCurrentIndex == -1)
            {
                float minDistance = float.MaxValue;
                for(int i = 0; i < mapLength; i++) 
                {
                    var d = Vector3.DistanceSquared(pos, mapSpline[i]);
                    if (d <= minDistance)
                    {
                        minDistance = d;
                        mapSplineCurrentIndex = i;
                    }
                }
            }
            else
            {
                var iN = getSplineIndex(mapSplineCurrentIndex, +1);
                var iP = getSplineIndex(mapSplineCurrentIndex, -1);

                var d = Vector3.DistanceSquared(pos, mapSpline[mapSplineCurrentIndex]);
                var dN = Vector3.DistanceSquared(pos, mapSpline[iN]);
                var dP = Vector3.DistanceSquared(pos, mapSpline[iP]);

                if (dN < d)
                    mapSplineCurrentIndex = iN;
                else if(dP < d)
                    mapSplineCurrentIndex = iP;


            }
            var prevIndex = getSplineIndex(mapSplineCurrentIndex, -1);
            var nextIndex = getSplineIndex(mapSplineCurrentIndex, +1);

            var prev = mapSpline[prevIndex];
            var current = mapSpline[mapSplineCurrentIndex];
            var next = mapSpline[nextIndex];

            var dPrev = Vector3.DistanceSquared(pos, prev);
            var dNext = Vector3.DistanceSquared(pos, next);

            Vector3 p0 = Vector3.Zero;
            Vector3 a = Vector3.Zero;
            float totD;

            if (dPrev < dNext)
            {
                p0 = prev;
                a = current - prev;
                progressFrom = (float)prevIndex / mapLength;
                progressTo = (float)mapSplineCurrentIndex / mapLength;

                totD = Vector3.DistanceSquared(prev, current);
            }
            else
            {
                p0 = current;
                a = next - current;
                progressFrom = (float)mapSplineCurrentIndex / mapLength;
                progressTo = (float)nextIndex / mapLength;
                
                totD = Vector3.DistanceSquared(current, next);
            }

            bool deadzone = progressFrom >= 0.99 && progressTo == 0;
            if (deadzone)
            {
                if(!lastFrameDeadzone)
                {
                    lastFrameDeadzone = true;
                    dzInFacingForward = Math.Abs(lp.yaw - facingFrontYaw) <= MathHelper.PiOver2;
                    dzInGoingForward = lp.speed >= 0;

                }
                lp.progress = 0.99f;

                
                return;
            }
            else
            {
                if (lastFrameDeadzone)
                {
                    lastFrameDeadzone = false;
  
                    var dzOutFacingForward = Math.Abs(lp.yaw - facingFrontYaw) <= MathHelper.PiOver2;
                    var dzOutGoingForward = lp.speed >= 0;
                    //Debug.WriteLine("");
                    if ((dzInFacingForward && dzInGoingForward && dzOutFacingForward && dzOutGoingForward) ||
                        (!dzInFacingForward && !dzInGoingForward && !dzOutFacingForward && !dzOutGoingForward))
                    {
                        //Debug.WriteLine("+1 LAP");
                        if (gameMode == "race")
                        {
                            //Debug.WriteLine("sent");
                            NetworkManager.SendLap(true);
                        }
                    }
                    else if ((dzInFacingForward && !dzInGoingForward && dzOutFacingForward && !dzOutGoingForward) ||
                        (!dzInFacingForward && dzInGoingForward && !dzOutFacingForward && dzOutGoingForward))
                    {
                        //Debug.WriteLine("-1 LAP");
                        if (gameMode == "race")
                        {
                            //Debug.WriteLine("sent");
                            NetworkManager.SendLap(false);
                        }
                    }    
                    else
                    {
                        //Debug.WriteLine($"no change inf {dzInFacingForward} ins {dzInGoingForward} of {dzOutFacingForward} os {dzOutGoingForward}");
                    }

                    
                }
            }
           

            var p0p = pos - p0;

            var dot = Vector3.Dot(p0p, a);

            var lsq = a.LengthSquared();

            var proj = (dot / lsq) * a + p0;

            float lerpFactor = Vector3.DistanceSquared(p0, proj) / totD;
            
            
            lp.progress = float.Lerp(progressFrom, progressTo, lerpFactor);

            
        }

        int getSplineIndex(int index, int delta)
        {

            var ret = index + delta;
            if (ret > (mapSpline.Length - 1))
                return ret % mapSpline.Length;
            if (ret < 0)
                return ret + mapSpline.Length;
            
            return ret;
        }


        void MapWallCollision()
        {
            var pc = CarManager.localPlayer;
            var bc = pc.car.collider;

            foreach (var t in CollisionHelper.mapWallTriangles)
            {                
                if (bc.Intersects(t))
                {

                    var normal = t.GetNormal();
                    Vector3 toObbCenter = bc.Center - t.v[0];
                    if (Vector3.Dot(normal, toObbCenter) < 0)
                        normal = -normal;
                    
                    var angleCorrection = 0f;
                    var flatNormal = new Vector2(normal.X, normal.Z);
                    
                    Vector3 newPos;

                    if (pc.frameHorizontalVelocity != Vector2.Zero)
                    {
                        angleCorrection = 1 - Vector2.Dot(flatNormal, Vector2.Normalize(pc.frameHorizontalVelocity));
                        newPos = pc.position + normal * angleCorrection * pc.thisFrameHorizontalDistance * 0.4f;
                    }
                    else
                    {
                        newPos = pc.position + normal * uDeltaTimeFloat;
                    }

                    pc.position = newPos;

                    if (pc.speed > 0)
                        pc.speed -= uDeltaTimeFloat * 40f;
                    if (pc.speed < 0)
                        pc.speed += uDeltaTimeFloat * 40f;

                }
            }
        }

        uint triHitID;
        void FloorMapCollision()
        {
            var lp = CarManager.localPlayer;
            var targetDistance = 1f;
            triHitID = uint.MaxValue;
            foreach (var t in CollisionHelper.mapFloorTriangles)
            {
                var hitPos = BoundingVolumesExtensions.IntersectRayWithTriangle(
                    lp.position + Vector3.Up * targetDistance,
                    Vector3.Down, t.v[0], t.v[1], t.v[2]);
                if (hitPos.HasValue)                    
                {
                    var hit = hitPos.Value;
                    triHitID = t.id;
                    
                    var newPos = hit + Vector3.Up * 0.2f;
                    if (lp.position.Y <= newPos.Y + 0.1f)
                    {
                        CheckTriID(t.id);
                        lp.position = newPos;
                    }
                    break;
                }
            }
        }


        uint []slowGrassIDs = [772, 773, 774, 775, 776, 777, 780, 797, 781, 
            782, 783, 784, 791, 792, 793, 794, 795, 799, 804, 805, 807, 808, 784];
        
        void CheckTriID(uint id)
        {

            if (id >= 34 && id <= 39)
            {
                CarManager.localPlayer.Collided(Vector3.Up * 150f);
                CarManager.localPlayer.speed *= 0.5f;
            }

            if (slowGrassIDs.Any(i => i == id))
            {
                if (CarManager.localPlayer.speed >= 10)
                {
                    CarManager.localPlayer.speed *= 0.8f;
                }

            }

        }
        public override void OnResolutionChange(int w, int h)
        {
            FloatingPlaneDrawer.ResolutionChange(w, h);
        }

        public void GameModeChange(Message message)
        {
            var str = message.GetString();
            gameMode = str;
            switch (str)
            {
                case "free":
                    lp.positionLocked = false;
                    
                    rh.TitleMsg.Text = "sos libre";
                    rh.TitleMsg.Visible = true;
                    rh.Lap.Visible = false;
                    rh.Positions.Visible = false;
                    break;
                case "race":
                    
                    rh.TitleMsg.Text = "Se pistea en";
                    lp.lapCount = message.GetUShort();
                    rh.Positions.Visible = true;
                    break;
                
            }
            
        }
        ushort countdown = 3;
        public void Countdown(ref Message msg)
        {
            ushort t = msg.GetUShort();
            countdown = t;
            rh.Countdown.Text = $"{t}";

            if (countdown == 3)
            {
                for(int i = 0; i < NetworkManager.playerCount +1; i++)
                {
                    var id = msg.GetUInt();
                    var pos = msg.GetVector3();
                    if(id == lp.id)
                    {
                        lp.TP(pos, Vector3.Normalize(new Vector3(-.98f, 0, .2f)));
                    }

                }
                lp.positionLocked = true;
                rh.Countdown.Visible = true;
                lock(CarManager.onlinePlayers)
                {

                    CarManager.onlinePlayers.ForEach(op => op.lap = 0);
                }
            }
            if (countdown == 0)
            {
                rh.Countdown.Text = "GO";
                rh.Countdown.UpdateToFontValues();
                lp.positionLocked = false;
                rh.Positions.Visible = true;
                UpdateLapPositions();
                
                rh.Lap.Visible = true;
            }
            if (countdown == 1000)
            {
                rh.Countdown.Visible = false;
                rh.TitleMsg.Visible = false;


            }
        }
        
        public void UpdateLapPositions()
        {
            var str = "";
            lock (CarManager.onlinePlayers)
            {
                var ord = CarManager.onlinePlayers.OrderByDescending(p => p.lap);
                ord = ord.OrderByDescending(p => p.progress);
                List<Player> ps = ord.ToList();
                uint pos = 1;
                var len = ord.Count();
                var lapCount = CarManager.localPlayer.lapCount;
                foreach (var p in ps)
                {
                    var lapProgress = 0f;
                    if(p.lap >= 1 )
                    {
                        lapProgress = (p.progress + p.lap - 1);
                    }
                    
                    var overallProgress = (int)(lapProgress * 100 / lapCount);

                    str += $"{pos}- {p.name} {overallProgress}%\n";
                }
            }
            rh.Positions.Text = str;

            rh.Lap.Text = $"Vueltas {lp.lap} / {lp.lapCount}";

        }
        internal void SetServerRespose(string str)
        {
            rh.ServerResponse.Text = str;
            rh.ServerResponse.Visible = true;
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
        //uint triHitID;
        void HighlightClosestVertex()
        {
            game.lightsManager.Destroy(testLights);
            testLights.Clear();
            //triHitID = uint.MaxValue;
            var ts = CollisionHelper.mapFloorTriangles.OrderBy(t => Vector3.DistanceSquared(CollisionHelper.Vec3Avg(t), game.camera.position));
            foreach (var t in ts)
            {
                var cam = game.camera;
                
                var hitPos = BoundingVolumesExtensions.IntersectRayWithTriangle(cam.position, cam.frontDirection, t.v[0], t.v[1], t.v[2]);
                if(hitPos.HasValue)
                {
                    //triHitID = t.id;
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

        bool mouseVisibleSaveState;
        bool mouseLockedSaveState;

        public override void LostFocus()
        {
            mouseVisibleSaveState = game.IsMouseVisible;
            mouseLockedSaveState = mouseLocked;

            game.IsMouseVisible = true;
            mouseLocked = false;
        }
        public override void Focused()
        {
            game.IsMouseVisible = mouseVisibleSaveState;
            mouseLocked = mouseLockedSaveState;
        }
        void LoadPeachMapTex()
        {
            peachMapTex = new Texture2D[27];
            for (int i = 0; i < 27; i++)
            {
                peachMapTex[i] = numTex[i];
            }
            var path = NixCars.ContentFolder3D + "maps/peach/";
            
            peachMapTex[0] = game.Content.Load<Texture2D>(path + "goal");
            peachMapTex[1] = game.Content.Load<Texture2D>(path + "heri");
            peachMapTex[2] = game.Content.Load<Texture2D>(path + "start"); 
            peachMapTex[3] = game.Content.Load<Texture2D>(path + "wall01");
            peachMapTex[4] = game.Content.Load<Texture2D>(path + "flower_r");
            peachMapTex[5] = game.Content.Load<Texture2D>(path + "wall02");
            peachMapTex[6] = game.Content.Load<Texture2D>(path + "waterC");
            peachMapTex[7] = game.Content.Load<Texture2D>(path + "back_yam");
            peachMapTex[8] = game.Content.Load<Texture2D>(path + "ya");
            peachMapTex[9] = game.Content.Load<Texture2D>(path + "back2");
            peachMapTex[10] = game.Content.Load<Texture2D>(path + "tree_s");
            peachMapTex[11] = game.Content.Load<Texture2D>(path + "hasi2");
            peachMapTex[12] = game.Content.Load<Texture2D>(path + "roof");
            peachMapTex[13] = game.Content.Load<Texture2D>(path + "grass3_2");
            peachMapTex[14] = game.Content.Load<Texture2D>(path + "flower_y");
            peachMapTex[15] = game.Content.Load<Texture2D>(path + "saku");
            peachMapTex[16] = game.Content.Load<Texture2D>(path + "suna");
            peachMapTex[17] = game.Content.Load<Texture2D>(path + "flower_4");
            peachMapTex[18] = game.Content.Load<Texture2D>(path + "wall01");
            peachMapTex[19] = game.Content.Load<Texture2D>(path + "NemuMo5");
            peachMapTex[20] = game.Content.Load<Texture2D>(path + "grass");
            peachMapTex[21] = game.Content.Load<Texture2D>(path + "win");
            peachMapTex[22] = game.Content.Load<Texture2D>(path + "renga");
            peachMapTex[23] = game.Content.Load<Texture2D>(path + "grass");
            peachMapTex[24] = game.Content.Load<Texture2D>(path + "suna3");
            peachMapTex[25] = game.Content.Load<Texture2D>(path + "grass3_2");
            peachMapTex[26] = game.Content.Load<Texture2D>(path + "grass4");
        }
        public void SavePositions(List<Vector3> positions, string filePath)
        {
            var serializablePositions = positions.Select(p => new SerializableVector3(p)).ToList();
            var serializer = new XmlSerializer(typeof(List<SerializableVector3>));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, serializablePositions);
            }
        }
        public List<Vector3> LoadPositions(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<SerializableVector3>));
            using (var reader = new StreamReader(filePath))
            {
                var serializablePositions = (List<SerializableVector3>)serializer.Deserialize(reader);
                return serializablePositions.Select(s => s.ToVector3()).ToList();
            }
        }

        
    }
    [Serializable]
    public class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public SerializableVector3() { }

        public SerializableVector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector3 ToVector3() => new Vector3(X, Y, Z);
    }

}
