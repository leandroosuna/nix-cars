using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using nix_cars;
using nix_cars.Components.GUI;
using nix_cars.Components.States;
using RenderingLibrary.Graphics;

using System.Linq;

namespace nix_cars.Screens;
partial class StartMenu
{
    NixCars game;
    public System.Timers.Timer timer;
    static StartMenu instance;
    partial void CustomInitialize()
    {
        instance = this;
        game = NixCars.GameInstance();
        Start.Click += Start_Click;
        Options.Click += Options_Click;
        Exit.Click += Exit_Click;

        var timeout = 2000; //ms
        timer = new System.Timers.Timer(timeout);
        timer.Elapsed += Timer_Elapsed;

        NameBox.IsFocused = true;
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        ToastError.IsVisible = false;
        timer.Stop();
    }

    private void Exit_Click(object sender, System.EventArgs e)
    {
        game.Exit();
    }

    private void Options_Click(object sender, System.EventArgs e)
    {
        GumManager.SwitchTo(Screen.OPTIONS);
    }

    private void Start_Click(object sender, System.EventArgs e)
    {
        if(NameBox.Text == "")
        {
            ToastError.TextInstance.Text = "Ingresa tu nombre para entrar";
            ToastError.IsVisible = true;
            timer.Start();
        }
        else
        {
            GameStateManager.SwitchTo(State.CARSELECT);
        }
    }

    public static StartMenu GetInstance()
    {
        return instance;
    }
}
