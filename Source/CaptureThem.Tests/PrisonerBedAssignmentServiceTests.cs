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
}
