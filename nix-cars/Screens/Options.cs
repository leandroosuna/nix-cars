using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using Newtonsoft.Json.Linq;
using nix_cars;
using RenderingLibrary.Graphics;
using SharpDX.Direct3D11;
using System;
using System.Diagnostics;
using System.Linq;

partial class Options
{
    System.Timers.Timer timer;
    NixCars game;
    partial void CustomInitialize()
    {
        game = NixCars.GameInstance();

        var lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "1280 x 720";
        ResComboBox.ListBoxInstance.AddChild(lbi);
        
        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "1366 x 768";
        ResComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "1600 x 900";
        ResComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "1920 x 1080";
        ResComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "2560 x 1440";
        ResComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "3840 x 2160";
        ResComboBox.ListBoxInstance.AddChild(lbi);

        ResComboBox.SelectedIndex = ResIndexFromCFG();
        ResComboBox.SelectionChanged += ResComboBox_SelectionChanged;

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "Ultra";
        QLightsComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "Alto";
        QLightsComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "Medio";
        QLightsComboBox.ListBoxInstance.AddChild(lbi);

        lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "Bajo";
        QLightsComboBox.ListBoxInstance.AddChild(lbi);

        QLightsComboBox.SelectedIndex = 0;
        QLightsComboBox.SelectionChanged += QLightsComboBox_SelectionChanged;
        NameTags.IsChecked = true;
        BoostBar.IsChecked = true;
        Exit.Click += Exit_Click;

        FPSLimit.ValueChanged += FPSLimit_ValueChanged;

        FullScreen.Checked += FullScreen_Checked;
        FullScreen.Unchecked += FullScreen_Unchecked;


        var timeout = 2000; //ms
        timer = new System.Timers.Timer(timeout);
        timer.Elapsed += Timer_Elapsed;
    }

    private void QLightsComboBox_SelectionChanged(object arg1, SelectionChangedEventArgs arg2)
    {
        switch(QLightsComboBox.SelectedIndex)
        {
            case 0: game.lightQuality = "ultra" ;break;
            case 1: game.lightQuality = "high"; break;
            case 2: game.lightQuality = "medium"; break;
            case 3: game.lightQuality = "low"; break;
        }
        game.SetupRenderTargets();
    }

    private void FullScreen_Checked(object sender, EventArgs e)
    {
        game.SetFullScreen(true);
    }
    private void FullScreen_Unchecked(object sender, EventArgs e)
    {
        game.SetFullScreen(false);
    }
    private void FPSLimit_ValueChanged(object sender, System.EventArgs e)
    {
        var l = FPSLimit.Value;

        if(l < 5)
        {
            FPSValue.Text = "VSync";
            game.SetFPSLimit(-1);
        }  
        else if(l > 95)
        {
            FPSValue.Text = "Ilimitado";
            game.SetFPSLimit(0);
        }
        else
        {
            var fpsLim = (int)NixCars.Map(l, 5, 95, 30, 400);
            FPSValue.Text = $"{fpsLim}";
            game.SetFPSLimit(fpsLim);
        }

    }
    int ResIndexFromCFG()
    {
        switch(game.CFG["ScreenWidth"].Value<int>())
        {
            case 1280: return 0;
            case 1366: return 1;
            case 1600: return 2;
            case 1920: return 3;
            case 2560: return 4;
            case 3840: return 5;
        }

        return 0;
    }

    private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        ToastError.IsVisible = false;
        timer.Stop();
    }

    int previousIndex = 0;
    private void ResComboBox_SelectionChanged(object arg1, SelectionChangedEventArgs arg2)
    {
        var res = ResComboBox.SelectedObject.ToString();
        var split = res.Split(" ");
        var w = Convert.ToInt32(split[0]);
        var h = Convert.ToInt32(split[2]);

        
        if (w > NixCars.displayWidth || h >  NixCars.displayHeight)
        {
            ToastError.TextInstance.Text = "Una resolucion mas alta que la de tu pantalla no, pa";
            ToastError.IsVisible = true;
            timer.Start();
            ResComboBox.SelectedIndex = previousIndex;
            return;
        }

        previousIndex = ResComboBox.SelectedIndex;

        game.ChangeResolution(w, h);
    }

    private void Exit_Click(object sender, System.EventArgs e)
    {
        ScreenSave screen = NixCars.GumProject.Screens.Find(item => item.Name == "StartMenu");
        NixCars.GumRoot.RemoveFromRoot();
        NixCars.GumRoot = screen.ToGraphicalUiElement();
        NixCars.GumRoot.AddToRoot();
    }
}
