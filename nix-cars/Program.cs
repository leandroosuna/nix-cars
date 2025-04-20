using System;
using System.IO;

var game = new nix_cars.NixCars();

try
{
    game.Run();
}
catch (Exception e)
{
    File.WriteAllText("CRASH-LOG.txt", $"MSG: {e.Message}\nFN: {e.TargetSite}\nTRACE: {e.StackTrace}\n");
    throw;
}
