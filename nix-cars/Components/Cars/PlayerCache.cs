using Microsoft.Xna.Framework;
using nix_cars.Components.Network;
using Riptide;

namespace nix_cars.Components.Cars
{
    public class PlayerCache
    {
        public long timeStamp; 
        public Vector3 position;
        public float yaw, pitch;
        public Vector2 horizontalVelocity;
        public bool f, b, l, r, boost;
        public float progress;
        public ushort lap;
        public PlayerCache(ref Message message, long now)
        {
            timeStamp = now;
            position = message.GetVector3();
            yaw = message.GetFloat();
            pitch = message.GetFloat();
            horizontalVelocity = message.GetVector2();
            f = message.GetBool();
            b = message.GetBool();
            l = message.GetBool();
            r = message.GetBool();
            boost = message.GetBool();

            progress = message.GetFloat();
            lap = message.GetUShort();
        }
    }
}
