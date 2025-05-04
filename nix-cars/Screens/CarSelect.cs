using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;
using nix_cars;
using nix_cars.Components.GUI;
using nix_cars.Components.States;
using RenderingLibrary.Graphics;

using System.Linq;

namespace nix_cars.Screens;
partial class CarSelect
{
    public Vector3 rgb0 = Vector3.One;
    public Vector3 rgb1 = Vector3.One;
 
    partial void CustomInitialize()
    {
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

    }

    private void R_ValueChanged(object sender, System.EventArgs e)
    {
        rgb0.X = (float)(R.Value / 100);
        RVal.Text = $"{(int)(rgb0.X * 255)}";
        SelectedColor.Color = new Color(rgb0.X, rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }

    private void G_ValueChanged(object sender, System.EventArgs e)
    {
        rgb0.Y = (float)(G.Value / 100);
        GVal.Text = $"{(int)(rgb0.Y * 255)}";
        SelectedColor.Color = new Color(rgb0.X , rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }

    private void B_ValueChanged(object sender, System.EventArgs e)
    {
        rgb0.Z = (float)(B.Value / 100);
        BVal.Text = $"{(int)(rgb0.Z * 255)}";
        SelectedColor.Color = new Color(rgb0.X, rgb0.Y, rgb0.Z);
        GameStateManager.carSelect.SetCarColor(0, rgb0);
    }
    private void R1_ValueChanged(object sender, System.EventArgs e)
    {
        rgb1.X = (float)(R1.Value / 100);
        RVal1.Text = $"{(int)(rgb1.X * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
    }

    private void G1_ValueChanged(object sender, System.EventArgs e)
    {
        rgb1.Y = (float)(G1.Value / 100);
        GVal1.Text = $"{(int)(rgb1.Y * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
    }

    private void B1_ValueChanged(object sender, System.EventArgs e)
    {
        rgb1.Z = (float)(B1.Value / 100);
        BVal1.Text = $"{(int)(rgb1.Z * 255)}";
        SelectedColor1.Color = new Color(rgb1.X, rgb1.Y, rgb1.Z);
        GameStateManager.carSelect.SetCarColor(1, rgb1);
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
        GameStateManager.SwitchTo(State.RUN);
    }

    private void Return_Click(object sender, System.EventArgs e)
    {
        GameStateManager.SwitchTo(State.MAIN);
        
    }
}
