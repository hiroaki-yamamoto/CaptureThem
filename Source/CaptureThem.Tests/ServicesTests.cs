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
    }

    [TestMethod]
    public void Services_CanBeInjectedAndReset()
    {
        var mockDesignation = new Mock<ICaptureDesignationService>();
        var mockEligibility = new Mock<ICaptureEligibilityService>();

        CaptureThemServices.DesignationService = mockDesignation.Object;
        CaptureThemServices.EligibilityService = mockEligibility.Object;

        Assert.AreSame(mockDesignation.Object, CaptureThemServices.DesignationService);
        Assert.AreSame(mockEligibility.Object, CaptureThemServices.EligibilityService);

        CaptureThemServices.ResetToDefaults();

        Assert.AreNotSame(mockDesignation.Object, CaptureThemServices.DesignationService);
        Assert.AreNotSame(mockEligibility.Object, CaptureThemServices.EligibilityService);
    }
}
