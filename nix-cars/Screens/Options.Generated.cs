//Code for Options
using GumRuntime;
using MonoGameGum.GueDeriving;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

partial class Options : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Options");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Options(visual);
            visual.Width = 0;
            visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Options)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Options", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime MainContainer { get; protected set; }
    public TextRuntime Title { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public ComboBox ResComboBox { get; protected set; }
    public ButtonStandard Exit { get; protected set; }
    public Label ResLabel { get; protected set; }
    public Label QLightsLabel { get; protected set; }
    public ComboBox QLightsComboBox { get; protected set; }
    public ContainerRuntime Res { get; protected set; }
    public CheckBox FullScreen { get; protected set; }
    public ContainerRuntime QLights { get; protected set; }
    public CheckBox NameTags { get; protected set; }
    public CheckBox BoostBar { get; protected set; }
    public Toast ToastError { get; protected set; }

    public Options(InteractiveGue visual) : base(visual) { }
    public Options()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        MainContainer = this.Visual?.GetGraphicalUiElementByName("MainContainer") as ContainerRuntime;
        Title = this.Visual?.GetGraphicalUiElementByName("Title") as TextRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as TextRuntime;
        ResComboBox = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ComboBox>(this.Visual,"ResComboBox");
        Exit = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ButtonStandard>(this.Visual,"Exit");
        ResLabel = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"ResLabel");
        QLightsLabel = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Label>(this.Visual,"QLightsLabel");
        QLightsComboBox = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<ComboBox>(this.Visual,"QLightsComboBox");
        Res = this.Visual?.GetGraphicalUiElementByName("Res") as ContainerRuntime;
        FullScreen = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<CheckBox>(this.Visual,"FullScreen");
        QLights = this.Visual?.GetGraphicalUiElementByName("QLights") as ContainerRuntime;
        NameTags = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<CheckBox>(this.Visual,"NameTags");
        BoostBar = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<CheckBox>(this.Visual,"BoostBar");
        ToastError = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<Toast>(this.Visual,"ToastError");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
