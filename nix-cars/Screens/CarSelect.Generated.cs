//Code for CarSelect
using GumRuntime;
using MonoGameGum.GueDeriving;
using nix_cars.Components;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace nix_cars.Screens;
partial class CarSelect : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("CarSelect");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new CarSelect(visual);
            visual.Width = 0;
            visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(CarSelect)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("CarSelect", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public TextRuntime Title { get; protected set; }
    public ButtonStandard PrevCar { get; protected set; }
    public ButtonStandard Return { get; protected set; }
    public ContainerRuntime MainContainer { get; protected set; }
    public ButtonStandard NextCar { get; protected set; }
    public ButtonStandard Select { get; protected set; }
    public ContainerRuntime ColorPicker1 { get; protected set; }
    public ContainerRuntime ColorPicker2 { get; protected set; }
    public ContainerRuntime ColorPicker3 { get; protected set; }
    public ContainerRuntime SliderWLabel { get; protected set; }
    public ContainerRuntime SliderWLabel3 { get; protected set; }
    public ContainerRuntime SliderWLabel6 { get; protected set; }
    public ContainerRuntime SliderWLabel1 { get; protected set; }
    public ContainerRuntime SliderWLabel4 { get; protected set; }
    public ContainerRuntime SliderWLabel7 { get; protected set; }
    public ContainerRuntime SliderWLabel2 { get; protected set; }
    public ContainerRuntime SliderWLabel5 { get; protected set; }
    public ContainerRuntime SliderWLabel8 { get; protected set; }
    public Label PickerLabel { get; protected set; }
    public Label PickerLabel1 { get; protected set; }
    public Label PickerLabel2 { get; protected set; }
    public Slider R { get; protected set; }
    public Slider R1 { get; protected set; }
    public Slider R2 { get; protected set; }
    public Slider G { get; protected set; }
    public Slider G1 { get; protected set; }
    public Slider G2 { get; protected set; }
    public Slider B { get; protected set; }
    public Slider B1 { get; protected set; }
    public Slider B2 { get; protected set; }
    public Label RVal { get; protected set; }
    public Label RVal1 { get; protected set; }
    public Label RVal2 { get; protected set; }
    public Label GVal { get; protected set; }
    public Label GVal1 { get; protected set; }
    public Label GVal2 { get; protected set; }
    public Label BVal { get; protected set; }
    public Label BVal1 { get; protected set; }
    public Label BVal2 { get; protected set; }
    public ColoredRectangleRuntime SelectedColor { get; protected set; }
    public ColoredRectangleRuntime SelectedColor1 { get; protected set; }
    public ColoredRectangleRuntime SelectedColor2 { get; protected set; }

    public CarSelect(InteractiveGue visual) : base(visual) { }
    public CarSelect()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Title = this.Visual?.GetGraphicalUiElementByName("Title") as TextRuntime;
        PrevCar = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"PrevCar");
        Return = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Return");
        MainContainer = this.Visual?.GetGraphicalUiElementByName("MainContainer") as ContainerRuntime;
        NextCar = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"NextCar");
        Select = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Select");
        ColorPicker1 = this.Visual?.GetGraphicalUiElementByName("ColorPicker1") as ContainerRuntime;
        ColorPicker2 = this.Visual?.GetGraphicalUiElementByName("ColorPicker2") as ContainerRuntime;
        ColorPicker3 = this.Visual?.GetGraphicalUiElementByName("ColorPicker3") as ContainerRuntime;
        SliderWLabel = this.Visual?.GetGraphicalUiElementByName("SliderWLabel") as ContainerRuntime;
        SliderWLabel3 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel3") as ContainerRuntime;
        SliderWLabel6 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel6") as ContainerRuntime;
        SliderWLabel1 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel1") as ContainerRuntime;
        SliderWLabel4 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel4") as ContainerRuntime;
        SliderWLabel7 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel7") as ContainerRuntime;
        SliderWLabel2 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel2") as ContainerRuntime;
        SliderWLabel5 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel5") as ContainerRuntime;
        SliderWLabel8 = this.Visual?.GetGraphicalUiElementByName("SliderWLabel8") as ContainerRuntime;
        PickerLabel = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"PickerLabel");
        PickerLabel1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"PickerLabel1");
        PickerLabel2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"PickerLabel2");
        R = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"R");
        R1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"R1");
        R2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"R2");
        G = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"G");
        G1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"G1");
        G2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"G2");
        B = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"B");
        B1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"B1");
        B2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Slider>(this.Visual,"B2");
        RVal = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"RVal");
        RVal1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"RVal1");
        RVal2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"RVal2");
        GVal = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"GVal");
        GVal1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"GVal1");
        GVal2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"GVal2");
        BVal = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"BVal");
        BVal1 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"BVal1");
        BVal2 = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"BVal2");
        SelectedColor = this.Visual?.GetGraphicalUiElementByName("SelectedColor") as ColoredRectangleRuntime;
        SelectedColor1 = this.Visual?.GetGraphicalUiElementByName("SelectedColor1") as ColoredRectangleRuntime;
        SelectedColor2 = this.Visual?.GetGraphicalUiElementByName("SelectedColor2") as ColoredRectangleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
