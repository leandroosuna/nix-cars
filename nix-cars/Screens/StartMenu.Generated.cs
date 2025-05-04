//Code for StartMenu
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
partial class StartMenu : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("StartMenu");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new StartMenu(visual);
            visual.Width = 0;
            visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(StartMenu)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("StartMenu", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ButtonStandard Start { get; protected set; }
    public ButtonStandard Options { get; protected set; }
    public ButtonStandard Exit { get; protected set; }
    public ContainerRuntime MainContainer { get; protected set; }
    public TextBox NameBox { get; protected set; }
    public Label NameBoxLabel { get; protected set; }
    public ContainerRuntime NameBoxContainer { get; protected set; }
    public Toast ToastError { get; protected set; }
    public TextRuntime Title { get; protected set; }

    public StartMenu(InteractiveGue visual) : base(visual) { }
    public StartMenu()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Start = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Start");
        Options = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Options");
        Exit = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Exit");
        MainContainer = this.Visual?.GetGraphicalUiElementByName("MainContainer") as ContainerRuntime;
        NameBox = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<TextBox>(this.Visual,"NameBox");
        NameBoxLabel = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"NameBoxLabel");
        NameBoxContainer = this.Visual?.GetGraphicalUiElementByName("NameBoxContainer") as ContainerRuntime;
        ToastError = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Toast>(this.Visual,"ToastError");
        Title = this.Visual?.GetGraphicalUiElementByName("Title") as TextRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
