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
        public float uTotalTime;
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
            uTotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            mouseState = Mouse.GetState();
            if(!camLocked)
                UpdateMousePositionDelta();
            else
                mouseDelta = Vector2.Zero;

            keyState = Keyboard.GetState();
            keysDown.RemoveAll(key => !key.IsDown());
            Key.Update(mouseState, keyState);

        }

        public void FinishUpdate()
        {

            game.basicModelEffect.SetView(game.camera.view);
            game.basicModelEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetView(game.camera.view);
            game.deferredEffect.SetProjection(game.camera.projection);
            game.deferredEffect.SetCameraPosition(game.camera.position);

            
            game.lightsManager.Update(uDeltaTimeFloat);
        }
        double FPSShowLimiter;
        public virtual void Draw(GameTime gameTime)
        {
            dDeltaTimeDouble = gameTime.ElapsedGameTime.TotalSeconds;
            dDeltaTimeFloat = (float)uDeltaTimeDouble;

            FPSShowLimiter += dDeltaTimeDouble;

            if(FPSShowLimiter >= .05f)
            {
                FPSShowLimiter = 0;
                FPS = (int)(1 / dDeltaTimeDouble);
            }


        }
        
       
        public abstract void OnResolutionChange(int width, int height);

        public static void InitInput()
        {
            var fileCfg = "Files/input-settings.json";
            var jsonKeys = JsonKeys.LoadFromJson(fileCfg);
            km = new KeyMappings(jsonKeys);

            km.TAB = new KeyboardKey(Keys.Tab);
            km.CAPS = new KeyboardKey(Keys.CapsLock);
            km.Command = new KeyboardKey(Keys.RightShift);
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
