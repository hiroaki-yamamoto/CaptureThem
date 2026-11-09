using Verse;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines messaging and alert notifications for player feedback during capture operations.
/// Isolates user notification mechanisms from domain logic to improve testability and maintainability.
/// </summary>
public interface ICaptureNotificationService
{
    /// <summary>
    /// Displays a caution message to the player when designating a pawn whose faction is non-hostile,
    /// warning that capturing will harm faction relations.
    /// </summary>
    /// <param name="victim">The non-hostile pawn being designated.</param>
    void WarnIfCapturingWillAngerFaction(Pawn victim);

    /// <summary>
    /// Displays a rejection notification when a capture job cannot proceed because no prisoner bed is available.
    /// </summary>
    /// <param name="victim">The pawn that could not be captured due to lack of beds.</param>
    void NotifyNoPrisonerBedAvailable(Pawn victim);
}
