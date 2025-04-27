using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace nix_cars.Components.FloatingPlanes
{
    public class FloatingLightTrail : FloatingPlane
    {
        public FloatingLightTrail() : base()
        {
            showThisFrame = true;
            int x = 20, y = 200;
            CreateTarget(x, y);
        }
        
        public override void Draw(ref Effect effect)
        {
            effect.Parameters["resolution"].SetValue(new Vector2(x, y));
            effect.CurrentTechnique = effect.Techniques["lightTrail"];
            game.fullScreenQuad.Draw(effect);
        }

        
        
    }


}
