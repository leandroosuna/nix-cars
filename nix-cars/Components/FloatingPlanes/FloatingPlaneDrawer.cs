using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace nix_cars.Components.FloatingPlanes
{
    public class FloatingPlaneDrawer
    {
        static List<FloatingPlane> floatingPlanes = new List<FloatingPlane>();

        static Model plane;
        static NixCars game;
        static Effect effect;
        public static RenderTarget2D target;
        public static void Init()
        {
            game = NixCars.GameInstance();

            plane = game.Content.Load<Model>(NixCars.ContentFolder3D+"basic/plane");
            effect = game.Content.Load<Effect>(NixCars.ContentFolderEffects+ "floating");
            effect.Parameters["screenSize"].SetValue(new Vector2(game.screenWidth, game.screenHeight));

            NixCars.AssignEffectToModel(plane, effect);
            target = new RenderTarget2D(game.GraphicsDevice, game.screenWidth, game.screenHeight);

        }
        // TODO: floating boost fix on res change
        public static void ResolutionChange(int w, int h)
        {
            effect.Parameters["screenSize"].SetValue(new Vector2(w,h));

        }
        public static void Add(FloatingPlane plane)
        {
            lock(floatingPlanes)
            {
                floatingPlanes.Add(plane);
            }
        }
        public static void Remove(FloatingPlane plane)
        {
            lock(floatingPlanes)
            {
                floatingPlanes.Remove(plane);
            }
        }
        public static void Draw()
        {
            effect.Parameters["view"]?.SetValue(game.camera.view);
            effect.Parameters["projection"]?.SetValue(game.camera.projection);
            effect.Parameters["time"]?.SetValue(game.gameState.uTotalTime);

            lock (floatingPlanes)
            {
                foreach (var ft in floatingPlanes)
                {
                    if (ft.Changed())
                    {
                        DrawIntoFloatingPlane(ft);
                    }
                }
            }
            game.GraphicsDevice.SetRenderTarget(target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1f, 0);
            game.GraphicsDevice.BlendState = BlendState.Additive;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            lock (floatingPlanes)
            {
                foreach (var b in floatingPlanes)
                {
                    if(b.showThisFrame)
                        DrawFloatingPlane(b);
                }
            }
        }
        static void DrawIntoFloatingPlane(FloatingPlane b)
        {
            game.GraphicsDevice.SetRenderTarget(b.GetTarget());
            game.GraphicsDevice.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.Transparent,    // blank slate
                1f,
                0
            );

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            b.Draw(ref effect);
        }
        
        static void DrawFloatingPlane(FloatingPlane b)
        {
            effect.CurrentTechnique = effect.Techniques["plane"];
            foreach (var mesh in plane.Meshes)
            {
                var w = mesh.ParentBone.Transform * b.GetWorld();
                effect.Parameters["world"]?.SetValue(w);
                effect.Parameters["colorTexture"]?.SetValue(b.GetTarget());

                mesh.Draw();
            }
            
        }
    }
}
