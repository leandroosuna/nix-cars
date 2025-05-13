using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using nix_cars.Components.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Sound
{
    public static class SoundManager
    {
        static NixCars game;
        static SoundEffect soundEngine1;
        static SoundEffect soundEngine2;

        static AudioListener listener;

        public static SoundEffectInstance soundEngine1Instance;
        public static SoundEffectInstance soundEngine2Instance;
        public static float EngineVolume = 1;
        public static float MasterVolume = .2f;

        public static void LoadContent()
        {
            game = NixCars.GameInstance();
            soundEngine1 = game.Content.Load<SoundEffect>(NixCars.ContentFolderSounds + "engine1");
            soundEngine2 = game.Content.Load<SoundEffect>(NixCars.ContentFolderSounds + "engine2");

            listener = new AudioListener();
            listener.Up = Vector3.UnitY;

            soundEngine1Instance = soundEngine1.CreateInstance();
            soundEngine1Instance.IsLooped = true;
            soundEngine1Instance.Pitch = 1f;
            soundEngine1Instance.Volume = 0; 
            //soundEngine1Instance.Play();

            soundEngine2Instance = soundEngine2.CreateInstance();
            soundEngine2Instance.IsLooped = true;
            soundEngine2Instance.Pitch = 0f;
            soundEngine2Instance.Volume = 1;
            //soundEngine2Instance.Play();


        }
        static float fakeSpeed = 0;
        public static void EngineSound(LocalPlayer lp, float dt)
        {
            if (soundEngine1Instance.State == SoundState.Stopped)
                soundEngine1Instance.Play();
            if (soundEngine2Instance.State == SoundState.Stopped)
                soundEngine2Instance.Play();
            
            float speedNorm = Math.Abs(lp.speed)/ lp.maxSpeedBoost;
            speedNorm = MathHelper.Clamp(speedNorm, 0f, 1f);

            if (lp.positionLocked)
            {
                if (lp.inF)
                {
                    fakeSpeed += dt;
                    if (fakeSpeed > .66f)
                        fakeSpeed = .66f;

                }
                else
                {
                    fakeSpeed -= dt;
                    if (fakeSpeed < 0)
                        fakeSpeed = 0;
                }

                speedNorm = fakeSpeed;

            }
            else
            {
                fakeSpeed = 0;
            }

            const float threshold = 2f / 3f;

            if (speedNorm <= threshold)
            {
                float t = speedNorm / threshold;
                soundEngine1Instance.Pitch = MathHelper.Lerp(-1f, 1f, t);
                soundEngine1Instance.Volume = 1f * EngineVolume * MasterVolume;
                soundEngine2Instance.Pitch = 0f;
                soundEngine2Instance.Volume = 0f;
            }
            else
            {
                float t = (speedNorm - threshold) / (1f - threshold);
                t = Math.Clamp(t, 0, 1);

                var p = MathHelper.Lerp(0f, 1f, t);
                soundEngine1Instance.Pitch = 0;
                soundEngine1Instance.Volume = 0 ;                        
                soundEngine2Instance.Pitch = p;    
                soundEngine2Instance.Volume = (1f - p * 0.6f) * EngineVolume * MasterVolume;                             
            }
        }
    }
}
