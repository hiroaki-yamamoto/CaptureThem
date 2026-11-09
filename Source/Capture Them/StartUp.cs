using System.Reflection;
using HarmonyLib;
using Verse;

namespace CaptureThem;

/// <summary>
/// Static initialization handler executing on RimWorld startup.
/// Applies Harmony patches defined within the assembly.
/// </summary>
[StaticConstructorOnStartup]
public static class StartUp
{
    static StartUp()
    {
        new Harmony("CaptureThem.patch").PatchAll(Assembly.GetExecutingAssembly());
    }
}
