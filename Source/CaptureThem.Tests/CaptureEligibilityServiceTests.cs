using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureEligibilityService"/>.
/// Validates input evaluation and null handling.
/// </summary>
[TestClass]
public class CaptureEligibilityServiceTests
{
    private readonly Mock<ICaptureDesignationService> mockDesignation = new();

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenThingIsNull()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        var report = service.CanDesignateThing(null!, null!);

        Assert.IsFalse(report.Accepted);
    }

    [TestMethod]
    public void CanDesignateCell_ReturnsFalse_WhenMapIsNull()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        var report = service.CanDesignateCell(default, null!);

        Assert.IsFalse(report.Accepted);
    }

    [TestMethod]
    public void IsEligibleForCaptureWork_ReturnsFalse_WhenCapturerOrTargetIsNull()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        Assert.IsFalse(service.IsEligibleForCaptureWork(null!, null!, false));
    }
}
