using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Effects;

namespace nix_cars.Components.Lights
{
    public abstract class LightVolume
    {
        public static Model sphere;
        public static Model lightSphere;
        public static Model lightCone;
        public static Model lightCylinder;
        public static Model cube;
        public static DeferredEffect deferredEffect;
        public static BasicModelEffect basicModelEffect;

        //public static Model cone;

        public Vector3 position;
        public BoundingSphere collider;
        public Vector3 color;
        public Vector3 ambientColor;
        public Vector3 specularColor;

        public bool enabled;
        public bool hasLightGeo;
        public bool skipDraw;
        static NixCars game;
        public LightVolume(Vector3 position, Vector3 color, Vector3 ambientColor, Vector3 specularColor)
        {
            this.position = position;
            this.color = color;
            this.ambientColor = ambientColor;
            this.specularColor = specularColor;
            enabled = true;
            hasLightGeo = false;
        }
        public static void Init()
        {
            game = NixCars.GameInstance();
            
            sphere = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/sphere");
            cube = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/cube");
            lightSphere = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/lightSphere");
            lightCone = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/lightCone");
            lightCylinder = game.Content.Load<Model>(NixCars.ContentFolder3D + "basic/lightCylinder");
            deferredEffect = game.deferredEffect;
            basicModelEffect = game.basicModelEffect;

            NixCars.AssignEffectToModel(sphere, basicModelEffect.effect);
            NixCars.AssignEffectToModel(cube, basicModelEffect.effect);

            NixCars.AssignEffectToModel(lightSphere, deferredEffect.effect);
            NixCars.AssignEffectToModel(lightCone, deferredEffect.effect);
            NixCars.AssignEffectToModel(lightCylinder, deferredEffect.effect);


        }

        public abstract void Update();
        public abstract void Draw();

        public void DrawLightGeo()
        {
            if(hasLightGeo)
            {
                basicModelEffect.SetTech("color_lightDis");
                basicModelEffect.SetColor(color);
            
                foreach (var mesh in sphere.Meshes)
                {
                    var w = mesh.ParentBone.Transform * Matrix.CreateScale(0.0025f) * Matrix.CreateTranslation(position);
                    //var w = mesh.ParentBone.Transform * Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(position);
                    basicModelEffect.SetWorld(w);
                    basicModelEffect.SetInverseTransposeWorld(Matrix.Invert(Matrix.Transpose(w)));
                    mesh.Draw();
                }
            }
        }
        

    }
}
