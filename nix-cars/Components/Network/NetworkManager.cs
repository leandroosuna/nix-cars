using Newtonsoft.Json.Linq;
using nix_cars.Components.Cars;
using nix_cars.Components.States;
using Riptide;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nix_cars.Components.Network
{
    internal class NetworkManager
    {
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeSetEvent(uint msDelay, uint msResolution, TimerCallback callback, IntPtr user, uint eventType);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeKillEvent(uint uTimerId);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);


        private delegate void TimerCallback(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2);
        private static TimerCallback callback;
        private static long perfFrequency;
        private static double ticksToMilliseconds;

        public static Client Client { get; set; }
        public static uint playerCount = 0;
        static NixCars game;
        private static uint timerId;
        static uint TargetMS;
        public static int ConnectionAttempts = 1;
        static string serverIP;
        public static uint tick;
        static LocalPlayer localPlayer;
        static string version;
        static bool versionReceived;
        static bool correctVersion;

        
        public static void Connect()
        {
            game = NixCars.GameInstance();
            
            QueryPerformanceFrequency(out perfFrequency);
            ticksToMilliseconds = 1000.0 / perfFrequency;

            localPlayer = CarManager.localPlayer;

            CarManager.localPlayer.id = game.CFG["ClientID"].Value<uint>();
            CarManager.localPlayer.name = game.CFG["PlayerName"].Value<string>();
            
            Client = new Client();

            if (game.CFG.ContainsKey("ServerIP"))
                serverIP = game.CFG["ServerIP"].Value<string>();
            else
            {
                var server = game.CFG["ServerURL"].Value<string>();
                serverIP = Dns.GetHostAddresses(server)[0].ToString();

            }


            serverIP += ":7777";

            Client.Connect(serverIP);

            Client.ConnectionFailed += Client_ConnectionFailed;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;
            callback = TimerElapsed;

            TargetMS = 1000 / game.CFG["TPS"].Value<uint>();
            
            timerId = timeSetEvent(TargetMS, 0, callback, IntPtr.Zero, 1);

        }
        public static long GetHighPrecisionTime()
        {
            QueryPerformanceCounter(out long counter);
            return (long)(counter * ticksToMilliseconds);
        }
        private static void TimerElapsed(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2)
        {
            Client.Update();
            if (Client.IsConnected)
            {
                SendData();
                tick++;
            }
            else
            {
                tick--;
            }
        }

        public static void SendData()
        {
            var msg = Message.Create(MessageSendMode.Unreliable, ClientToServer.PlayerData);
            msg.AddUInt(localPlayer.id);
            msg.AddVector3(localPlayer.position);
            msg.AddVector2(localPlayer.horizontalVelocity);
            msg.AddFloat(localPlayer.yaw);
            msg.AddFloat(localPlayer.pitch);

            Client.Send(msg);
        }

        static bool isReconnect = false;
        private static void Client_Connected(object sender, EventArgs e)
        {
            Debug.WriteLine("CONNECTED");
            //if (isReconnect)
            //{
            //    SendPlayerIdentity();
            //    isReconnect = false;
            //}
            SendPlayerIdentity();
        }

        private static void Client_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {

            Client.Connect(serverIP);
            ConnectionAttempts++;

        }

        private static void Client_Disconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("DISCONNECTED");

            //attempt auto reconnect
            Client.Connect(serverIP);
            isReconnect = true;
        }

        public static void StopNetThread()
        {
            timeKillEvent(timerId);
        }
        public static void SendPlayerIdentity()
        {
            Message msg = Message.Create(MessageSendMode.Reliable, ClientToServer.PlayerIdentity);
            msg.AddUInt(localPlayer.id);
            msg.AddString(localPlayer.name);
            msg.AddInt(game.CFG["Version"].Value<int>());
            Client.Send(msg);
        }
        [MessageHandler((ushort)ServerToClient.Version)]
        private static void HandleVersion(Message message)
        {
            correctVersion = game.CFG["Version"].Value<int>() == message.GetInt();
            versionReceived = true;
        }

        [MessageHandler((ushort)ServerToClient.PlayerName)]
        private static void HandlePlayerName(Message message)
        {
            uint count = message.GetUInt();
            for(uint i = 0; i < count; i++)
            {
                var id = message.GetUInt();
                string name = message.GetString();
                EnemyPlayer e = (EnemyPlayer)CarManager.GetPlayerFromId(id, true);
                e.SetName(name);

                //Debug.WriteLine($"{id} set name {name} ");
            }
        }

        [MessageHandler((ushort)ServerToClient.AllPlayerData)]
        private static void HandleAllPlayerData(Message message)
        {
            var now = GetHighPrecisionTime(); 
            
            playerCount = message.GetUInt();
            for(var i = 0; i < playerCount; i++)
            {
                var id = message.GetUInt();
                var connected = message.GetBool();

                if (id != localPlayer.id)
                {
                    var p = (EnemyPlayer) CarManager.GetPlayerFromId(id, true);
                    p.connected = connected;

                    if (connected)
                    {
                        var newNode = new PlayerCache(ref message, now);

                        lock (p.cacheMutex)
                        {
                            var node = p.netDataCache.Last;
                            while (node != null && node.Value.timeStamp > newNode.timeStamp)
                                node = node.Previous;
                            if (node == null) p.netDataCache.AddFirst(newNode);
                            else p.netDataCache.AddAfter(node, newNode);

                            // Keep cache from growing unbounded
                            while (p.netDataCache.Count > 50)
                                p.netDataCache.RemoveFirst();
                        }
                    }
                }
                else
                {
                    if(connected)
                    {
                        var cache = new PlayerCache(ref message, 0);
                        //TODO: server reconciliation
                    }
                }

            }
        }

        
    }
}
