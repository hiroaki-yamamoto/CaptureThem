using System;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using RimWorld;
using UnityEngine;
using Verse;

namespace CaptureThem;

/// <summary>
/// Designator tool that allows players to mark downed foreign pawns for capture.
/// Adheres to SOLID principles by delegating validation, notifications, and designation operations to dedicated services.
/// </summary>
public class Designator_CapturePawn : Designator
{
    private readonly ICaptureDesignationService designationService;
    private readonly ICaptureEligibilityService eligibilityService;
    private readonly ICaptureNotificationService notificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Designator_CapturePawn"/> class using global default services.
    /// </summary>
    public Designator_CapturePawn()
        : this(
            CaptureThemServices.DesignationService,
            CaptureThemServices.EligibilityService,
            CaptureThemServices.NotificationService)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Designator_CapturePawn"/> class with custom services (useful for dependency injection and testing).
    /// </summary>
    /// <param name="designationService">The designation management service.</param>
    /// <param name="eligibilityService">The eligibility evaluation service.</param>
    /// <param name="notificationService">The notification and messaging service.</param>
    public Designator_CapturePawn(
        ICaptureDesignationService designationService,
        ICaptureEligibilityService eligibilityService,
        ICaptureNotificationService notificationService)
    {
        this.designationService = designationService ?? throw new ArgumentNullException(nameof(designationService));
        this.eligibilityService = eligibilityService ?? throw new ArgumentNullException(nameof(eligibilityService));
        this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

        defaultLabel = "DesignatorCapturePawn".Translate();
        defaultDesc = "DesignatorCapturePawnDesc".Translate();
        icon = ContentFinder<Texture2D>.Get("CapturePawnGizmo");
        useMouseIcon = true;
        soundSucceeded = SoundDefOf.Designate_Haul;
        hotKey = KeyBindingDefOf.Misc1;
    }

    /// <inheritdoc />
    protected override DesignationDef Designation => designationService.DesignationDef;

    /// <inheritdoc />
    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Orders;

    /// <inheritdoc />
    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        return eligibilityService.CanDesignateCell(loc, Map);
    }

    /// <inheritdoc />
    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        return eligibilityService.CanDesignateThing(t, Map);
    }

    /// <inheritdoc />
    public override void DesignateSingleCell(IntVec3 c)
    {
        var thingList = c.GetThingList(Map);
        for (var i = 0; i < thingList.Count; i++)
        {
            if (thingList[i] is Pawn pawn)
            {
                DesignateThing(pawn);
            }
        }
    }

    /// <inheritdoc />
    public override void DesignateThing(Thing t)
    {
        if (t is Pawn pawn)
        {
            notificationService.WarnIfCapturingWillAngerFaction(pawn);
        }

        designationService.Designate(t, Map);
    }
}
