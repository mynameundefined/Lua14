﻿using Lua14.Lua.Data;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using System.Reflection;
using Lua14.Lua.Objects;

namespace Lua14;

public sealed class EntryPoint : GameShared
{
    public override void PreInit()
    {
        Assembly subversionAssembly = Assembly.GetExecutingAssembly();
        SubverterPatch.Harm.PatchAll(subversionAssembly);
    }

    public override void PostInit()
    {
        MarseyLogger.Log(MarseyLogger.LogType.DEBG, "IoC BuildGraph started.");

        IoCManager.BuildGraph();
        LoadLuaMods();
    }

    private static void LoadLuaMods()
    {
        var mods = DataLoader.ReadAutoexecFolder();

        MarseyLogger.Log(MarseyLogger.LogType.DEBG, "Starting execution of all mods in autoexec...");
        foreach (var mod in mods)
        {
            try
            {
                Runner runner = new(mod);
                runner.ExecuteMain();
            }
            catch (Exception e)
            {
                MarseyLogger.Log(MarseyLogger.LogType.FATL, $"Failed to start mod with name {mod.Name}");
                MarseyLogger.Log(MarseyLogger.LogType.FATL, $"Exception: {e}");
            }
        }
    }
}