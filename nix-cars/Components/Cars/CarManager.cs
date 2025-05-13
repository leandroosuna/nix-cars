using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.Effects;
using nix_cars.Components.Network;
using nix_cars.Components.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nix_cars.Components.Cars
{
    public class CarManager
    {
        static List<(ushort, string, Model)> carModels = new List<(ushort, string, Model)>();
        static List<CarColors> carColors = new List<CarColors>();
        static string[] carModelNames;

        static NixCars game;

        public static LocalPlayer localPlayer;
        public static List<EnemyPlayer> players = new List<EnemyPlayer>();
        public static List<Player> onlinePlayers = new List<Player>();
        const string ColorsFilePath = "Files/car-colors.xml";
        public static (ushort, Model) GetModel(string name)
        {
            foreach (var (id, n, m) in carModels)
            {
                if(name == n) return (id,m);
            }
            return (ushort.MaxValue, null);
        }
        public static void Init()
        {
            game = NixCars.GameInstance();
            carModelNames = ["sport", "roadster", "hatchback", "f1", "muscle"];

            for (ushort i = 0; i < carModelNames.Length; i++)
            {
                var name = carModelNames[i];
                carModels.Add((i, name, game.Content.Load<Model>(NixCars.ContentFolder3D + "cars/" + name)));
            }
            foreach (var (_,_,m) in carModels)
                NixCars.AssignEffectToModel(m, game.basicModelEffect.effect);


            LoadCarColors();

            //carColors = [
            //    new CarColors("sport", [new Vector3(1, 0.5f, 0), Vector3.One]),
            //    new CarColors("roadster", [new Vector3(0, 0.25f, 1), Vector3.One, new Vector3(0, 0f, .8f)]),
            //    new CarColors("hatchback", [new Vector3(0, 1, 1), Vector3.One, ]),
            //    new CarColors("muscle", [new Vector3(0, 1, 0), Vector3.One, Vector3.One])
            //];
            //SaveCarColors();


            
            var pc = new CarMuscle();
            localPlayer = new LocalPlayer(pc);
            
            lock(onlinePlayers)
            {
                onlinePlayers.Add(localPlayer);
            }
        }
        
        public static void UpdatePlayers()
        {
            lock (players)
            {
                foreach (var p in players)
                    p.Update();
                players.RemoveAll(p => p.readyForRemoval);

                lock (onlinePlayers)
                {
                    foreach (var p in players)
                    {
                        if (!onlinePlayers.Contains(p))
                            onlinePlayers.Add(p);
                    }
                    
                    onlinePlayers.RemoveAll(p => !players.Contains(p) && p != localPlayer);

                }
                
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
        public static void ChangePlayerCar(Player p, Car car)
        {
            p.car.DestroyLights();
            p.car = car;
            car.Init(p);

            if(p == localPlayer)
            {
                SaveColorsFrom(p.car);
                // TODO: send to server, for other players to update the model.
                NetworkManager.SendCarChange();
            }

        }
        public static void CreatePlayerCar(Player p, ushort id, Vector3[] colors)
        {
            Car car = new CarSport();
            switch (id)
            {
                case 0: break;
                case 1: car = new CarRoadster(); break;
                case 2: car = new CarHatchback(); break;
                case 3: car = new CarF1(); break;
                case 4: car = new CarMuscle(); break;
            }
            car.LoadModel();

            car.colors = colors;
            ChangePlayerCar(p, car);
        }

        public static void LoadColorsTo(List<Player> players)
        {
            foreach(var p in players)
            {
                var name = "sport";

                if (p.car is CarSport)
                    name = "sport";
                if (p.car is CarHatchback)
                    name = "hatchback";
                if (p.car is CarMuscle)
                    name = "muscle";
                if (p.car is CarRoadster)
                    name = "roadster";

                var c = carColors.Find(cc => cc.name == name);
                p.car.colors = c.colors;
            }
        }

        public static void SaveColorsFrom(Car car)
        {
            var name = "sport";

            if (car is CarSport)
                name = "sport";
            if (car is CarHatchback)
                name = "hatchback";
            if (car is CarMuscle)
                name = "muscle";
            if (car is CarRoadster)
                name = "roadster";

            var c = carColors.Find(cc => cc.name == name);
            c.colors = car.colors;

            SaveCarColors();
        }
        public static void SaveCarColors()
        {
            var serializer = new XmlSerializer(typeof(List<CarColors>));
            using (var writer = new StreamWriter(ColorsFilePath))
            {
                serializer.Serialize(writer, carColors);
            }
        }
        public static void LoadCarColors()
        {
            var serializer = new XmlSerializer(typeof(List<CarColors>));
            using (var reader = new StreamReader(ColorsFilePath))
            {
                carColors = (List<CarColors>)serializer.Deserialize(reader);
            }
        }
    }
    [Serializable]
    public class CarColors
    {
        public string name;
        public Vector3[] colors;

        public CarColors() { }
        public CarColors(string name, Vector3[] colors)
        {
            this.name = name;
            this.colors = colors;
        }
        public CarColors(CarColorsSer ccs) 
        {
            name = ccs.name;
            colors = ccs.colors.Select(c => new Vector3(c.X, c.Y, c.Z)).ToArray();
        }
    }

    [Serializable]
    public class CarColorsSer
    {
        public string name;

        public SerializableVector3[] colors;

        public CarColorsSer() { }
        public CarColorsSer(CarColors cs)
        {
            name = cs.name;
            colors = cs.colors.Select(s => new SerializableVector3(s)).ToArray();
        }
    }

    

}
