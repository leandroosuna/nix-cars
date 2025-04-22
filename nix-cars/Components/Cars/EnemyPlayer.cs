using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.FlotatingTextures;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using nix_cars.Components.Network;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;


namespace nix_cars.Components.Cars
{
    public class EnemyPlayer : Player
    {
        public Mutex cacheMutex;
        public bool E = false;
        public bool readyForRemoval = false;
        public EnemyPlayer(uint id) : base()
        {
            this.id = id;
            cacheMutex = new Mutex(false, "cache-"+id);
            car = new CarSport();
            car.Init(this);

        }
        public void SetName(string name)
        {
            this.name = name;

            if (nameBanner != null)
                FlotatingTextureDrawer.RemoveText(nameBanner);

            nameBanner = new FlotatingText();
            nameBanner.SetText(name);

            FlotatingTextureDrawer.AddText(nameBanner);
        }
        public void ChangeCar(Car car)
        {
            this.car = car;
            car.Init(this);
        }
        void SelfDestroy()
        {
            if(nameBanner != null)
                FlotatingTextureDrawer.RemoveText(nameBanner);
            
            car.DestroyLights();
            
            readyForRemoval = true;
        }
        public void Update()
        {
            E = false;
            if (!connected)
            {
                SelfDestroy();
                return;
            }
            if (netDataCache.Count < 2)
                return;

                var renderBehind = 20; // ms TODO: fix TPS dependent
            var interpolationFactor = 0.05f; // wont be perfect, but good enough
            var now = NetworkManager.GetHighPrecisionTime() - renderBehind;

            PlayerCache newer;
            lock (cacheMutex)
            {

                // drop any frames that are entirely before our render window
                while (netDataCache.Count > 3 && netDataCache.First.Next.Value.timeStamp <= now)
                    netDataCache.RemoveFirst();

                newer = netDataCache.First.Next.Value;

            }

            position = Vector3.Lerp(position, newer.position, interpolationFactor);

            var q0 = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
            var q1 = Quaternion.CreateFromYawPitchRoll(newer.yaw, newer.pitch, 0);
            var q = Quaternion.Slerp(q0, q1, interpolationFactor);

            (yaw, pitch) = (
                (float)Math.Atan2(2 * (q.W * q.Y + q.X * q.Z), 1 - 2 * (q.Y * q.Y + q.Z * q.Z)),
                (float)Math.Asin(2 * (q.W * q.Z - q.Y * q.X))
            );

            currentTurnRate = 0;
            CalculateWorld();
            car.HandleLights(false);
            car.CalculateLightsPosition();
            car.UpdateCollider();

            var camYaw = NixCars.GameInstance().camera.yaw;

            camYaw = MathHelper.ToRadians(camYaw);
            var mx = Matrix.CreateFromYawPitchRoll(camYaw + MathF.PI, pitch + MathHelper.PiOver2, 0f)
                * Matrix.CreateTranslation(position + Vector3.Up * 3f);

            nameBanner.SetRT(mx);
        }
    }
}
