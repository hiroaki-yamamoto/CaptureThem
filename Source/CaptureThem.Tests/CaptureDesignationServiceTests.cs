using System;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureDesignationService"/>.
/// Validates null handling and boundary conditions.
/// </summary>
[TestClass]
public class CaptureDesignationServiceTests
{
    [TestMethod]
    public void HasDesignation_ReturnsFalse_WhenThingIsNull()
    {
        var service = new CaptureDesignationService();

        Assert.IsFalse(service.HasDesignation(null!, null));
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
    public void Designate_ThrowsArgumentNullException_WhenArgumentsAreNull()
    {
        var service = new CaptureDesignationService();

        Assert.ThrowsException<ArgumentNullException>(() => service.Designate(null!, null!));
    }

    [TestMethod]
    public void TryRemoveDesignation_HandlesNullSafely()
    {
        var service = new CaptureDesignationService();

        // Should not throw
        service.TryRemoveDesignation(null!, null);
    }
}
