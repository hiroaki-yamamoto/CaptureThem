using System;
using System.Collections.Generic;
using System.Linq;
using CaptureThem.Interfaces;
using Verse;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for managing capture designations on pawns and other things.
/// Encapsulates direct interaction with RimWorld's <see cref="DesignationManager"/>.
/// </summary>
public class CaptureDesignationService : ICaptureDesignationService
{
    /// <inheritdoc />
    public DesignationDef DesignationDef => CaptureThemDefOf.CaptureThemCapture;

    /// <inheritdoc />
    public bool HasDesignation(Thing thing, Map? map = null)
    {
        if (thing == null)
        {
            return false;
        }

        var targetMap = map ?? thing.Map ?? (thing as Pawn)?.MapHeld;
        return targetMap?.designationManager?.DesignationOn(thing, DesignationDef) != null;
    }

    /// <inheritdoc />
    public void Designate(Thing thing, Map map)
    {
        ArgumentNullException.ThrowIfNull(thing);
        ArgumentNullException.ThrowIfNull(map);

        map.designationManager.RemoveAllDesignationsOn(thing);
        map.designationManager.AddDesignation(new Designation(thing, DesignationDef));
    }

    /// <inheritdoc />
    public void TryRemoveDesignation(Thing thing, Map? map = null)
    {
        if (thing == null)
        {
            return;
        }

        var targetMap = map ?? thing.Map;
        if (targetMap != null)
        {
            targetMap.designationManager.TryRemoveDesignationOn(thing, DesignationDef);
            return;
        }

        if (thing is Pawn pawn && pawn.MapHeld != null)
        {
            pawn.MapHeld.designationManager.TryRemoveDesignationOn(thing, DesignationDef);
        }
    }

    /// <inheritdoc />
    public bool AnySpawnedDesignation(Map map)
    {
        if (map?.designationManager == null)
        {
            return false;
        }

        return map.designationManager.AnySpawnedDesignationOfDef(DesignationDef);
    }

    /// <inheritdoc />
    public IEnumerable<Thing> GetDesignatedThings(Map map)
    {
        if (map?.designationManager == null)
        {
            return Enumerable.Empty<Thing>();
        }

        return map.designationManager.SpawnedDesignationsOfDef(DesignationDef)
            .Select(designation => designation.target.Thing)
            .Where(thing => thing != null);
    }
}
