using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using Newtonsoft.Json.Linq;
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
        var nb = game.CFG["PlayerName"].Value<string>();

        
        foreach (var c in nb)
            NameBox.HandleCharEntered(c);

        NameBox.IsFocused = true;

        NameBox.TextChanged += NameBox_TextChanged;
    }

    private void NameBox_TextChanged(object sender, System.EventArgs e)
    {
        var nt = NameBox.Text;
        if(nt.Contains('ñ'))
        {
            NameBox.Text = nt.Replace('ñ', 'n');
        }
        
        if (nt.Contains('á'))
        {
            NameBox.Text = nt.Replace('á', 'a');
        }
        if (nt.Contains('é'))
        {
            NameBox.Text = nt.Replace('é', 'e');
        }
        if (nt.Contains('í'))
        {
            NameBox.Text = nt.Replace('í', 'i');
        }
        if (nt.Contains('ó'))
        {
            NameBox.Text = nt.Replace('ó', 'o');
        }
        if (nt.Contains('ú'))
        {
            NameBox.Text = nt.Replace('ú', 'u');
        }

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
        GameStateManager.mainMenu.HandleEnterGame();
    }

    public static StartMenu GetInstance()
    {
        return instance;
    }
}
