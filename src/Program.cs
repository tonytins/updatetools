using TonyBark.UpdateDotNetTools;

Console.WriteLine("Starting update all dotnet tools");
Console.WriteLine("Finding installed tools");
var prog = @"UpdateDotNetTools";
foreach (var cmd in UpdateTool.Parse(UpdateTool.Command("dotnet", "tool list -g")))
{
    if (cmd.Equals(prog, StringComparison.OrdinalIgnoreCase))
    {
        /** TODO: Update the tool itself. If it is under MAC, 
         * it can delete the running software. But after deleting, calling Process.Start
         *  will throw Win32Exception Can't find itself 
         **/
    }
    else
    {
        UpdateTool.TryUpdate(cmd);
    }
}

// TODO: Try to update itself, it can be updated under MAC devices, but not under Windows
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    UpdateTool.TryUpdate(prog);
}

Console.WriteLine("Update finished");

