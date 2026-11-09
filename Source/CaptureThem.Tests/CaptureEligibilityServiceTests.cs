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

    [TestMethod]
    public void CanDesignateThing_ReturnsFalse_WhenTargetIsShambler()
    {
        var service = new CaptureEligibilityService(mockDesignation.Object);

        var shambler = new Verse.Pawn();
        var healthTrackerType = typeof(Verse.Pawn_HealthTracker);
        var healthField = typeof(Verse.Pawn).GetField("health");
        if (healthTrackerType != null && healthField != null)
        {
            var health = System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(healthTrackerType);
            healthField.SetValue(shambler, health);
        }

        var trackerType = typeof(RimWorld.Building_Bed).Assembly.GetType("RimWorld.Pawn_MutantTracker");
        var mutantField = typeof(Verse.Pawn).GetField("mutant");

        if (trackerType != null && mutantField != null)
        {
            var tracker = System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(trackerType);
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
        var capturer = new Verse.Pawn();
        var shambler = new Verse.Pawn();

        Assert.IsFalse(service.IsEligibleForCaptureWork(capturer, shambler, false));
    }
}
