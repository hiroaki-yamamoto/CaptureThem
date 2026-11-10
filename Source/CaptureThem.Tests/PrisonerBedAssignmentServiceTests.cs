using System.Runtime.CompilerServices;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RimWorld;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="PrisonerBedAssignmentService"/>.
/// Validates null argument handling for bed search and reservation.
/// </summary>
[TestClass]
public class PrisonerBedAssignmentServiceTests
{
    [TestMethod]
    public void FindBedFor_ReturnsNull_WhenVictimOrCapturerIsNull()
    {
        var service = new PrisonerBedAssignmentService();
        var pawn = new Pawn();

        Assert.IsNull(service.FindBedFor(null!, null!));
        Assert.IsNull(service.FindBedFor(pawn, null!));
        Assert.IsNull(service.FindBedFor(null!, pawn));
    }

    [TestMethod]
    public void CanReserveBed_ReturnsFalse_WhenCapturerOrBedIsNull()
    {
        var service = new PrisonerBedAssignmentService();
        var pawn = new Pawn();
        var bed = (Building_Bed)RuntimeHelpers.GetUninitializedObject(typeof(Building_Bed));

        Assert.IsFalse(service.CanReserveBed(null!, null!, false));
        Assert.IsFalse(service.CanReserveBed(pawn, null!, false));
        Assert.IsFalse(service.CanReserveBed(null!, bed, false));
    }

    [TestMethod]
    public void HasKnownAvailableBed_ReturnsTrue_WhenMapIsNull()
    {
        var service = new PrisonerBedAssignmentService();
        Assert.IsTrue(service.HasKnownAvailableBed(null));
    }

    [TestMethod]
    public void HasKnownAvailableBed_ReturnsTrue_WhenNoCacheRegistered()
    {
        var service = new PrisonerBedAssignmentService();
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map.uniqueID = 1;

        Assert.IsTrue(service.HasKnownAvailableBed(map));
    }

    [TestMethod]
    public void HasKnownAvailableBed_ReturnsFalse_WhenWithinThrottlePeriod()
    {
        var currentTick = 1000;
        var service = new PrisonerBedAssignmentService(() => currentTick, throttleTicks: 250);
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map.uniqueID = 1;

        service.RegisterNoBedFound(map);

        currentTick = 1100; // 1000 + 100 < 1000 + 250 -> still within throttle period
        Assert.IsFalse(service.HasKnownAvailableBed(map));
    }

    [TestMethod]
    public void HasKnownAvailableBed_ReturnsTrue_AndEvictsEntry_WhenThrottleExpires()
    {
        var currentTick = 1000;
        var service = new PrisonerBedAssignmentService(() => currentTick, throttleTicks: 250);
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map.uniqueID = 1;

        service.RegisterNoBedFound(map);

        currentTick = 1251; // 1000 + 251 > 1000 + 250 -> expired
        Assert.IsTrue(service.HasKnownAvailableBed(map));

        // Subsequent check should also return true because expired entry is evicted
        Assert.IsTrue(service.HasKnownAvailableBed(map));
    }

    [TestMethod]
    public void InvalidateBedCache_ClearsCacheForSpecificMap()
    {
        var currentTick = 1000;
        var service = new PrisonerBedAssignmentService(() => currentTick, throttleTicks: 250);
        var map1 = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map1.uniqueID = 1;
        var map2 = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map2.uniqueID = 2;

        service.RegisterNoBedFound(map1);
        service.RegisterNoBedFound(map2);

        service.InvalidateBedCache(map1);

        Assert.IsTrue(service.HasKnownAvailableBed(map1));
        Assert.IsFalse(service.HasKnownAvailableBed(map2));
    }

    [TestMethod]
    public void InvalidateBedCache_ClearsCacheForAllMaps_WhenMapIsNull()
    {
        var currentTick = 1000;
        var service = new PrisonerBedAssignmentService(() => currentTick, throttleTicks: 250);
        var map1 = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map1.uniqueID = 1;
        var map2 = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        map2.uniqueID = 2;

        service.RegisterNoBedFound(map1);
        service.RegisterNoBedFound(map2);

        service.InvalidateBedCache(null);

        Assert.IsTrue(service.HasKnownAvailableBed(map1));
        Assert.IsTrue(service.HasKnownAvailableBed(map2));
    }
}
