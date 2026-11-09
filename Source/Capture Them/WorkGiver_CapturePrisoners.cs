using System;
using System.Collections.Generic;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem;

/// <summary>
/// WorkGiver responsible for scanning, filtering, and assigning capture jobs to eligible colonists.
/// Follows SOLID principles by orchestrating specialized services for eligibility, threat assessment, bed assignment, and job creation.
/// </summary>
public class WorkGiver_CapturePrisoners : WorkGiver_RescueDowned
{
    private const float ThreatDetectionRadius = 40f;

    private readonly ICaptureDesignationService designationService;
    private readonly ICaptureEligibilityService eligibilityService;
    private readonly IThreatAssessmentService threatAssessmentService;
    private readonly IPrisonerBedAssignmentService bedAssignmentService;
    private readonly ICaptureNotificationService notificationService;
    private readonly ICaptureJobFactory jobFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkGiver_CapturePrisoners"/> class using global default services.
    /// Required for RimWorld reflection instantiation via XML defs.
    /// </summary>
    public WorkGiver_CapturePrisoners()
        : this(
            CaptureThemServices.DesignationService,
            CaptureThemServices.EligibilityService,
            CaptureThemServices.ThreatAssessmentService,
            CaptureThemServices.BedAssignmentService,
            CaptureThemServices.NotificationService,
            CaptureThemServices.JobFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkGiver_CapturePrisoners"/> class with custom services (useful for dependency injection and testing).
    /// </summary>
    /// <param name="designationService">The designation service.</param>
    /// <param name="eligibilityService">The eligibility service.</param>
    /// <param name="threatAssessmentService">The threat assessment service.</param>
    /// <param name="bedAssignmentService">The bed assignment service.</param>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="jobFactory">The job factory.</param>
    public WorkGiver_CapturePrisoners(
        ICaptureDesignationService designationService,
        ICaptureEligibilityService eligibilityService,
        IThreatAssessmentService threatAssessmentService,
        IPrisonerBedAssignmentService bedAssignmentService,
        ICaptureNotificationService notificationService,
        ICaptureJobFactory jobFactory)
    {
        this.designationService = designationService ?? throw new ArgumentNullException(nameof(designationService));
        this.eligibilityService = eligibilityService ?? throw new ArgumentNullException(nameof(eligibilityService));
        this.threatAssessmentService = threatAssessmentService ?? throw new ArgumentNullException(nameof(threatAssessmentService));
        this.bedAssignmentService = bedAssignmentService ?? throw new ArgumentNullException(nameof(bedAssignmentService));
        this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        this.jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
    }

    /// <inheritdoc />
    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !designationService.AnySpawnedDesignation(pawn.Map);
    }

    /// <inheritdoc />
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return designationService.GetDesignatedThings(pawn.Map);
    }

    /// <inheritdoc />
    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!eligibilityService.IsEligibleForCaptureWork(pawn, t, forced))
        {
            return false;
        }

        var victim = (Pawn)t;
        if (threatAssessmentService.IsDangerNear(pawn, victim, ThreatDetectionRadius))
        {
            return false;
        }

        var bed = bedAssignmentService.FindBedFor(victim, pawn, allowFallback: true);
        if (bed != null)
        {
            return bedAssignmentService.CanReserveBed(pawn, bed, forced);
        }

        notificationService.NotifyNoPrisonerBedAvailable(victim);
        return false;
    }

    /// <inheritdoc />
    public override Job? JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Pawn victim)
        {
            return null;
        }

        var destinationBed = bedAssignmentService.FindBedFor(victim, pawn, allowFallback: false);
        return jobFactory.CreateCaptureJob(pawn, victim, destinationBed!);
    }
}
