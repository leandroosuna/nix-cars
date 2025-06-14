﻿using Gum.DataTypes;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;
using nix_cars.Screens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace nix_cars.Components.GUI
{
    static class GumManager
    {
        static GumService Gum => GumService.Default;
        static GumProjectSave GumProject;
        static GraphicalUiElement GumRoot;

        static GraphicalUiElement gueStartMenu;
        static GraphicalUiElement gueCarSelect;
        static GraphicalUiElement gueOptions;
        static GraphicalUiElement gueRaceHud;

        static NixCars game;
        static Screen currentScreen;

        static StartMenu startMenu;
        static RaceHud raceHud;

        public static void Init(NixCars g)
        {
            game = g;
            
            GumProject = Gum.Initialize(g, "GUM/gum.gumx");
            
            gueStartMenu = GumProject.Screens.Find(item => item.Name == "StartMenu").ToGraphicalUiElement();
            gueCarSelect = GumProject.Screens.Find(item => item.Name == "CarSelect").ToGraphicalUiElement();
            gueOptions = GumProject.Screens.Find(item => item.Name == "Options").ToGraphicalUiElement();
            gueRaceHud = GumProject.Screens.Find(item => item.Name == "RaceHud").ToGraphicalUiElement();

            SwitchTo(Screen.MAIN, false);

        }
        public static void Update(GameTime gt)
        {
            Gum.Update(game, gt, GumRoot);
        }
        public static void Draw()
        {
            Gum.Draw();
        }
        public static bool CurrentScreenIs(Screen s)
        {
            return currentScreen == s;
        }
        //public static void SwitchTo(Screen s, bool clear = true)
        //{
        //    if(clear)
        //        GumRoot.RemoveFromRoot();

        //    string name = "StartMenu";
        //    switch (s)
        //    {
        //        case Screen.MAIN: name = "StartMenu";break;
        //        case Screen.CARSELECT: name = "CarSelect"; break;
        //        case Screen.OPTIONS: name = "Options"; break;
        //        case Screen.RACEHUD: name = "RaceHud"; break;
        //    }
        //    currentScreen = s;
        //    GumRoot = GumProject.Screens.Find(item => item.Name == name).ToGraphicalUiElement();
        //    GumRoot.AddToRoot();
        //}
        public static void SwitchTo(Screen s, bool clear = true)
        {
            if (clear)
                GumRoot.RemoveFromRoot();

            currentScreen = s;
            switch (s)
            {
                case Screen.MAIN: GumRoot = gueStartMenu; break;
                case Screen.CARSELECT: GumRoot = gueCarSelect; break;
                case Screen.OPTIONS: GumRoot = gueOptions; break;
                case Screen.RACEHUD: GumRoot = gueRaceHud; break;
            }
            GumRoot.AddToRoot();
        }

        public static void Clear()
        {
            GumRoot.RemoveFromRoot();
        }
        public static void ReCenterUI(bool isFullScreen)
        {
            if (GumRoot != null)
            {
                if (isFullScreen)
                {
                    GraphicalUiElement.CanvasWidth = NixCars.displayWidth;
                    GraphicalUiElement.CanvasHeight = NixCars.displayHeight;
                }
                else
                {
                    GraphicalUiElement.CanvasWidth = game.screenWidth;
                    GraphicalUiElement.CanvasHeight = game.screenHeight;
                }
                GumRoot.UpdateLayout();
                GumRoot.RemoveFromRoot();
                GumRoot.AddToRoot();
            }
        }
        public static RaceHud GetRaceHud()
        {
           return RaceHud.GetInstance();
        }
        public static StartMenu GetStartMenu()
        {
            return StartMenu.GetInstance();
        }
        public static CarSelect GetCarSelect()
        {
            return CarSelect.GetInstance();
        }
    }
    public enum Screen
    {
        MAIN,
        CARSELECT,
        OPTIONS,
        RACEHUD
    }
}
