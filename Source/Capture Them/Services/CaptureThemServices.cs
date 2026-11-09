using System;
using CaptureThem.Interfaces;

namespace CaptureThem.Services;

/// <summary>
/// Provides a centralized registry and service locator for Capture Them services.
/// Supports Dependency Inversion by enabling services to be replaced or mocked during testing or extension.
/// </summary>
public static class CaptureThemServices
{
    private static ICaptureDesignationService? designationService;
    private static ICaptureEligibilityService? eligibilityService;
    private static IThreatAssessmentService? threatAssessmentService;
    private static IPrisonerBedAssignmentService? bedAssignmentService;
    private static ICaptureNotificationService? notificationService;
    private static ICaptureJobFactory? jobFactory;

    /// <summary>
    /// Gets or sets the designation service instance.
    /// </summary>
    public static ICaptureDesignationService DesignationService
    {
        get => designationService ??= new CaptureDesignationService();
        set => designationService = value;
    }

    /// <summary>
    /// Gets or sets the capture eligibility service instance.
    /// </summary>
    public static ICaptureEligibilityService EligibilityService
    {
        get => eligibilityService ??= new CaptureEligibilityService(DesignationService);
        set => eligibilityService = value;
    }

    /// <summary>
    /// Gets or sets the threat assessment service instance.
    /// </summary>
    public static IThreatAssessmentService ThreatAssessmentService
    {
        get => threatAssessmentService ??= new ThreatAssessmentService();
        set => threatAssessmentService = value;
    }

    /// <summary>
    /// Gets or sets the prisoner bed assignment service instance.
    /// </summary>
    public static IPrisonerBedAssignmentService BedAssignmentService
    {
        get => bedAssignmentService ??= new PrisonerBedAssignmentService();
        set => bedAssignmentService = value;
    }

    /// <summary>
    /// Gets or sets the capture notification service instance.
    /// </summary>
    public static ICaptureNotificationService NotificationService
    {
        get => notificationService ??= new CaptureNotificationService();
        set => notificationService = value;
    }

    /// <summary>
    /// Gets or sets the capture job factory instance.
    /// </summary>
    public static ICaptureJobFactory JobFactory
    {
        get => jobFactory ??= new CaptureJobFactory();
        set => jobFactory = value;
    }

    /// <summary>
    /// Resets all service instances to their default implementations.
    /// Particularly useful in test teardown routines.
    /// </summary>
    public static void ResetToDefaults()
    {
        designationService = null;
        eligibilityService = null;
        threatAssessmentService = null;
        bedAssignmentService = null;
        notificationService = null;
        jobFactory = null;
    }
}
