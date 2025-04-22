using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nix_cars.Components.FlotatingTextures
{
    public class FlotatingText
    {
        RenderTarget2D target;
        Matrix world;
        bool changed;
        string text = "";
        Color textColor = Color.White;
        public SpriteFont font;
        public float scale;
        float scaleAdjust = 0.08f;
        Matrix mxScale;
        public FlotatingText()
        {
            scale = 1;
            font = NixCars.GameInstance().font25;
        }
        public void SetFont(SpriteFont font)
        { 
            this.font = font; 
        }
        public void SetTextColor(Color color)
        {
            textColor = color;
            SetText(text);
        }
        public void SetText(string text, float scale = 1)
        {
            if(this.text == text)
            {
                return;
            }
            this.text = text;
            Vector2 size = font.MeasureString(text);

            var x = (int)size.X;
            var y = (int)size.Y;
            
            target = new RenderTarget2D(NixCars.GameInstance().GraphicsDevice, x, y,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            mxScale = Matrix.CreateScale(new Vector3(scale * scaleAdjust * x, 1f , scale * scaleAdjust * y));
            
            changed = true;
        
        }
        public void DrawText(ref SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, Vector2.Zero, textColor);
        }
        public void SetRT(Matrix rototranslation)
        {
            world = mxScale * rototranslation;

        }

        public Matrix GetWorld() {  return world; }
        public RenderTarget2D GetTarget() { return target; }

        public bool Changed() {
            bool ret = changed;
            if (changed)
                changed = false;
            return ret; 
        }
        
    }


}
