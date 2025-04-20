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

namespace nix_cars.Components.Network
{
    internal class NetworkManager
    {
        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeSetEvent(uint msDelay, uint msResolution, TimerCallback callback, IntPtr user, uint eventType);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeKillEvent(uint uTimerId);

        private delegate void TimerCallback(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2);
        private static TimerCallback callback;
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

        [MessageHandler((ushort)ServerToClient.AllPlayerData)]
        private static void HandleAllPlayerData(Message message)
        {
            playerCount = message.GetUInt();
            for(var i = 0; i < playerCount; i++)
            {
                var id = message.GetUInt();
                var connected = message.GetBool();

                if (id != localPlayer.id)
                {
                    var p = (EnemyPlayer) CarManager.GetPlayerFromId(id, true);
                    p.connected = connected;

                    if(connected)
                    {
                        var cache = new PlayerCache(ref message, game.mainStopwatch.ElapsedMilliseconds);
                        p.cacheMutex.WaitOne();
                        p.netDataCache.Add(cache);
                        p.cacheMutex.ReleaseMutex();
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
