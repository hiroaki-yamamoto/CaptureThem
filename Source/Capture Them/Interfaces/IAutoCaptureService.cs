using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureThem.Interfaces;

/// <summary>
/// Service responsible for managing automated designation of eligible downed enemies following raid events.
/// Implements the Single Responsibility Principle by encapsulating raid completion detection and post-raid capture orchestration.
/// </summary>
public interface IAutoCaptureService
{
    /// <summary>
    /// Determines whether the specified <see cref="Lord"/> represents an enemy raid targeted at a player colony.
    /// </summary>
    /// <param name="lord">The lord group to evaluate.</param>
    /// <returns><c>true</c> if the lord belongs to a hostile faction and executes a colony raid job; otherwise, <c>false</c>.</returns>
    bool IsHostileColonyRaid(Lord? lord);

    /// <summary>
    /// Automatically designates all eligible downed enemy pawns on the given map for capture.
    /// </summary>
    /// <param name="map">The map where the raid took place.</param>
    /// <param name="raidFaction">The raid faction, or <c>null</c> to capture downed pawns from any hostile faction.</param>
    /// <returns>The number of pawns newly designated for capture.</returns>
    int AutoCaptureDownedEnemiesAfterRaid(Map? map, Faction? raidFaction = null);

    /// <summary>
    /// Handles notification when a raid Lord transitions into panic fleeing state.
    /// </summary>
    /// <param name="lord">The raid Lord that began fleeing.</param>
    void NotifyRaidFleeing(Lord? lord);

    /// <summary>
    /// Handles notification when a pawn belonging to a raid Lord is downed.
    /// Triggers automated capture if all remaining raiders in the group are no longer mobile.
    /// </summary>
    /// <param name="lord">The lord the downed pawn belongs to.</param>
    /// <param name="downedPawn">The pawn that was downed.</param>
    void NotifyRaidPawnDowned(Lord? lord, Pawn? downedPawn);

    /// <summary>
    /// Handles notification when a raid Lord is destroyed or cleaned up.
    /// </summary>
    /// <param name="lord">The lord being cleaned up or destroyed.</param>
    void NotifyRaidLordDestroyed(Lord? lord);
}
