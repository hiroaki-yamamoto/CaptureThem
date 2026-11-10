using System.Reflection;
using CaptureThem.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RimWorld;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="Designator_CapturePawn"/>.
/// Validates constructor argument assertions and delegation behavior.
/// </summary>
[TestClass]
public class DesignatorCapturePawnTests
{
    private readonly Mock<ICaptureDesignationService> mockDesignationService = new();
    private readonly Mock<ICaptureEligibilityService> mockEligibilityService = new();
    private readonly Mock<ICaptureNotificationService> mockNotificationService = new();

    [TestMethod]
    public void Constructor_ThrowsArgumentNullException_WhenAnyDependencyIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new Designator_CapturePawn(
            null!, mockEligibilityService.Object, mockNotificationService.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new Designator_CapturePawn(
            mockDesignationService.Object, null!, mockNotificationService.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new Designator_CapturePawn(
            mockDesignationService.Object, mockEligibilityService.Object, null!));
    }

    [TestMethod]
    public void DesignateThing_DelegatesToDesignationService_AndDoesNotWarnForNonPawns()
    {
        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        var thing = new Thing();
        designator.DesignateThing(thing);

        mockNotificationService.Verify(n => n.WarnIfCapturingWillAngerFaction(It.IsAny<Pawn>()), Times.Never);
        mockDesignationService.Verify(d => d.Designate(thing, It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void DesignateThing_WarnsAndDesignates_WhenTargetIsPawn()
    {
        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        var pawn = new Pawn();
        designator.DesignateThing(pawn);

        mockNotificationService.Verify(n => n.WarnIfCapturingWillAngerFaction(pawn), Times.Once);
        mockDesignationService.Verify(d => d.Designate(pawn, It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void CanDesignateThing_DelegatesToEligibilityService()
    {
        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        var thing = new Thing();
        mockEligibilityService.Setup(e => e.CanDesignateThing(thing, It.IsAny<Map>())).Returns(false);

        var report = designator.CanDesignateThing(thing);

        Assert.IsFalse(report.Accepted);
        mockEligibilityService.Verify(e => e.CanDesignateThing(thing, It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void CanDesignateCell_DelegatesToEligibilityService()
    {
        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        var cell = new IntVec3(5, 0, 5);
        mockEligibilityService.Setup(e => e.CanDesignateCell(cell, It.IsAny<Map>())).Returns(false);

        var report = designator.CanDesignateCell(cell);

        Assert.IsFalse(report.Accepted);
        mockEligibilityService.Verify(e => e.CanDesignateCell(cell, It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void Designation_ReturnsDesignationDefFromService()
    {
        var def = new DesignationDef { defName = "TestCaptureDef" };
        mockDesignationService.SetupGet(d => d.DesignationDef).Returns(def);

        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        var prop = typeof(Designator_CapturePawn).GetProperty("Designation", BindingFlags.Instance | BindingFlags.NonPublic);
        var result = prop?.GetValue(designator);

        Assert.AreSame(def, result);
    }

    [TestMethod]
    public void DrawStyleCategory_ReturnsOrders()
    {
        var designator = new Designator_CapturePawn(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockNotificationService.Object);

        Assert.AreEqual(DrawStyleCategoryDefOf.Orders, designator.DrawStyleCategory);
    }
}
