using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CaptureThem.Tests;

/// <summary>
/// Verifies the dependency injection and lifecycle behavior of <see cref="CaptureThemServices"/>.
/// Ensures that services adhere to the Dependency Inversion Principle and can be substituted.
/// </summary>
[TestClass]
public class ServicesTests
{
    [TestCleanup]
    public void Cleanup()
    {
        CaptureThemServices.ResetToDefaults();
    }

    [TestMethod]
    public void Services_ReturnDefaultInstances_WhenNotOverridden()
    {
        CaptureThemServices.ResetToDefaults();

        Assert.IsNotNull(CaptureThemServices.DesignationService);
        Assert.IsNotNull(CaptureThemServices.EligibilityService);
        Assert.IsNotNull(CaptureThemServices.ThreatAssessmentService);
        Assert.IsNotNull(CaptureThemServices.BedAssignmentService);
        Assert.IsNotNull(CaptureThemServices.NotificationService);
        Assert.IsNotNull(CaptureThemServices.JobFactory);
        Assert.IsNotNull(CaptureThemServices.SettingsService);
        Assert.IsNotNull(CaptureThemServices.AutoCaptureService);
    }

    [TestMethod]
    public void Services_CanBeInjectedAndReset()
    {
        var mockDesignation = new Mock<ICaptureDesignationService>();
        var mockEligibility = new Mock<ICaptureEligibilityService>();
        var mockThreat = new Mock<IThreatAssessmentService>();
        var mockBed = new Mock<IPrisonerBedAssignmentService>();
        var mockNotification = new Mock<ICaptureNotificationService>();
        var mockJob = new Mock<ICaptureJobFactory>();
        var mockSettings = new Mock<ICaptureSettingsService>();
        var mockAutoCapture = new Mock<IAutoCaptureService>();

        CaptureThemServices.DesignationService = mockDesignation.Object;
        CaptureThemServices.EligibilityService = mockEligibility.Object;
        CaptureThemServices.ThreatAssessmentService = mockThreat.Object;
        CaptureThemServices.BedAssignmentService = mockBed.Object;
        CaptureThemServices.NotificationService = mockNotification.Object;
        CaptureThemServices.JobFactory = mockJob.Object;
        CaptureThemServices.SettingsService = mockSettings.Object;
        CaptureThemServices.AutoCaptureService = mockAutoCapture.Object;

        Assert.AreSame(mockDesignation.Object, CaptureThemServices.DesignationService);
        Assert.AreSame(mockEligibility.Object, CaptureThemServices.EligibilityService);
        Assert.AreSame(mockThreat.Object, CaptureThemServices.ThreatAssessmentService);
        Assert.AreSame(mockBed.Object, CaptureThemServices.BedAssignmentService);
        Assert.AreSame(mockNotification.Object, CaptureThemServices.NotificationService);
        Assert.AreSame(mockJob.Object, CaptureThemServices.JobFactory);
        Assert.AreSame(mockSettings.Object, CaptureThemServices.SettingsService);
        Assert.AreSame(mockAutoCapture.Object, CaptureThemServices.AutoCaptureService);

        CaptureThemServices.ResetToDefaults();

        Assert.AreNotSame(mockDesignation.Object, CaptureThemServices.DesignationService);
        Assert.AreNotSame(mockEligibility.Object, CaptureThemServices.EligibilityService);
        Assert.AreNotSame(mockThreat.Object, CaptureThemServices.ThreatAssessmentService);
        Assert.AreNotSame(mockBed.Object, CaptureThemServices.BedAssignmentService);
        Assert.AreNotSame(mockNotification.Object, CaptureThemServices.NotificationService);
        Assert.AreNotSame(mockJob.Object, CaptureThemServices.JobFactory);
        Assert.AreNotSame(mockSettings.Object, CaptureThemServices.SettingsService);
        Assert.AreNotSame(mockAutoCapture.Object, CaptureThemServices.AutoCaptureService);
    }
}
