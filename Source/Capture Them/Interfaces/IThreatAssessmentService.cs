using Verse;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines threat evaluation capabilities to detect nearby hostiles and dangerous conditions around pawns.
/// Follows the Single Responsibility Principle by isolating threat perception logic from work-givers.
/// </summary>
public interface IThreatAssessmentService
{
    /// <summary>
    /// Checks if any hostile targets or imminent threats are within the specified radial distance of the target pawn.
    /// </summary>
    /// <param name="capturer">The pawn performing the capture.</param>
    /// <param name="target">The pawn being evaluated for safety.</param>
    /// <param name="radius">The radial search distance in map cells.</param>
    /// <returns><c>true</c> if danger is detected nearby; otherwise, <c>false</c>.</returns>
    bool IsDangerNear(Pawn capturer, Pawn target, float radius);
}
