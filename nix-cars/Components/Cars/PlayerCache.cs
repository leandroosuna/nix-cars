using Microsoft.Xna.Framework;
using nix_cars.Components.Network;
using Riptide;

namespace nix_cars.Components.Cars
{
    public class PlayerCache
    {
        public Vector3 position;
        public float yaw, pitch;
        public long timeStamp;
        
        public PlayerCache(ref Message message, long now)
        {
            timeStamp = now;
            position = message.GetVector3();
            yaw = message.GetFloat();
            pitch = message.GetFloat();
        }
    }
}
