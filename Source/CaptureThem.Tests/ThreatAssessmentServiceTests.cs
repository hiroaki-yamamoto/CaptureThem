using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="ThreatAssessmentService"/>.
/// Validates boundary and safety checks when evaluating danger.
/// </summary>
[TestClass]
public class ThreatAssessmentServiceTests
{
    [TestMethod]
    public void IsDangerNear_ReturnsFalse_WhenTargetOrCapturerIsNull()
    {
        var service = new ThreatAssessmentService();
        var pawn = new Pawn();

        Assert.IsFalse(service.IsDangerNear(null!, null!, 40f));
        Assert.IsFalse(service.IsDangerNear(pawn, null!, 40f));
        Assert.IsFalse(service.IsDangerNear(null!, pawn, 40f));
    }

    [TestMethod]
    public void IsDangerNear_ReturnsFalse_WhenTargetIsNotSpawned()
    {
        var service = new ThreatAssessmentService();
        var capturer = new Pawn();
        var target = new Pawn(); // Spawned is false by default

        Assert.IsFalse(service.IsDangerNear(capturer, target, 40f));
    }

    [TestMethod]
    public void IsDangerNear_ReturnsFalse_WhenTargetMapIsNull()
    {
        var service = new ThreatAssessmentService();
        var capturer = new Pawn();
        var target = new Pawn();
        // target.Map is null

        Assert.IsFalse(service.IsDangerNear(capturer, target, 40f));
    }
}
