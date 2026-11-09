using RimWorld;
using Verse;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines rules for evaluating whether cells, pawns, or things are eligible for capture designation and capture work.
/// Decouples eligibility validation from the user interface and AI decision-making.
/// </summary>
public interface ICaptureEligibilityService
{
    /// <summary>
    /// Evaluates whether the specified map cell is valid for placing a capture designation.
    /// </summary>
    /// <param name="loc">The target cell location.</param>
    /// <param name="map">The map containing the cell.</param>
    /// <returns>An <see cref="AcceptanceReport"/> indicating whether the cell can be designated.</returns>
    AcceptanceReport CanDesignateCell(IntVec3 loc, Map map);

    /// <summary>
    /// Evaluates whether the specified thing can be designated for capture.
    /// </summary>
    /// <param name="thing">The thing to evaluate.</param>
    /// <param name="map">The map containing the thing.</param>
    /// <returns>An <see cref="AcceptanceReport"/> indicating whether the thing can be designated.</returns>
    AcceptanceReport CanDesignateThing(Thing thing, Map map);

    /// <summary>
    /// Evaluates whether a target thing is currently a valid candidate for a capturer pawn to carry out capture work.
    /// </summary>
    /// <param name="capturer">The pawn attempting the capture.</param>
    /// <param name="target">The target thing to capture.</param>
    /// <param name="forced">Whether the action was forced by player command.</param>
    /// <returns><c>true</c> if the target can be captured; otherwise, <c>false</c>.</returns>
    bool IsEligibleForCaptureWork(Pawn capturer, Thing target, bool forced);
}
