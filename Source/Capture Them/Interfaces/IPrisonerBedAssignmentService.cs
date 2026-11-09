using RimWorld;
using Verse;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines services for finding and verifying reservation of prisoner beds.
/// Isolates bed allocation and reservation checks from the work-giver logic.
/// </summary>
public interface IPrisonerBedAssignmentService
{
    /// <summary>
    /// Searches for a valid prisoner bed suitable for the designated victim.
    /// </summary>
    /// <param name="victim">The downed pawn to be imprisoned.</param>
    /// <param name="capturer">The pawn who will transport the victim.</param>
    /// <param name="allowFallback">Whether fallback beds (e.g. medical/non-ideal beds) are checked if primary search fails.</param>
    /// <returns>A suitable <see cref="Building_Bed"/>, or <c>null</c> if no bed is available.</returns>
    Building_Bed? FindBedFor(Pawn victim, Pawn capturer, bool allowFallback = true);

    /// <summary>
    /// Verifies whether the capturer pawn can reserve the specified bed.
    /// </summary>
    /// <param name="capturer">The capturing pawn.</param>
    /// <param name="bed">The candidate bed.</param>
    /// <param name="forced">Whether the action is forced by the player.</param>
    /// <returns><c>true</c> if the bed is reservable; otherwise, <c>false</c>.</returns>
    bool CanReserveBed(Pawn capturer, Building_Bed bed, bool forced);
}
