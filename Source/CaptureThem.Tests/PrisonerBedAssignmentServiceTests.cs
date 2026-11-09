using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        Assert.IsNull(service.FindBedFor(null!, null!));
    }

    [TestMethod]
    public void CanReserveBed_ReturnsFalse_WhenCapturerOrBedIsNull()
    {
        var service = new PrisonerBedAssignmentService();

        Assert.IsFalse(service.CanReserveBed(null!, null!, false));
    }
}
