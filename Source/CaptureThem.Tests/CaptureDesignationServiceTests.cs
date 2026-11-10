using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureDesignationService"/>.
/// Validates null handling and boundary conditions.
/// </summary>
[TestClass]
public class CaptureDesignationServiceTests
{
    [TestMethod]
    public void DesignationDef_ReturnsCaptureThemCaptureDef()
    {
        var service = new CaptureDesignationService();

        Assert.AreEqual(CaptureThemDefOf.CaptureThemCapture, service.DesignationDef);
    }

    [TestMethod]
    public void HasDesignation_ReturnsFalse_WhenThingIsNull()
    {
        var service = new CaptureDesignationService();

        Assert.IsFalse(service.HasDesignation(null!, null));
    }

    [TestMethod]
    public void HasDesignation_ReturnsFalse_WhenThingHasNoMapAndMapParamIsNull()
    {
        var service = new CaptureDesignationService();
        var thing = new Thing();

        Assert.IsFalse(service.HasDesignation(thing, null));
    }

    [TestMethod]
    public void AnySpawnedDesignation_ReturnsFalse_WhenMapIsNull()
    {
        var service = new CaptureDesignationService();

        Assert.IsFalse(service.AnySpawnedDesignation(null!));
    }

    [TestMethod]
    public void GetDesignatedThings_ReturnsEmpty_WhenMapIsNull()
    {
        var service = new CaptureDesignationService();

        var result = service.GetDesignatedThings(null!);

        Assert.IsNotNull(result);
        var enumerator = result.GetEnumerator();
        Assert.IsFalse(enumerator.MoveNext());
    }

    [TestMethod]
    public void Designate_ThrowsArgumentNullException_WhenThingIsNull()
    {
        var service = new CaptureDesignationService();
        var map = (Map)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Map));

        var ex = Assert.ThrowsException<ArgumentNullException>(() => service.Designate(null!, map));
        Assert.AreEqual("thing", ex.ParamName);
    }

    [TestMethod]
    public void Designate_ThrowsArgumentNullException_WhenMapIsNull()
    {
        var service = new CaptureDesignationService();
        var thing = new Thing();

        var ex = Assert.ThrowsException<ArgumentNullException>(() => service.Designate(thing, null!));
        Assert.AreEqual("map", ex.ParamName);
    }

    [TestMethod]
    public void TryRemoveDesignation_HandlesNullSafely()
    {
        var service = new CaptureDesignationService();

        // Should not throw
        service.TryRemoveDesignation(null!, null);
    }

    [TestMethod]
    public void TryRemoveDesignation_HandlesThingWithNullMapSafely()
    {
        var service = new CaptureDesignationService();
        var thing = new Thing();

        // Target map is null, pawn is not Pawn with MapHeld, should safely return without throwing
        service.TryRemoveDesignation(thing, null);
    }
}
