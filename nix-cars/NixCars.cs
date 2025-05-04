using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using nix_cars.Components.FloatingPlanes;
using nix_cars.Components.Cameras;
using nix_cars.Components.Cars;
using nix_cars.Components.Collisions;
using nix_cars.Components.Effects;
using nix_cars.Components.Gizmos;
using nix_cars.Components.Lights;
using nix_cars.Components.Network;
using nix_cars.Components.States;
using System;
using System.Diagnostics;
using System.IO;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Gum.Wireframe;
using Gum.DataTypes;
using nix_cars.Components.GUI;

namespace nix_cars
{
    public class NixCars : Game
    {
        public const string ContentFolder3D = "3D/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderFonts = "Fonts/";

        public GraphicsDeviceManager Graphics;
        public Camera camera;
        public static NixCars game;
        public Point screenCenter;
        public int screenWidth;
        public int screenHeight;

        public GameState gameState;

        public BasicModelEffect basicModelEffect;
        public DeferredEffect deferredEffect;
        public FullScreenQuad fullScreenQuad;
        public SpriteBatch spriteBatch;
        public SpriteFont font25;
        public SpriteFont font15;

        public LightsManager lightsManager;
        
        public RenderTarget2D colorTarget;
        public RenderTarget2D normalTarget;
        public RenderTarget2D positionTarget;
        public RenderTarget2D bloomFilterTarget;
        public RenderTarget2D blurHTarget;
        public RenderTarget2D blurVTarget;
        public RenderTarget2D lightTarget;
        public string lightQuality;

        public Gizmos gizmos;
        public JObject CFG;
        public Stopwatch mainStopwatch = new Stopwatch();

        public static int displayWidth;
        public static int displayHeight;
        public static int displayHz;
        public const string appSettingsPath = "app-settings.json";
        public NixCars()
        {
            displayHz = DisplayHelper.GetCurrentRefreshRate();
            
            CFG = JObject.Parse(File.ReadAllText(appSettingsPath));

            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            game = this;

            lightQuality = CFG["GraphicsPreset"].Value<string>();
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;


            var cdm = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            displayWidth = cdm.Width;
            displayHeight = cdm.Height;

            SetRes(CFG["ScreenWidth"].Value<int>(), CFG["ScreenHeight"].Value<int>());
            IsMouseVisible = false;


            //SetFPSLimit(CFG["FPSLimit"].Value<int>());

            //Graphics.SynchronizeWithVerticalRetrace = CFG["VSync"].Value<bool>();
            SetFPSLimit(CFG["FPSLimit"].Value<int>());
            Graphics.SynchronizeWithVerticalRetrace = false;
            game.Graphics.HardwareModeSwitch = false;
            Graphics.ApplyChanges();

            if (!CFG.ContainsKey("ClientID"))
            {
                var ri = new Random().NextInt64();
                CFG["ClientID"] = (uint)ri;

                SaveCFG();
            }
            Exiting += (s, e) => {

                //NetworkManager.Client.Disconnect();
                NetworkManager.StopNetThread();
                mainStopwatch.Stop();

            };
        }
        
        protected override void Initialize()
        {
            GumManager.Init(this);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fullScreenQuad = new FullScreenQuad(GraphicsDevice);
            basicModelEffect = new BasicModelEffect("basic");
            deferredEffect = new DeferredEffect("deferred");

            camera = new Camera(GraphicsDevice.Viewport.AspectRatio);
            font25 = Content.Load<SpriteFont>(ContentFolderFonts + "unispace/25");
            font15 = Content.Load<SpriteFont>(ContentFolderFonts + "unispace/15");
            SetupRenderTargets();
            lightsManager = new LightsManager();

            CarManager.Init();
            GameStateManager.Init();
            LightVolume.Init();
            gizmos = new Gizmos();
            gizmos.LoadContent(GraphicsDevice);

            mainStopwatch.Start();
            //NetworkManager.Connect();
            FloatingPlaneDrawer.Init();

        }
        
        protected override void Update(GameTime gameTime)
        {
            CheckGameRegainedFocus();

            gameState.Update(gameTime);
            
            gizmos.UpdateViewProjection(game.camera.view, game.camera.projection);


            GumManager.Update(gameTime);
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            gameState.Draw(gameTime);
            GumManager.Draw();
            base.Draw(gameTime);
        }
      
        public static void AssignEffectToModel(Model m, Effect e)
        {
            foreach (var mesh in m.Meshes)
                foreach (var part in mesh.MeshParts)
                    part.Effect = e;
        }

        public static NixCars GameInstance()
        {
            return game;
        }
        bool wasFocused;
        void CheckGameRegainedFocus()
        {
            if (!this.IsActive)
            {
                wasFocused = false;

                gameState.LostFocus();
            }
            else if (!wasFocused)
            {
                wasFocused = true;

                gameState.Focused();
            }
        }
        
        public void SetupRenderTargets()
        {
            var surfaceFormat = SurfaceFormat.HalfVector4;
            var lightResMultiplier = 1f;

            switch (lightQuality)
            {
                case "ultra": surfaceFormat = SurfaceFormat.Vector4; lightResMultiplier = 1f; break;
                case "high": surfaceFormat = SurfaceFormat.Vector4; lightResMultiplier = .7f; break;
                case "medium": surfaceFormat = SurfaceFormat.HalfVector4; lightResMultiplier = 0.5f; break;
                case "low": surfaceFormat = SurfaceFormat.HalfVector4; lightResMultiplier = 0.25f; break;
            }

            colorTarget = new RenderTarget2D(GraphicsDevice,
                screenWidth, screenHeight, false, surfaceFormat, DepthFormat.Depth24Stencil8);
            normalTarget = new RenderTarget2D(GraphicsDevice,
                screenWidth, screenHeight, false, surfaceFormat, DepthFormat.Depth24Stencil8);
            positionTarget = new RenderTarget2D(GraphicsDevice,
                screenWidth, screenHeight, false, surfaceFormat, DepthFormat.Depth24Stencil8);
            lightTarget = new RenderTarget2D(GraphicsDevice,
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, 
                surfaceFormat, DepthFormat.Depth24Stencil8);
            bloomFilterTarget = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, 
                surfaceFormat, DepthFormat.Depth24Stencil8);
            blurHTarget = new RenderTarget2D(GraphicsDevice,
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, surfaceFormat, 
                DepthFormat.Depth24Stencil8);
            blurVTarget = new RenderTarget2D(GraphicsDevice,
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, surfaceFormat, 
                DepthFormat.Depth24Stencil8);
        }
        public void SetFPSLimit(int l)
        {
            if(l == -1)
            {
                IsFixedTimeStep = true;
                Graphics.SynchronizeWithVerticalRetrace = true;
                TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / displayHz);
                Graphics.ApplyChanges();
                SaveCFG();
                return;
            }
            else if (l >= 30)
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / l);
                CFG["FPSLimit"] = l;
            }
            else
            {
                IsFixedTimeStep = false;

                CFG["FPSLimit"] = 0;
                
            }
            SaveCFG();

            if (Graphics.SynchronizeWithVerticalRetrace)
            {
                Graphics.SynchronizeWithVerticalRetrace = false;
                Graphics.ApplyChanges();
            }

        }
        public void ChangeResolution(int width, int height)
        {
            gameState.OnResolutionChange(width, height);
            SetRes(width, height);
            SetupRenderTargets();
            
            
        }
        public void SetFullScreen(bool val)
        {
            game.Graphics.IsFullScreen = val;
            game.Window.IsBorderless = true;
            game.Graphics.ApplyChanges();

            GumManager.ReCenterUI(Graphics.IsFullScreen);
        }

        void SetRes(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();
            Window.Position = new Point((displayWidth - screenWidth) / 2, (displayHeight - screenHeight) / 2);
            screenCenter = new Point(screenWidth / 2 + Window.Position.X, screenHeight / 2 + Window.Position.Y);
            
            GumManager.ReCenterUI(Graphics.IsFullScreen);
        }
        
        public void SaveCFG()
        {
            File.WriteAllText(appSettingsPath, CFG.ToString());
        }
        public static double Map(double value, double low1, double high1, double low2, double high2)
        {
            return low2 + ((value - low1) / (high1 - low1)) * (high2 - low2);
        }
    }
}
