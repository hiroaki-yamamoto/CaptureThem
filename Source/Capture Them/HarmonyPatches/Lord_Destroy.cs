using CaptureThem.Services;
using HarmonyLib;
using Verse.AI.Group;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony postfix patch for <see cref="Lord.Destroy"/>.
/// Ensures that any remaining eligible downed hostile pawns are designated for capture
/// when a raid Lord group is destroyed or cleaned up.
/// Follows Single Responsibility by delegating the handling logic to <see cref="CaptureThemServices.AutoCaptureService"/>.
/// </summary>
[HarmonyPatch(typeof(Lord), "Destroy")]
public static class Lord_Destroy
{
    /// <summary>
    /// Postfix patch executed after a Lord has been destroyed.
    /// </summary>
    /// <param name="__instance">The <see cref="Lord"/> instance being destroyed.</param>
    public static void Postfix(Lord __instance)
    {
        if (__instance != null)
        {
            CaptureThemServices.AutoCaptureService.NotifyRaidLordDestroyed(__instance);
        }
    }
}
