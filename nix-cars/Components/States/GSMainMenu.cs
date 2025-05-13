using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using nix_cars.Components.Cars;
using nix_cars.Components.GUI;
using nix_cars.Components.Network;
using nix_cars.Screens;
using SharpDX.Direct3D9;

namespace nix_cars.Components.States
{
    public class GSMainMenu : GameState
    {
        StartMenu sm;
        string name;
        public GSMainMenu() : base()
        {
        
        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
            GumManager.SwitchTo(Screen.MAIN);
            sm = GumManager.GetStartMenu();

            name = game.CFG["PlayerName"].Value<string>();
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            game.camera.SmoothMoveTo(new Vector3(0,5,-5));
            game.camera.SmoothRotateTo(Vector3.Zero);
            game.camera.Update(uDeltaTimeFloat);

            if (km.KeyDownOnce(km.Escape))
            {
                game.Exit();
            }
            
            if (km.KeyDownOnce(km.Enter))
            {
                HandleEnterGame();
            }

            FinishUpdate();
        }
        public void HandleEnterGame()
        {
            if (sm.NameBox.Text == "")
            {
                sm.ToastError.TextInstance.Text = "Ingresa tu nombre para entrar";
                sm.ToastError.IsVisible = true;
                sm.timer.Start();
            }
            else
            {
                if(sm.NameBox.Text != name)
                {
                    CarManager.localPlayer.name = sm.NameBox.Text;
                    game.CFG["PlayerName"] = sm.NameBox.Text;
                    game.SaveCFG();
                    NetworkManager.SendPlayerIdentity();
                }
                GameStateManager.SwitchTo(State.CARSELECT);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Color.Black);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
;
        }
        public override void OnResolutionChange(int w, int h)
        {

        }
        public override void LostFocus()
        {
            
        }
        public override void Focused()
        {
            
        }

    }
}
