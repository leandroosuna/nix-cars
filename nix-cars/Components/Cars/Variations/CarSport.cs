using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Cars
{
    public class CarSport : Car
    {
        public override void LoadModel()
        {
            base.LoadModel();
            (id, model) = CarManager.GetModel("sport");
            colors = [new Vector3(1,.2f,0), Vector3.One];
        }
        public override void Draw()
        {
            int m = 0;
            int mp = 0;
            var e = game.basicModelEffect;
            var ee = game.basicModelEffect.effect;
            e.SetTech("basic_color");
            e.SetKA(0.3f);
            e.SetKD(0.9f);
            e.SetKS(0.8f);
            e.SetShininess(30f);

            foreach (var mesh in model.Meshes)
            {
                e.SetKD(0.9f);
                e.SetWorld(mesh.ParentBone.Transform * p.world);
                e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(mesh.ParentBone.Transform * p.world)));
                e.SetTiling(Vector2.One);
                
                mp = 0;
                foreach (var part in mesh.MeshParts)
                {
                    if (m == 0)
                    {
                        if (mp == 0)
                            e.SetColor(colors[0]);

                        else if (mp == 1)
                            e.SetColor(new Vector3(.2f));
                        else if (mp == 2)
                            e.SetColor(Color.Gray.ToVector3());
                        else if (mp == 3)
                        {
                            e.SetColor(p.car.brakeL.color);
                            e.SetKD(0f);
                        }
                        else if (mp == 4)
                        {
                            e.SetColor(Color.White.ToVector3());
                            e.SetKD(0f);
                        }
                        else if (mp == 5)
                        {
                            e.SetColor(Color.White.ToVector3());
                            e.SetKD(0.9f);
                        }

                    }
                    else
                    {
                        if (m == 1 || m == 2)
                        {

                            var wheelWorld = p.frontWheelWorld * mesh.ParentBone.Transform * p.world;

                            e.SetWorld(wheelWorld);
                            e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(wheelWorld)));
                        }
                        else
                        {
                            var wheelWorld = p.backWheelWorld * mesh.ParentBone.Transform * p.world;

                            e.SetWorld(wheelWorld);
                            e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(wheelWorld)));
                        }
                        if (mp == 0)
                            e.SetColor(Color.Black.ToVector3());
                        else
                            e.SetColor(colors[1]);
                    }

                    foreach (var pass in ee.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        game.GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        game.GraphicsDevice.Indices = part.IndexBuffer;
                        game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                    mp++;
                }
                m++;
            }
        }
    }
}
