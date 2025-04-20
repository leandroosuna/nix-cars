using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.Collisions;
using nix_cars.Components.Lights;
using SharpDX.Direct2D1.Effects;
using System;
using System.Linq;
using System.Threading;


namespace nix_cars.Components.Cars
{
    public class EnemyPlayer : Player
    {
        public Mutex cacheMutex;
        public EnemyPlayer(uint id) : base()
        {
            this.id = id;
            cacheMutex = new Mutex(false, "cache-"+id);
            car = new CarSport();
            car.Init(this);

        }

        public void ChangeCar(Car car)
        {
            this.car = car;
            car.Init(this);
        }
        
        public void Update(long now)
        {
            if (!connected)
            {
                return;
            }
            cacheMutex.WaitOne();
            if (netDataCache.Count < 2)
            {
                cacheMutex.ReleaseMutex();
                return;
            }
            netDataCache = netDataCache.OrderBy(pc => pc.timeStamp).ToList();

            var framesBehind = 2;
            long renderTimeStamp = now - 10 * framesBehind;

            if (netDataCache.All(pc => pc.timeStamp <= renderTimeStamp))
            {
                cacheMutex.ReleaseMutex();
                return;
            }
            int indexFound = 0;
            for (int i = 0; i < netDataCache.Count - 1; i++)
            {
                var t0 = netDataCache[i].timeStamp;
                var t1 = netDataCache[i + 1].timeStamp;

                if (t0 <= renderTimeStamp && renderTimeStamp < t1)
                {
                    indexFound = i;
                    var x0 = netDataCache[i].position;
                    var x1 = netDataCache[i + 1].position;

                    var y0 = netDataCache[i].yaw;
                    var y1 = netDataCache[i + 1].yaw;

                    var p0 = netDataCache[i].pitch;
                    var p1 = netDataCache[i + 1].pitch;

                    position = x0 + (x1 - x0) * (renderTimeStamp - t0) / (t1 - t0);

                    yaw = y0 + (y1 - y0) * (renderTimeStamp - t0) / (t1 - t0);
                    pitch = p0 + (p1 - p0) * (renderTimeStamp - t0) / (t1 - t0);
                }
            }
            if (netDataCache.Count > 6)
            { 
                if(indexFound>=4)
                    netDataCache.RemoveRange(0, indexFound-4);
            }
            cacheMutex.ReleaseMutex();

            currentTurnRate = 0;
            //currentTurnRate = 2.5f; //test R
            CalculateWorld();
            car.HandleLights(false);
            car.CalculateLightsPosition();
            car.UpdateCollider();
        }
        
    }
}
