using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;
using nix_cars;
using nix_cars.Components.Cars;
using nix_cars.Components.GUI;
using nix_cars.Components.States;
using RenderingLibrary.Graphics;

using System.Linq;

namespace nix_cars.Screens;
partial class CarSelect
{
    public Vector3 rgb0 = Vector3.One;
    public Vector3 rgb1 = Vector3.One;
    public Vector3 rgb2 = Vector3.One;

    static CarSelect instance;

    public static CarSelect GetInstance() { return instance; }

    public bool eventsEnabled = true;
    partial void CustomInitialize()
    {
        instance = this;
        Select.Click += Select_Click;
        NextCar.Click += NextCar_Click;
        PrevCar.Click += PrevCar_Click;
        Return.Click += Return_Click;

        R.ValueChanged += R_ValueChanged;
        G.ValueChanged += G_ValueChanged;
        B.ValueChanged += B_ValueChanged;

        R1.ValueChanged += R1_ValueChanged;
        G1.ValueChanged += G1_ValueChanged;
        B1.ValueChanged += B1_ValueChanged;

        R2.ValueChanged += R2_ValueChanged;
        G2.ValueChanged += G2_ValueChanged;
        B2.ValueChanged += B2_ValueChanged;

    }

    private void R_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb0.X = (float)(R.Value / 100);
        RVal.Text = $"{(int)(rgb0.X * 255)}";
        SelectedColor.Color = new Color(rgb0.X, rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }

    private void G_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb0.Y = (float)(G.Value / 100);
        GVal.Text = $"{(int)(rgb0.Y * 255)}";
        SelectedColor.Color = new Color(rgb0.X , rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }

    private void B_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb0.Z = (float)(B.Value / 100);
        BVal.Text = $"{(int)(rgb0.Z * 255)}";
        SelectedColor.Color = new Color(rgb0.X, rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }
    private void R1_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb1.X = (float)(R1.Value / 100);
        RVal1.Text = $"{(int)(rgb1.X * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
    }

    private void G1_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb1.Y = (float)(G1.Value / 100);
        GVal1.Text = $"{(int)(rgb1.Y * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
    }

    private void B1_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb1.Z = (float)(B1.Value / 100);
        BVal1.Text = $"{(int)(rgb1.Z * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
    }
    private void R2_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb2.X = (float)(R2.Value / 100);
        RVal2.Text = $"{(int)(rgb2.X * 255)}";
        SelectedColor2.Color = new Color(rgb2.X, rgb2.Y, rgb2.Z);
        GameStateManager.carSelect.SetCarColor(2, rgb2);
    }

    private void G2_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb2.Y = (float)(G2.Value / 100);
        GVal2.Text = $"{(int)(rgb2.Y * 255)}";
        SelectedColor2.Color = new Color(rgb2.X, rgb2.Y, rgb2.Z);
        GameStateManager.carSelect.SetCarColor(2, rgb2);
    }

    private void B2_ValueChanged(object sender, System.EventArgs e)
    {
        if (!eventsEnabled) return;
        rgb2.Z = (float)(B2.Value / 100);
        BVal2.Text = $"{(int)(rgb2.Z * 255)}";
        SelectedColor2.Color = new Color(rgb2.X, rgb2.Y, rgb2.Z);
        GameStateManager.carSelect.SetCarColor(2, rgb2);
    }


    private void PrevCar_Click(object sender, System.EventArgs e)
    {
        GameStateManager.carSelect.InFocusChangeBy(1);

    }

    private void NextCar_Click(object sender, System.EventArgs e)
    {
        GameStateManager.carSelect.InFocusChangeBy(-1);


    }
    private void Select_Click(object sender, System.EventArgs e)
    {
        GameStateManager.carSelect.SelectAndEnter();
    }

    private void Return_Click(object sender, System.EventArgs e)
    {
        GameStateManager.SwitchTo(State.MAIN);
        
    }
}
