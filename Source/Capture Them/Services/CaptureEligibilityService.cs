using System;
using CaptureThem.Interfaces;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for evaluating capture eligibility for cells, things, and AI work.
/// Decouples business rules from the user interface and AI worker classes.
/// </summary>
public class CaptureEligibilityService : ICaptureEligibilityService
{
    private readonly Func<ICaptureDesignationService> designationServiceAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureEligibilityService"/> class.
    /// </summary>
    /// <param name="designationService">The designation service, or <c>null</c> to resolve via <see cref="CaptureThemServices"/>.</param>
    public CaptureEligibilityService(ICaptureDesignationService? designationService = null)
    {
        designationServiceAccessor = designationService != null
            ? () => designationService
            : () => CaptureThemServices.DesignationService;
    }

    private ICaptureDesignationService DesignationService => designationServiceAccessor();

    /// <inheritdoc />
    public AcceptanceReport CanDesignateCell(IntVec3 loc, Map map)
    {
        if (map == null || !loc.InBounds(map) || loc.Fogged(map))
        {
            return false;
        }

        var firstPawn = loc.GetFirstPawn(map);
        if (firstPawn == null || !firstPawn.RaceProps.Humanlike)
        {
            return "MessageMustDesignateDownedForeignPawn".Translate();
        }

        var result = CanDesignateThing(firstPawn, map);
        if (!result.Accepted)
        {
            return result;
        }

        return true;
    }

    /// <inheritdoc />
    public AcceptanceReport CanDesignateThing(Thing thing, Map map)
    {
        if (thing == null)
        {
            return false;
        }

        if (DesignationService.HasDesignation(thing, map))
        {
            return false;
        }

        return thing is Pawn { Downed: true } pawn &&
               pawn.Faction != Faction.OfPlayer &&
               !pawn.InBed() &&
               !pawn.IsPrisonerOfColony &&
               pawn.RaceProps.Humanlike;
    }

    /// <inheritdoc />
    public bool IsEligibleForCaptureWork(Pawn capturer, Thing target, bool forced)
    {
        if (capturer == null || target is not Pawn { Downed: true } pawn)
        {
            return false;
        }

        if (pawn.Faction == capturer.Faction)
        {
            return false;
        }

        var map = target.Map ?? capturer.Map;
        if (!DesignationService.HasDesignation(target, map))
        {
            return false;
        }

        if (pawn.InBed() || !capturer.CanReserve(pawn, 1, -1, null, forced))
        {
            return false;
        }

        return true;
    }
}
