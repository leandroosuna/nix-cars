using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nix_cars.Components.FloatingPlanes
{
    public class FloatingText : FloatingPlane
    {
        string text = "";
        Color textColor = Color.White;
        public SpriteFont font;
        public FloatingText() : base()
        {
            showThisFrame = true;
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

            CreateTarget((int)size.X, (int)size.Y);
        }
        public override void Draw(ref Effect effect)
        {
            game.spriteBatch.Begin();
            game.spriteBatch.DrawString(font, text, Vector2.Zero, textColor);
            game.spriteBatch.End();
        }

        
        
    }


}
