//Code for RaceHud
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
partial class RaceHud : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("RaceHud");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new RaceHud(visual);
            visual.Width = 0;
            visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(RaceHud)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("RaceHud", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public SpriteRuntime HitIndicator { get; protected set; }
    public TextRuntime TitleMsg { get; protected set; }
    public TextRuntime Lap { get; protected set; }
    public TextRuntime Positions { get; protected set; }
    public TextRuntime Countdown { get; protected set; }
    public ColoredRectangleRuntime AbilityR0 { get; protected set; }
    public ColoredRectangleRuntime AbilityR1 { get; protected set; }
    public ColoredRectangleRuntime AbilityR2 { get; protected set; }
    public SpriteRuntime sprA0 { get; protected set; }
    public SpriteRuntime sprA1 { get; protected set; }
    public SpriteRuntime sprA2 { get; protected set; }
    public ContainerRuntime AbilityIndicator0 { get; protected set; }
    public ContainerRuntime AbilityIndicator1 { get; protected set; }
    public ContainerRuntime AbilityIndicator2 { get; protected set; }
    public TextBox CommandBox { get; protected set; }
    public TextRuntime ServerResponse { get; protected set; }
    public TextRuntime RTT { get; protected set; }
    public TextRuntime FPS { get; protected set; }

    public RaceHud(InteractiveGue visual) : base(visual) { }
    public RaceHud()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        HitIndicator = this.Visual?.GetGraphicalUiElementByName("HitIndicator") as SpriteRuntime;
        TitleMsg = this.Visual?.GetGraphicalUiElementByName("TitleMsg") as TextRuntime;
        Lap = this.Visual?.GetGraphicalUiElementByName("Lap") as TextRuntime;
        Positions = this.Visual?.GetGraphicalUiElementByName("Positions") as TextRuntime;
        Countdown = this.Visual?.GetGraphicalUiElementByName("Countdown") as TextRuntime;
        AbilityR0 = this.Visual?.GetGraphicalUiElementByName("AbilityR0") as ColoredRectangleRuntime;
        AbilityR1 = this.Visual?.GetGraphicalUiElementByName("AbilityR1") as ColoredRectangleRuntime;
        AbilityR2 = this.Visual?.GetGraphicalUiElementByName("AbilityR2") as ColoredRectangleRuntime;
        sprA0 = this.Visual?.GetGraphicalUiElementByName("sprA0") as SpriteRuntime;
        sprA1 = this.Visual?.GetGraphicalUiElementByName("sprA1") as SpriteRuntime;
        sprA2 = this.Visual?.GetGraphicalUiElementByName("sprA2") as SpriteRuntime;
        AbilityIndicator0 = this.Visual?.GetGraphicalUiElementByName("AbilityIndicator0") as ContainerRuntime;
        AbilityIndicator1 = this.Visual?.GetGraphicalUiElementByName("AbilityIndicator1") as ContainerRuntime;
        AbilityIndicator2 = this.Visual?.GetGraphicalUiElementByName("AbilityIndicator2") as ContainerRuntime;
        CommandBox = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<TextBox>(this.Visual,"CommandBox");
        ServerResponse = this.Visual?.GetGraphicalUiElementByName("ServerResponse") as TextRuntime;
        RTT = this.Visual?.GetGraphicalUiElementByName("RTT") as TextRuntime;
        FPS = this.Visual?.GetGraphicalUiElementByName("FPS") as TextRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
