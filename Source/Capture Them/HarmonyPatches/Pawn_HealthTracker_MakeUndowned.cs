using CaptureThem.Services;
using HarmonyLib;
using Verse;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony prefix patch for <see cref="Pawn_HealthTracker"/>'s internal MakeUndowned method.
/// Removes the capture designation when a pawn regains consciousness or is no longer downed.
/// Follows Single Responsibility by delegating designation cleanup to <see cref="CaptureThemServices.DesignationService"/>.
/// </summary>
[HarmonyPatch(typeof(Pawn_HealthTracker), "MakeUndowned")]
public static class Pawn_HealthTracker_MakeUndowned
{
    /// <summary>
    /// Prefix patch executed before MakeUndowned runs.
    /// </summary>
    /// <param name="___pawn">The pawn field injected by Harmony from the target instance.</param>
    public static void Prefix(Pawn ___pawn)
    {
        CaptureThemServices.DesignationService.TryRemoveDesignation(___pawn);
    }
}
