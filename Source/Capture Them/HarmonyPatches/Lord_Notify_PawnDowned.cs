using CaptureThem.Services;
using HarmonyLib;
using Verse;
using Verse.AI.Group;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony postfix patch for <see cref="Lord.Notify_PawnDowned"/>.
/// Evaluates whether all raiders in the hostile Lord have been rendered non-mobile,
/// and if so, triggers automated capture designation for eligible downed enemies.
/// Follows Single Responsibility by delegating the handling logic to <see cref="CaptureThemServices.AutoCaptureService"/>.
/// </summary>
[HarmonyPatch(typeof(Lord), nameof(Lord.Notify_PawnDowned))]
public static class Lord_Notify_PawnDowned
{
    /// <summary>
    /// Postfix patch executed after a pawn belonging to the lord has been downed.
    /// </summary>
    /// <param name="__instance">The <see cref="Lord"/> instance.</param>
    /// <param name="p">The pawn that was downed.</param>
    public static void Postfix(Lord __instance, Pawn p)
    {
        if (__instance != null)
        {
            CaptureThemServices.AutoCaptureService.NotifyRaidPawnDowned(__instance, p);
        }
    }
}
