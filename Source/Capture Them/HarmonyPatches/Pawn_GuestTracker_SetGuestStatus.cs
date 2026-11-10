using CaptureThem.Services;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CaptureThem.HarmonyPatches;

/// <summary>
/// Harmony postfix patch for <see cref="Pawn_GuestTracker.SetGuestStatus"/>.
/// Removes the capture designation once a pawn successfully transitions to prisoner status.
/// Follows Single Responsibility by delegating designation cleanup to <see cref="CaptureThemServices.DesignationService"/>.
/// </summary>
[HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus))]
public static class Pawn_GuestTracker_SetGuestStatus
{
    /// <summary>
    /// Postfix patch invoked after guest status has been updated.
    /// </summary>
    /// <param name="___pawn">The pawn field injected by Harmony from the target instance.</param>
    /// <param name="guestStatus">The new guest status applied to the pawn.</param>
    public static void Postfix(Pawn ___pawn, GuestStatus guestStatus)
    {
        if (guestStatus == GuestStatus.Prisoner)
        {
            CaptureThemServices.DesignationService.TryRemoveDesignation(___pawn);
        }
        else
        {
            // When a pawn transitions away from prisoner status (e.g. released, recruited, or transferred),
            // a prisoner bed may have become available. Invalidate negative bed cache immediately.
            CaptureThemServices.BedAssignmentService.InvalidateBedCache(___pawn?.Map ?? ___pawn?.MapHeld);
        }
    }
}
