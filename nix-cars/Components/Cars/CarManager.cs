using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Cars
{
    public class CarManager
    {
        static List<(string, Model)> carModels = new List<(string, Model)>();
        static string[] carModelNames;

        static NixCars game;

        public static LocalPlayer localPlayer;
        public static Player enemyCar;
        
        public static Model GetModel(string name)
        {
            foreach (var (n, m) in carModels)
            {
                if(name == n) return m;
            }
            return null;
        }
        public static void Init()
        {
            game = NixCars.GameInstance();
            carModelNames = new string[] { "sport" };

            for (int i = 0; i < carModelNames.Length; i++)
            {
                var name = carModelNames[i];
                carModels.Add((name, game.Content.Load<Model>(NixCars.ContentFolder3D + "cars\\" + name)));
            }
            foreach (var (_,m) in carModels)
                NixCars.AssignEffectToModel(m, game.basicModelEffect.effect);

            var pc = new CarSport();
            
            localPlayer = new LocalPlayer(pc);

            //enemyCar = new Player(GetModel("sport"), CarType.Sport);
            //enemyCar.position = new Vector3(200f, 10, -300);

        }
        public static void UpdatePlayerCar(bool f, bool b, bool l, bool r, float deltaTime)
        {
            localPlayer.Update(f, b, l, r, deltaTime);
        }
        public static void DrawPlayerCar()
        {
            localPlayer.car.Draw();
        }


    }


}
