using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace nix_cars.Components.FlotatingTextures
{
    public class FlotatingTextureDrawer
    {
        // TODO: generic flotatingTexture, to have 
        static List<FlotatingText> flotatingTexts = new List<FlotatingText>();

        static Model plane;
        static NixCars game;
        static Effect effect;
        public static RenderTarget2D target;
        public static void Init()
        {
            game = NixCars.GameInstance();

            plane = game.Content.Load<Model>(NixCars.ContentFolder3D+"basic/plane");
            effect = game.Content.Load<Effect>(NixCars.ContentFolderEffects+ "banner");

            NixCars.AssignEffectToModel(plane, effect);
            target = new RenderTarget2D(game.GraphicsDevice, game.screenWidth, game.screenHeight);

        }

        public static void AddText(FlotatingText banner)
        {
            lock(flotatingTexts)
            {
                flotatingTexts.Add(banner);
            }
        }
        public static void RemoveText(FlotatingText banner)
        {
            lock(flotatingTexts)
            {
                flotatingTexts.Remove(banner);
            }
        }
        public static void Draw()
        {
            effect.Parameters["view"]?.SetValue(game.camera.view);
            effect.Parameters["projection"]?.SetValue(game.camera.projection);

            lock(flotatingTexts)
            {
                foreach (var b in flotatingTexts)
                {
                    if (b.Changed())
                    {
                        DrawIntoBannerTex(b);
                    }
                }
            }
            game.GraphicsDevice.SetRenderTarget(target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1f, 0);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            lock(flotatingTexts)
            {
                foreach (var b in flotatingTexts)
                    DrawBanner(b);
            }
        }
        static void DrawIntoBannerTex(FlotatingText b)
        {
            game.GraphicsDevice.SetRenderTarget(b.GetTarget());
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.Transparent, 1f, 0);
            game.spriteBatch.Begin();
            b.DrawText(ref game.spriteBatch);
            game.spriteBatch.End();
        }
        
        static void DrawBanner(FlotatingText b)
        {
            foreach(var mesh in plane.Meshes)
            {
                var w = mesh.ParentBone.Transform * b.GetWorld();
                effect.Parameters["world"]?.SetValue(w);

                effect.Parameters["colorTexture"]?.SetValue(b.GetTarget());

                mesh.Draw();
            }
            
        }
    }
}
