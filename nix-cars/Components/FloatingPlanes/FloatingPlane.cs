using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.FloatingPlanes
{
    public abstract class FloatingPlane
    {
        RenderTarget2D target;
        Matrix world;
        public bool hasChanged;

        public float scale = 1;
        float scaleAdjust = 0.08f;
        Matrix mxScale;
        public NixCars game;

        public int x, y;

        public bool showThisFrame;
        public FloatingPlane()
        {
            game = NixCars.GameInstance();
        }
        public abstract void Draw(ref Effect effect);

        public void SetRT(Matrix rototranslation)
        {
            world = mxScale * rototranslation;

        }
        public void CreateTarget(int x, int y)
        {
            hasChanged = true;

            if (x == this.x && y == this.y)
                return;

            this.x = x;
            this.y = y;

            target = new RenderTarget2D(NixCars.GameInstance().GraphicsDevice, x, y,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            mxScale = Matrix.CreateScale(new Vector3(scale * scaleAdjust * x, 1f, scale * scaleAdjust * y));

            
        }

        public void SetTarget(RenderTarget2D target)
        {
            hasChanged = true;

            this.target = target;
            x = target.Bounds.Width;
            y = target.Bounds.Height;

            mxScale = Matrix.CreateScale(new Vector3(scale * scaleAdjust * x, 1f, scale * scaleAdjust * y));

        }
        public Matrix GetWorld() { return world; }
        public RenderTarget2D GetTarget() { return target; }

        public bool Changed()
        {
            bool ret = hasChanged;
            if (hasChanged)
                hasChanged = false;
            return ret;
        }
    }
}
