using CaptureThem.Services;
using HarmonyLib;
using RimWorld;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony postfix patch for <see cref="LordToil_PanicFlee.Init"/>.
/// Triggers automated capture designation for downed hostile pawns when raid enemies begin fleeing.
/// Follows Single Responsibility by delegating the handling logic to <see cref="CaptureThemServices.AutoCaptureService"/>.
/// </summary>
[HarmonyPatch(typeof(LordToil_PanicFlee), nameof(LordToil_PanicFlee.Init))]
public static class LordToil_PanicFlee_Init
{
    /// <summary>
    /// Postfix patch executed after LordToil_PanicFlee has initialized.
    /// </summary>
    /// <param name="__instance">The <see cref="LordToil_PanicFlee"/> instance.</param>
    public static void Postfix(LordToil_PanicFlee __instance)
    {
        if (__instance?.lord != null)
        {
            CaptureThemServices.AutoCaptureService.NotifyRaidFleeing(__instance.lord);
        }
    }
}
