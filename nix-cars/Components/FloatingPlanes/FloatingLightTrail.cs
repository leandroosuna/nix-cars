using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace nix_cars.Components.FloatingPlanes
{
    public class FloatingLightTrail : FloatingPlane
    {
        Texture2D tex;
        int sectorWidth = 192;
        int sectorHeight = 576;
        int sectorPosY = 128;
        int sectorPosX = 128;
        
        float time = 0f;

        public FloatingLightTrail() : base()
        {
            showThisFrame = true;
            int x = 192, y = 576;
            CreateTarget(x, y);

            var r = new Random();
            time = (float)r.NextDouble();

            tex = game.Content.Load<Texture2D>(NixCars.ContentFolder3D + "other/boostTrail");
        }
        public void Update(bool boosting, float deltaTime)
        {

            showThisFrame = boosting;
            time += deltaTime;
            if(time >= .05f)
            {
                hasChanged = true;
                time = 0;
                sectorPosX += sectorWidth;

                if (sectorPosX == 1856)
                    sectorPosX = 128;
            }
            

        }

        public override void Draw(ref Effect effect)
        {
            game.spriteBatch.Begin();
            var sector = new Rectangle(sectorPosX, sectorPosY, sectorWidth, sectorHeight);
            game.spriteBatch.Draw(tex, Vector2.Zero, sector, Color.White);
            game.spriteBatch.End();
        }

        
        
    }


}
