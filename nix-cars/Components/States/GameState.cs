using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nix_cars.Components.States.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.States
{
    public abstract class GameState
    {
        public NixCars game;
        public double uDeltaTimeDouble;
        public float uDeltaTimeFloat;
        public double dDeltaTimeDouble;
        public float dDeltaTimeFloat;
        public int FPS;

        public static List<Key> keysDown = new List<Key>();
        public static KeyboardState keyState;
        public static MouseState mouseState;
        public static KeyMappings km;
        public static Key testKey;

        Point mousePosition;
        Vector2 delta;
        public bool mouseLocked;
        public bool camLocked = false;
        public Vector2 mouseDelta;
        float mouseSensitivity = 0.15f;
        float mouseSensAdapt = .09f;
        System.Drawing.Point center;

        public GameState()
        {
            game = NixCars.GameInstance();
            if (km == null)
            {
                InitInput();
            }

            var screenCenter = game.screenCenter;
            center = new System.Drawing.Point(screenCenter.X, screenCenter.Y);
        }
        public abstract void OnSwitch();
        public abstract void LostFocus();
        public abstract void Focused();

        public virtual void Update(GameTime gameTime)
        {
            uDeltaTimeDouble = gameTime.ElapsedGameTime.TotalSeconds;
            uDeltaTimeFloat = (float)uDeltaTimeDouble;

            game.basicModelEffect.SetView(game.camera.view);
            game.basicModelEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetView(game.camera.view);
            game.deferredEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetCameraPosition(game.camera.position);
            
            mouseState = Mouse.GetState();
            if(!camLocked)
                UpdateMousePositionDelta();
            else
                mouseDelta = Vector2.Zero;

            keyState = Keyboard.GetState();
            keysDown.RemoveAll(key => !key.IsDown());
            Key.Update(mouseState, keyState);

            game.lightsManager.Update(uDeltaTimeFloat);

            

        }
        public virtual void Draw(GameTime gameTime)
        {
            dDeltaTimeDouble = gameTime.ElapsedGameTime.TotalSeconds;
            dDeltaTimeFloat = (float)uDeltaTimeDouble;
            FPS = (int)(1 / dDeltaTimeDouble);


        }


        public static void InitInput()
        {
            var fileCfg = "input-settings.json";
            var jsonKeys = JsonKeys.LoadFromJson(fileCfg);
            km = new KeyMappings(jsonKeys);
            //keyMappings.Debug0 = new KeyboardKey(Keys.D0);
            //keyMappings.Debug1 = new KeyboardKey(Keys.D1);
            //keyMappings.Debug2 = new KeyboardKey(Keys.D2);
            //keyMappings.Debug3 = new KeyboardKey(Keys.D3);
            //keyMappings.Debug7 = new KeyboardKey(Keys.D7);
            //keyMappings.Debug8 = new KeyboardKey(Keys.D8);
            //keyMappings.Debug9 = new KeyboardKey(Keys.D9);
            km.TAB = new KeyboardKey(Keys.Tab);
            km.CAPS = new KeyboardKey(Keys.CapsLock);
        }

        
        public void UpdateMousePositionDelta()
        {
            mousePosition.X = System.Windows.Forms.Cursor.Position.X;
            mousePosition.Y = System.Windows.Forms.Cursor.Position.Y;

            delta.X = mousePosition.X - center.X;
            delta.Y = mousePosition.Y - center.Y;

            if (mouseLocked)
            {
                mouseDelta = delta * mouseSensitivity * mouseSensAdapt;
                System.Windows.Forms.Cursor.Position = center;
            }
            else
            {
                mouseDelta = Vector2.Zero;
            }
        }

    }
}
