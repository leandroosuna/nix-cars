using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars;
using nix_cars.Components.Effects;

namespace nix_cars.Components.Lights
{
    public class LightsManager
    {
        NixCars game;
        DeferredEffect effect;

        public List<LightVolume> lights = new List<LightVolume>();

        public List<LightVolume> lightsToDraw = new List<LightVolume>();

        public AmbientLight ambientLight;
        public LightsManager()
        {
            game = NixCars.GameInstance();
            effect = game.deferredEffect;
            effect.SetScreenSize(new Vector2(game.screenWidth, game.screenHeight));

            var ambientColor = new Vector3(1f, 1f, 1f);
            ambientLight = new AmbientLight(new Vector3(-50, 50, 50), ambientColor, ambientColor, ambientColor);
            effect.SetAmbientLight(ambientLight);
        }
        //float ang = 0;
        public void Update(float deltaTime)
        {
            lightsToDraw.Clear();
            
            lock(lights)
            {
                foreach (var l in lights)
                {
                    l.Update();

                    var inView = game.camera.FrustumContains(l.collider);


                    if (l.enabled && inView)
                        lightsToDraw.Add(l);
                }
            }
        }
        public void DrawAmbient()
        {
            effect.SetKA(.15f);
            effect.SetKD(.25f);
            effect.SetKS(.2f);
            effect.SetShininess(5f);
            effect.SetAmbientLight(ambientLight);

            effect.SetTech("ambient_light");

            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            game.fullScreenQuad.Draw(effect.effect);
        }
        public void Draw()
        {
            DrawAmbient();
            DrawLightVolumes();
        }
        public void DrawLightVolumes()
        {
            game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise; //remove front side of spheres to be drawn

            lightsToDraw.ForEach(l =>
            {
                if (!l.skipDraw)
                {
                    if (l is PointLight)
                        effect.SetTech("point_light");
                    if (l is ConeLight)
                        effect.SetTech("cone_light");
                    if (l is CylinderLight)
                        effect.SetTech("cylinder_light");

                    l.Draw();
                }
            });
        }
        public void DrawLightGeo()
        {
            lightsToDraw.ForEach(l => l.DrawLightGeo());            
        }
        public void Register(LightVolume volume)
        {
            lock (lights)
            {
                lights.Add(volume);
            }
        }
        public void Register(List<LightVolume> volumes)
        {
            lock (lights)
            {
                lights.AddRange(volumes);
            }
        }
        public void Register(List<PointLight> points)
        {
            lock (lights)
            {
                lights.AddRange(points);
            }
        }
        public void Destroy(LightVolume volume)
        {
            lock(lights)
            {
                lights.Remove(volume);
            }
        }
        public void Destroy(List<LightVolume> volumes)
        {
            lock (lights)
            {
                foreach (var volume in volumes)
                {
                    lights.Remove(volume);
                }
            }
        }
        public void Destroy(List<PointLight> points)
        {
            lock (lights)
            {
                foreach (var p in points)
                {
                    lights.Remove(p);
                }
            }
        }
    }
}
