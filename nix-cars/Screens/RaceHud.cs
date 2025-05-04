using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace nix_cars.Screens;
partial class RaceHud
{
    static RaceHud instance;
    partial void CustomInitialize()
    {
        instance = this;
    }
    public static RaceHud GetInstance()
    {
        return instance;
    }

}
