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
        public static List<EnemyPlayer> players = new List<EnemyPlayer>();
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

            // TODO: get type and colors from file.
            var pc = new CarSport();
            
            localPlayer = new LocalPlayer(pc);
            
            //enemyCar = new Player(GetModel("sport"), CarType.Sport);
            //enemyCar.position = new Vector3(200f, 10, -300);

        }
       
        public static void UpdatePlayers()
        {
            lock (players)
            {
                foreach (var p in players)
                    p.Update();
                players.RemoveAll(p => p.readyForRemoval);
            }
        }
        public static void DrawPlayers()
        {
            localPlayer.car.Draw();

            foreach(var p in players)
            {
                if(p.connected)
                    p.car.Draw();
            }
        }

        public static Player GetPlayerFromId(uint id, bool createIfNull = false)
        {
            if (id == localPlayer.id) return localPlayer;

            foreach (var player in players)
            {
                if (player.id == id)
                {
                    return player;
                }
            }
            if (createIfNull)
            {
                var p = new EnemyPlayer(id);
                p.SetName("noname");
                lock(players)
                {
                    players.Add(p);
                }
                return p;
            }

            return new EnemyPlayer(uint.MaxValue);
        }
    }


}
