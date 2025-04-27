using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace nix_cars.Components.FloatingPlanes
{
    public class FloatingBoostMeter : FloatingPlane
    {
        public float boostValue = .5f;
        public FloatingBoostMeter() : base()
        {
            int x = 300, y = 250;
            CreateTarget(x, y);

        }
        public void SetBoostValue(float b)
        {
            boostValue = b;
            hasChanged = true;

            showThisFrame = b < 1;
        }

        public override void Draw(ref Effect effect)
        {
            effect.CurrentTechnique = effect.Techniques["boost"];
            effect.Parameters["boostValue"].SetValue(boostValue);
            game.fullScreenQuad.Draw(effect);
        }

        
        
    }


}
