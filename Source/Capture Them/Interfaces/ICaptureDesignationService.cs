using System.Collections.Generic;
using Verse;

namespace CaptureThem.Interfaces;

/// <summary>
/// Defines services for managing capture designations on pawns and other things in the game world.
/// Follows the Single Responsibility Principle by decoupling designation lifecycle management from UI and AI work-givers.
/// </summary>
public interface ICaptureDesignationService
{
    /// <summary>
    /// Gets the <see cref="DesignationDef"/> representing the capture designation.
    /// </summary>
    DesignationDef DesignationDef { get; }

    /// <summary>
    /// Checks whether the specified thing currently has an active capture designation.
    /// </summary>
    /// <param name="thing">The thing to inspect.</param>
    /// <param name="map">The map where the designation resides, or <c>null</c> to infer from the thing.</param>
    /// <returns><c>true</c> if the target has an active capture designation; otherwise, <c>false</c>.</returns>
    bool HasDesignation(Thing thing, Map? map = null);

    /// <summary>
    /// Designates the specified thing for capture and clears existing designations.
    /// </summary>
    /// <param name="thing">The thing to designate for capture.</param>
    /// <param name="map">The map where the thing exists.</param>
    void Designate(Thing thing, Map map);

    /// <summary>
    /// Attempts to remove any capture designation on the specified thing.
    /// Safely inspects both the thing's primary map and held map if applicable.
    /// </summary>
    /// <param name="thing">The thing whose designation should be removed.</param>
    /// <param name="map">The map context, or <c>null</c> to automatically check <see cref="Thing.Map"/> and <see cref="Thing.MapHeld"/>.</param>
    void TryRemoveDesignation(Thing thing, Map? map = null);

    /// <summary>
    /// Determines whether any capture designations exist on the given map.
    /// </summary>
    /// <param name="map">The map to search.</param>
    /// <returns><c>true</c> if at least one spawned capture designation exists; otherwise, <c>false</c>.</returns>
    bool AnySpawnedDesignation(Map map);

    /// <summary>
    /// Retrieves all things that currently possess an active capture designation on the map.
    /// </summary>
    /// <param name="map">The map to inspect.</param>
    /// <returns>An enumerable sequence of designated things.</returns>
    IEnumerable<Thing> GetDesignatedThings(Map map);
}
