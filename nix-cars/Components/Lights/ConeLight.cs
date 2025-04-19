using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nix_cars.Components.Effects;

namespace nix_cars.Components.Lights
{
    public class ConeLight : LightVolume
    {
        float width;
        float length;
        Vector3 scaleVec;
        public Vector3 direction;
        float yaw, pitch;
        public Matrix world;
        public ConeLight(Vector3 position, Vector3 direction, float width, float length, Vector3 color, Vector3 specularColor) : base(position, color, Vector3.Zero, specularColor)
        {
            this.width= width;
            this.length = length;
            this.position = position;
            this.direction = direction;

            collider = new BoundingSphere(position + direction * length / 2, length/2);

            yaw = MathF.Atan2(direction.X, direction.Z);
            pitch = MathF.Asin(direction.Y);

            scaleVec = new Vector3(width / 2, length, width / 2);
            scaleVec = scaleVec * 0.01f;
            world = Matrix.CreateScale(scaleVec) *
                Matrix.CreateFromYawPitchRoll(yaw, pitch - MathHelper.PiOver2, 0) *
                Matrix.CreateTranslation(position + direction * length / 2);

        }
        
        public override void Draw()
        {

            deferredEffect.SetLightDiffuseColor(color);
            deferredEffect.SetLightSpecularColor(specularColor);
            deferredEffect.SetLightPosition(position);
            deferredEffect.SetRadius(length);
            
            foreach (var mesh in lightCone.Meshes)
            {
                deferredEffect.SetWorld(mesh.ParentBone.Transform * world);

                //effect.Parameters["world"].SetValue(mesh.ParentBone.Transform * world);

                mesh.Draw();
            }
        }
        

        public override void Update()
        {
            collider.Center = position + direction * length / 2;
            
            yaw = MathF.Atan2(direction.X, direction.Z);
            pitch = MathF.Asin(direction.Y);

            world = Matrix.CreateScale(scaleVec) *
                Matrix.CreateFromYawPitchRoll(yaw, pitch - MathHelper.PiOver2, 0)*
                Matrix.CreateTranslation(position + direction * length / 2);
        }
    }
}
