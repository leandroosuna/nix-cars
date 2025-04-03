using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Cars
{
    public class CarManager
    {
        const int carCount = 1;
        static List<Model> carModels = new List<Model>();
        static NixCars game;

        public static Car playerCar;

        public static void Init()
        {
            game = NixCars.GameInstance();
            
            for(int i = 0; i< carCount; i++)
            {
                carModels.Add(game.Content.Load<Model>(NixCars.ContentFolder3D+"cars\\car"+i));
                
            }
            foreach(var m in carModels)
                NixCars.AssignEffectToModel(m, game.basicModelEffect.effect);


            playerCar = new Car(carModels[0], CarType.Sport);


        }
        public static void UpdatePlayerCar(bool f, bool b, bool l, bool r, float deltaTime)
        {
            playerCar.Update(f, b, l, r, deltaTime);
        }
        public static void DrawPlayerCar()
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

            foreach (var mesh in playerCar.model.Meshes)
            {
                e.SetKD(0.9f);
                //var w = mesh.ParentBone.Transform * Matrix.CreateScale(.5f) * Matrix.CreateTranslation(0, 1, 0);
                e.SetWorld(mesh.ParentBone.Transform * playerCar.world);
                e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(mesh.ParentBone.Transform * playerCar.world)));
                e.SetTiling(Vector2.One);
                //effect.Parameters["world"].SetValue(w);
                var r = new Random();

                var randomColor = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
                randomColor.Normalize();

                mp = 0;
                foreach (var part in mesh.MeshParts)
                {
                    if (m == 0)
                    {
                        if (mp == 0)
                            e.SetColor(Color.Red.ToVector3());

                        else if (mp == 1)
                            e.SetColor(Color.Gray.ToVector3());
                        else if (mp == 2)
                            e.SetColor(new Vector3(.1f, .1f, .1f));
                        else if (mp == 3)
                        {
                            e.SetColor(playerCar.brakeL.color);
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

                            var wheelWorld = playerCar.frontWheelWorld * mesh.ParentBone.Transform * playerCar.world;

                            e.SetWorld(wheelWorld);
                            e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(wheelWorld)));
                        }
                        else
                        {
                            var wheelWorld = playerCar.backWheelWorld * mesh.ParentBone.Transform * playerCar.world;

                            e.SetWorld(wheelWorld);
                            e.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(wheelWorld)));
                        }
                        if (mp == 0)
                            e.SetColor(Color.Black.ToVector3());
                        else
                            e.SetColor(Color.White.ToVector3());
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
