using System;
using System.Collections.Generic;
using CaptureThem.Interfaces;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for finding and reserving prisoner beds for capture candidates.
/// Encapsulates interaction with RimWorld's <see cref="RestUtility"/> and manages a negative cache
/// with tick-based throttling to eliminate wasted search cycles when beds are full.
/// </summary>
public class PrisonerBedAssignmentService : IPrisonerBedAssignmentService
{
    /// <summary>
    /// Default duration in game ticks to throttle subsequent bed searches on a map where no prisoner beds were found.
    /// 250 ticks corresponds to roughly 4.16 seconds in 1x speed.
    /// </summary>
    public const int DefaultThrottleTicks = 250;

    private readonly Func<int> tickProvider;
    private readonly int throttleTicks;
    private readonly Dictionary<int, int> noBedUntilTickByMapId = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PrisonerBedAssignmentService"/> class.
    /// </summary>
    /// <param name="tickProvider">Optional tick provider function for retrieving current game ticks. Defaults to <see cref="Find.TickManager"/>.</param>
    /// <param name="throttleTicks">Duration in ticks before throttled searches expire.</param>
    public PrisonerBedAssignmentService(Func<int>? tickProvider = null, int throttleTicks = DefaultThrottleTicks)
    {
        this.tickProvider = tickProvider ?? GetCurrentTicks;
        this.throttleTicks = throttleTicks;
    }

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
            HandleBedSearchResult(victim, capturer, primaryBed);
            return primaryBed;
        }

        var fallbackBed = RestUtility.FindBedFor(victim, capturer, false, true, GuestStatus.Prisoner);
        HandleBedSearchResult(victim, capturer, fallbackBed);
        return fallbackBed;
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

    /// <inheritdoc />
    public bool HasKnownAvailableBed(Map? map)
    {
        if (map == null)
        {
            return true;
        }

        lock (noBedUntilTickByMapId)
        {
            if (noBedUntilTickByMapId.TryGetValue(map.uniqueID, out var untilTick))
            {
                var currentTick = tickProvider();
                if (currentTick < untilTick)
                {
                    return false;
                }

                noBedUntilTickByMapId.Remove(map.uniqueID);
            }
        }

        return true;
    }

    /// <inheritdoc />
    public void InvalidateBedCache(Map? map = null)
    {
        lock (noBedUntilTickByMapId)
        {
            if (map != null)
            {
                noBedUntilTickByMapId.Remove(map.uniqueID);
            }
            else
            {
                noBedUntilTickByMapId.Clear();
            }
        }
    }

    /// <summary>
    /// Explicitly registers that no beds were found on the specified map, initiating the throttle timer.
    /// Internal for unit test validation and potential caller hooks.
    /// </summary>
    /// <param name="map">The map where no prisoner beds were found.</param>
    internal void RegisterNoBedFound(Map map)
    {
        if (map == null)
        {
            return;
        }

        lock (noBedUntilTickByMapId)
        {
            var currentTick = tickProvider();
            noBedUntilTickByMapId[map.uniqueID] = currentTick + throttleTicks;
        }
    }

    private void HandleBedSearchResult(Pawn victim, Pawn capturer, Building_Bed? bed)
    {
        var targetMap = victim.Map ?? capturer.Map;
        if (targetMap == null)
        {
            return;
        }

        if (bed == null)
        {
            RegisterNoBedFound(targetMap);
        }
        else
        {
            lock (noBedUntilTickByMapId)
            {
                noBedUntilTickByMapId.Remove(targetMap.uniqueID);
            }
        }
    }

    private static int GetCurrentTicks()
    {
        try
        {
            return Current.ProgramState == ProgramState.Playing ? Find.TickManager.TicksGame : 0;
        }
        catch
        {
            return 0;
        }
    }
}
