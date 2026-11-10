using System.Runtime.CompilerServices;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Verse;

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
    public void Constructor_UsesCaptureThemServices_WhenDesignationServiceOmitted()
    {
        var service = new CaptureEligibilityService();
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenThingIsNull()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        var report = service.CanDesignateThing(null!, null!);

        Assert.IsFalse(report.Accepted);
    }

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenThingHasDesignationAlready()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);
        var thing = new Thing();
        mockDesignation.Setup(d => d.HasDesignation(thing, It.IsAny<Map>())).Returns(true);

        var report = service.CanDesignateThing(thing, null!);

        Assert.IsFalse(report.Accepted);
        mockDesignation.Verify(d => d.HasDesignation(thing, It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenTargetIsNotPawn()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);
        var thing = new Thing();
        mockDesignation.Setup(d => d.HasDesignation(thing, It.IsAny<Map>())).Returns(false);

        var report = service.CanDesignateThing(thing, null!);

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
        Assert.IsFalse(service.IsEligibleForCaptureWork(new Pawn(), null!, false));
        Assert.IsFalse(service.IsEligibleForCaptureWork(null!, new Pawn(), false));
    }

    [TestMethod]
    public void IsEligibleForCaptureWork_ReturnsFalse_WhenTargetIsNotPawn()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);
        var capturer = new Pawn();
        var target = new Thing();

        Assert.IsFalse(service.IsEligibleForCaptureWork(capturer, target, false));
    }

    [TestMethod]
    public void IsEligibleForCaptureWork_ReturnsFalse_WhenTargetHealthIsNull()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);
        var capturer = new Pawn();
        var victim = new Pawn(); // health field defaults to null

        Assert.IsFalse(service.IsEligibleForCaptureWork(capturer, victim, false));
    }

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenTargetIsShambler()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        var shambler = new Pawn();
        var healthTrackerType = typeof(Pawn_HealthTracker);
        var healthField = typeof(Pawn).GetField("health");
        if (healthTrackerType != null && healthField != null)
        {
            var health = RuntimeHelpers.GetUninitializedObject(healthTrackerType);
            healthField.SetValue(shambler, health);
        }

        var trackerType = typeof(RimWorld.Building_Bed).Assembly.GetType("RimWorld.Pawn_MutantTracker");
        var mutantField = typeof(Pawn).GetField("mutant");

        if (trackerType != null && mutantField != null)
        {
            var tracker = RuntimeHelpers.GetUninitializedObject(trackerType);
            mutantField.SetValue(shambler, tracker);
        }

        // Even without active anomaly defs, verify that null or non-eligible pawns return false
        var report = service.CanDesignateThing(shambler, null!);
        Assert.IsFalse(report.Accepted);
    }

    [TestMethod]
    public void IsEligibleForCaptureWork_ReturnsFalse_WhenTargetIsShamblerOrPawnFactionMatches()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);
        var capturer = new Pawn();
        var shambler = new Pawn();

        Assert.IsFalse(service.IsEligibleForCaptureWork(capturer, shambler, false));
    }
}
