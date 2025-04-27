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

namespace nix_cars
{
    public class NixCars : Game
    {
        public const string ContentFolder3D = "3D/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderFonts = "Fonts/";

        private GraphicsDeviceManager Graphics;
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
        public string graphicsPreset;

        public Gizmos gizmos;
        public JObject CFG;
        public Stopwatch mainStopwatch = new Stopwatch();

        public GumService Gum => GumService.Default;
        public NixCars()
        {
            
            CFG = JObject.Parse(File.ReadAllText("app-settings.json"));

            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            game = this;

            graphicsPreset = CFG["GraphicsPreset"].Value<string>();
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;


            screenWidth = CFG["ScreenWidth"].Value<int>();
            screenHeight = CFG["ScreenHeight"].Value<int>();
            Graphics.PreferredBackBufferWidth = screenWidth;
            Graphics.PreferredBackBufferHeight = screenHeight;
            int dw = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int dh = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Window.Position = new Point((dw - screenWidth) / 2, (dh - screenHeight) / 2);
            screenCenter = new Point(screenWidth / 2 + Window.Position.X, screenHeight / 2 + Window.Position.Y);
            IsMouseVisible = false;


            //SetFPSLimit(CFG["FPSLimit"].Value<int>());

            //Graphics.SynchronizeWithVerticalRetrace = CFG["VSync"].Value<bool>();
            SetFPSLimit(CFG["FPSLimit"].Value<int>());
            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.ApplyChanges();

            if (!CFG.ContainsKey("ClientID"))
            {
                var ri = new Random().NextInt64();
                CFG["ClientID"] = (uint)ri;

                File.WriteAllText("app-settings.json", CFG.ToString());
            }
            Exiting += (s, e) => {

                NetworkManager.Client.Disconnect();
                NetworkManager.StopNetThread();
                mainStopwatch.Stop();

            };
        }
        GraphicalUiElement s1;
        protected override void Initialize()
        {
            var gumProyect = Gum.Initialize(this, "GUM/gum.gumx");

            var screen = gumProyect.Screens.Find(item => item.Name == "ScreenStart");
            s1 = screen.ToGraphicalUiElement();
            s1.AddToRoot();


            //s1.GetGraphicalUiElementByName
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

            GameStateManager.Init();
            LightVolume.Init();
            CarManager.Init();
            gizmos = new Gizmos();
            gizmos.LoadContent(GraphicsDevice);

            mainStopwatch.Start();
            NetworkManager.Connect();
            FloatingPlaneDrawer.Init();

        }

        protected override void Update(GameTime gameTime)
        {
            CheckGameRegainedFocus();

            gameState.Update(gameTime);
            
            gizmos.UpdateViewProjection(game.camera.view, game.camera.projection);



            Gum.Update(this, gameTime, s1);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            gameState.Draw(gameTime);

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

            switch (graphicsPreset)
            {
                case "ultra": surfaceFormat = SurfaceFormat.Vector4; lightResMultiplier = 1f; break;
                case "high": surfaceFormat = SurfaceFormat.HalfVector4; lightResMultiplier = 1f; break;
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
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, surfaceFormat, DepthFormat.Depth24Stencil8);
            bloomFilterTarget = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight, false, surfaceFormat, DepthFormat.Depth24Stencil8);
            blurHTarget = new RenderTarget2D(GraphicsDevice,
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, surfaceFormat, DepthFormat.Depth24Stencil8);
            blurVTarget = new RenderTarget2D(GraphicsDevice,
                (int)(screenWidth * lightResMultiplier), (int)(screenHeight * lightResMultiplier), false, surfaceFormat, DepthFormat.Depth24Stencil8);
        }
        public void SetFPSLimit(float l)
        {
            var fpslim = (int)l;
            if (fpslim >= 30)
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / fpslim);
                //CFG["FPSLimit"] = fpslim;
            }
            else
            {
                IsFixedTimeStep = false;
                //CFG["FPSLimit"] = 0;
            }


        }
    }
}
