using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using nix_cars;
using nix_cars.Components.States;
using RenderingLibrary.Graphics;

using System.Linq;

partial class StartMenu
{
    NixCars game;
    System.Timers.Timer timer;
    partial void CustomInitialize()
    {
        game = NixCars.GameInstance();
        Start.Click += Start_Click;
        Options.Click += Options_Click;
        Exit.Click += Exit_Click;

        var timeout = 2000; //ms
        timer = new System.Timers.Timer(timeout);
        timer.Elapsed += Timer_Elapsed;
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

        ScreenSave screen = NixCars.GumProject.Screens.Find(item => item.Name == "Options");
        NixCars.GumRoot.RemoveFromRoot();
        NixCars.GumRoot = screen.ToGraphicalUiElement();
        NixCars.GumRoot.AddToRoot();

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
            GameStateManager.SwitchTo(State.RUN);
        }
    }
}
