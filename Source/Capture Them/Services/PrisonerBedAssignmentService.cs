using CaptureThem.Interfaces;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for finding and reserving prisoner beds for capture candidates.
/// Encapsulates interaction with RimWorld's <see cref="RestUtility"/>.
/// </summary>
public class PrisonerBedAssignmentService : IPrisonerBedAssignmentService
{
    /// <inheritdoc />
    public Building_Bed? FindBedFor(Pawn victim, Pawn capturer, bool allowFallback = true)
    {
        if (victim == null || capturer == null)
        {
            return null;
        }

        var primaryBed = RestUtility.FindBedFor(victim, capturer, false, false, GuestStatus.Prisoner);
        if (primaryBed != null || !allowFallback)
        {
            return primaryBed;
        }

        return RestUtility.FindBedFor(victim, capturer, false, true, GuestStatus.Prisoner);
    }

    /// <inheritdoc />
    public bool CanReserveBed(Pawn capturer, Building_Bed bed, bool forced)
    {
        if (capturer == null || bed == null)
        {
            return false;
        }

        return capturer.CanReserve(bed, 1, -1, null, forced);
    }
}
