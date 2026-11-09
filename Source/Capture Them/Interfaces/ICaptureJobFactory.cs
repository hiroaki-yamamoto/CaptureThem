using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines a factory for constructing capture <see cref="Job"/> instances and recording player tutorial knowledge.
/// Follows the Single Responsibility Principle by decoupling Job creation and knowledge progression from the scheduling work-giver.
/// </summary>
public interface ICaptureJobFactory
{
    /// <summary>
    /// Creates a capture job directing the capturer pawn to carry the victim to the designated bed.
    /// Also marks the Capturing concept as demonstrated in the player knowledge database.
    /// </summary>
    /// <param name="capturer">The colonist pawn executing the capture.</param>
    /// <param name="victim">The downed pawn to capture.</param>
    /// <param name="bed">The destination prisoner bed.</param>
    /// <returns>A configured <see cref="Job"/> ready for AI execution.</returns>
    Job CreateCaptureJob(Pawn capturer, Pawn victim, Building_Bed bed);
}
