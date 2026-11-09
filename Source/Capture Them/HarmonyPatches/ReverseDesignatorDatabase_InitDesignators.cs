using CaptureThem;
using HarmonyLib;
using Verse;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony postfix patch for <see cref="ReverseDesignatorDatabase.InitDesignators"/>.
/// Registers the <see cref="Designator_CapturePawn"/> designator into the reverse designator database,
/// making it available when selecting pawns in the game world.
/// </summary>
[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
public static class ReverseDesignatorDatabase_InitDesignators
{
    /// <summary>
    /// Postfix patch that adds the capture designator to the reverse designator database.
    /// </summary>
    /// <param name="__instance">The <see cref="ReverseDesignatorDatabase"/> instance being initialized.</param>
    public static void Postfix(ref ReverseDesignatorDatabase __instance)
    {
        __instance.AllDesignators.Add(new Designator_CapturePawn());
    }
}
