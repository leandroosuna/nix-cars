using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.States
{
    public class GameStateManager
    {
        static NixCars game;
        public static GSMainMenu mainMenu;
        public static GSRun run;
        public static GSCarSelect carSelect;

        public static bool paused = false;
        static GameState last;
        public static void Init()
        {
            game = NixCars.GameInstance();
            mainMenu = new GSMainMenu();
            run = new GSRun();
            carSelect = new GSCarSelect();

            SwitchTo(State.MAIN);
        }
        public static void SwitchTo(State state)
        {
            last = game.gameState;
            GameState newState = game.gameState;
            switch (state)
            {
                case State.MAIN:
                    newState = mainMenu;
                    break;
                case State.RUN:
                    newState = run;
                    break;
                case State.CARSELECT:
                    newState = carSelect;
                    break;


            }

            game.gameState = newState;
            newState.OnSwitch();

        }
        //public static void TogglePause()
        //{
        //    paused = !paused;
        //    //SwitchTo(paused ? State.PAUSE : State.RUN);

        //    if (paused)
        //    {
        //        game.IsMouseVisible = true;
        //        game.gameState.inputManager.mouseLocked = false;
        //    }
        //    else
        //    {
        //        game.gameState.inputManager.mouseLocked = true;
        //        System.Windows.Forms.Cursor.Position = game.gameState.inputManager.center;
        //        game.IsMouseVisible = false;
        //    }

        //}

    public static void SwitchToLast()
        {

            game.gameState = last;
            game.gameState.OnSwitch();
        }
    }
    public enum State
    {
        MAIN,
        OPTIONS,
        //INPUTMAP,
        CARSELECT,
        RUN,
        //PAUSE,
    }
}
