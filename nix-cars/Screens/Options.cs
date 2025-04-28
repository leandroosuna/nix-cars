using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using nix_cars;
using RenderingLibrary.Graphics;

using System.Linq;

partial class Options
{
    System.Timers.Timer timer;
    partial void CustomInitialize()
    {
        var lbi = new ListBoxItem();
        lbi.ListItemDisplayText = "1280 x 720";
        ResComboBox.ListBoxInstance.AddChild(lbi);
        ResComboBox.SelectedIndex = 0;

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

        NameTags.IsChecked = true;
        BoostBar.IsChecked = true;
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

    private void ResComboBox_SelectionChanged(object arg1, SelectionChangedEventArgs arg2)
    {
        ToastError.TextInstance.Text = ResComboBox.SelectedObject.ToString();
        ToastError.IsVisible = true;
        timer.Start();
    }

    private void Exit_Click(object sender, System.EventArgs e)
    {
        ScreenSave screen = NixCars.GumProject.Screens.Find(item => item.Name == "StartMenu");
        NixCars.GumRoot.RemoveFromRoot();
        NixCars.GumRoot = screen.ToGraphicalUiElement();
        NixCars.GumRoot.AddToRoot();
    }
}
