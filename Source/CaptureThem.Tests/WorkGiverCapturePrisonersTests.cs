using System.Runtime.CompilerServices;
using CaptureThem.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="WorkGiver_CapturePrisoners"/>.
/// Validates dependency injection, null argument checks, and service orchestration.
/// </summary>
[TestClass]
public class WorkGiverCapturePrisonersTests
{
    private readonly Mock<ICaptureDesignationService> mockDesignationService = new();
    private readonly Mock<ICaptureEligibilityService> mockEligibilityService = new();
    private readonly Mock<IThreatAssessmentService> mockThreatAssessmentService = new();
    private readonly Mock<IPrisonerBedAssignmentService> mockBedAssignmentService = new();
    private readonly Mock<ICaptureNotificationService> mockNotificationService = new();
    private readonly Mock<ICaptureJobFactory> mockJobFactory = new();

    private WorkGiver_CapturePrisoners CreateSut()
    {
        return new WorkGiver_CapturePrisoners(
            mockDesignationService.Object,
            mockEligibilityService.Object,
            mockThreatAssessmentService.Object,
            mockBedAssignmentService.Object,
            mockNotificationService.Object,
            mockJobFactory.Object);
    }

    [TestMethod]
    public void Constructor_ThrowsArgumentNullException_WhenAnyDependencyIsNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            null!, mockEligibilityService.Object, mockThreatAssessmentService.Object,
            mockBedAssignmentService.Object, mockNotificationService.Object, mockJobFactory.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            mockDesignationService.Object, null!, mockThreatAssessmentService.Object,
            mockBedAssignmentService.Object, mockNotificationService.Object, mockJobFactory.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            mockDesignationService.Object, mockEligibilityService.Object, null!,
            mockBedAssignmentService.Object, mockNotificationService.Object, mockJobFactory.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            mockDesignationService.Object, mockEligibilityService.Object, mockThreatAssessmentService.Object,
            null!, mockNotificationService.Object, mockJobFactory.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            mockDesignationService.Object, mockEligibilityService.Object, mockThreatAssessmentService.Object,
            mockBedAssignmentService.Object, null!, mockJobFactory.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new WorkGiver_CapturePrisoners(
            mockDesignationService.Object, mockEligibilityService.Object, mockThreatAssessmentService.Object,
            mockBedAssignmentService.Object, mockNotificationService.Object, null!));
    }

    [TestMethod]
    public void ShouldSkip_ReturnsTrue_WhenAnySpawnedDesignationIsFalse()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        mockDesignationService.Setup(d => d.AnySpawnedDesignation(It.IsAny<Map>())).Returns(false);

        var result = sut.ShouldSkip(pawn, false);

        Assert.IsTrue(result);
        mockDesignationService.Verify(d => d.AnySpawnedDesignation(It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void ShouldSkip_ReturnsFalse_WhenAnySpawnedDesignationIsTrue_AndBedKnownAvailable()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        mockDesignationService.Setup(d => d.AnySpawnedDesignation(It.IsAny<Map>())).Returns(true);
        mockBedAssignmentService.Setup(b => b.HasKnownAvailableBed(It.IsAny<Map>())).Returns(true);

        var result = sut.ShouldSkip(pawn, false);

        Assert.IsFalse(result);
        mockDesignationService.Verify(d => d.AnySpawnedDesignation(It.IsAny<Map>()), Times.Once);
        mockBedAssignmentService.Verify(b => b.HasKnownAvailableBed(It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void ShouldSkip_ReturnsTrue_WhenAnySpawnedDesignationIsTrue_ButBedKnownUnavailable_AndNotForced()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        mockDesignationService.Setup(d => d.AnySpawnedDesignation(It.IsAny<Map>())).Returns(true);
        mockBedAssignmentService.Setup(b => b.HasKnownAvailableBed(It.IsAny<Map>())).Returns(false);

        var result = sut.ShouldSkip(pawn, false);

        Assert.IsTrue(result);
        mockDesignationService.Verify(d => d.AnySpawnedDesignation(It.IsAny<Map>()), Times.Once);
        mockBedAssignmentService.Verify(b => b.HasKnownAvailableBed(It.IsAny<Map>()), Times.Once);
    }

    [TestMethod]
    public void ShouldSkip_ReturnsFalse_WhenBedKnownUnavailable_ButForced()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        mockDesignationService.Setup(d => d.AnySpawnedDesignation(It.IsAny<Map>())).Returns(true);
        mockBedAssignmentService.Setup(b => b.HasKnownAvailableBed(It.IsAny<Map>())).Returns(false);

        var result = sut.ShouldSkip(pawn, true);

        Assert.IsFalse(result);
        mockDesignationService.Verify(d => d.AnySpawnedDesignation(It.IsAny<Map>()), Times.Once);
        mockBedAssignmentService.Verify(b => b.HasKnownAvailableBed(It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void HasJobOnThing_ReturnsFalse_WhenTargetIsNotEligible()
    {
        var sut = CreateSut();
        mockEligibilityService
            .Setup(e => e.IsEligibleForCaptureWork(It.IsAny<Pawn>(), It.IsAny<Thing>(), It.IsAny<bool>()))
            .Returns(false);

        var result = sut.HasJobOnThing(null!, null!, false);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasJobOnThing_ReturnsFalse_WhenDangerNear()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        var victim = new Pawn();
        mockEligibilityService.Setup(e => e.IsEligibleForCaptureWork(pawn, victim, false)).Returns(true);
        mockThreatAssessmentService.Setup(t => t.IsDangerNear(pawn, victim, 40f)).Returns(true);

        var result = sut.HasJobOnThing(pawn, victim, false);

        Assert.IsFalse(result);
        mockBedAssignmentService.Verify(b => b.FindBedFor(It.IsAny<Pawn>(), It.IsAny<Pawn>(), It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public void HasJobOnThing_ReturnsFalse_AndNotifies_WhenNoBedAvailable()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        var victim = new Pawn();
        mockEligibilityService.Setup(e => e.IsEligibleForCaptureWork(pawn, victim, false)).Returns(true);
        mockThreatAssessmentService.Setup(t => t.IsDangerNear(pawn, victim, 40f)).Returns(false);
        mockBedAssignmentService.Setup(b => b.FindBedFor(victim, pawn, true)).Returns((Building_Bed?)null);

        var result = sut.HasJobOnThing(pawn, victim, false);

        Assert.IsFalse(result);
        mockNotificationService.Verify(n => n.NotifyNoPrisonerBedAvailable(victim), Times.Once);
    }

    [TestMethod]
    public void HasJobOnThing_ReturnsTrue_WhenBedFoundAndCanReserve()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        var victim = new Pawn();
        var bed = (Building_Bed)RuntimeHelpers.GetUninitializedObject(typeof(Building_Bed));
        mockEligibilityService.Setup(e => e.IsEligibleForCaptureWork(pawn, victim, false)).Returns(true);
        mockThreatAssessmentService.Setup(t => t.IsDangerNear(pawn, victim, 40f)).Returns(false);
        mockBedAssignmentService.Setup(b => b.FindBedFor(victim, pawn, true)).Returns(bed);
        mockBedAssignmentService.Setup(b => b.CanReserveBed(pawn, bed, false)).Returns(true);

        var result = sut.HasJobOnThing(pawn, victim, false);

        Assert.IsTrue(result);
        mockNotificationService.Verify(n => n.NotifyNoPrisonerBedAvailable(It.IsAny<Pawn>()), Times.Never);
    }

    [TestMethod]
    public void HasJobOnThing_ReturnsFalse_WhenBedFoundButCannotReserve()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        var victim = new Pawn();
        var bed = (Building_Bed)RuntimeHelpers.GetUninitializedObject(typeof(Building_Bed));
        mockEligibilityService.Setup(e => e.IsEligibleForCaptureWork(pawn, victim, false)).Returns(true);
        mockThreatAssessmentService.Setup(t => t.IsDangerNear(pawn, victim, 40f)).Returns(false);
        mockBedAssignmentService.Setup(b => b.FindBedFor(victim, pawn, true)).Returns(bed);
        mockBedAssignmentService.Setup(b => b.CanReserveBed(pawn, bed, false)).Returns(false);

        var result = sut.HasJobOnThing(pawn, victim, false);

        Assert.IsFalse(result);
        mockNotificationService.Verify(n => n.NotifyNoPrisonerBedAvailable(It.IsAny<Pawn>()), Times.Never);
    }

    [TestMethod]
    public void JobOnThing_ReturnsNull_WhenTargetIsNotPawn()
    {
        var sut = CreateSut();

        var result = sut.JobOnThing(null!, null!, false);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void JobOnThing_CreatesCaptureJob_WhenTargetIsPawn()
    {
        var sut = CreateSut();
        var pawn = new Pawn();
        var victim = new Pawn();
        var bed = (Building_Bed)RuntimeHelpers.GetUninitializedObject(typeof(Building_Bed));
        var expectedJob = new Job();

        mockBedAssignmentService.Setup(b => b.FindBedFor(victim, pawn, false)).Returns(bed);
        mockJobFactory.Setup(j => j.CreateCaptureJob(pawn, victim, bed)).Returns(expectedJob);

        var result = sut.JobOnThing(pawn, victim, false);

        Assert.AreSame(expectedJob, result);
        mockBedAssignmentService.Verify(b => b.FindBedFor(victim, pawn, false), Times.Once);
        mockJobFactory.Verify(j => j.CreateCaptureJob(pawn, victim, bed), Times.Once);
    }

    [TestMethod]
    public void PotentialWorkThingsGlobal_DelegatesToDesignationService()
    {
        var sut = CreateSut();
        var expectedThings = new List<Thing>();
        mockDesignationService
            .Setup(d => d.GetDesignatedThings(It.IsAny<Map>()))
            .Returns(expectedThings);

        var pawn = new Pawn();
        var result = sut.PotentialWorkThingsGlobal(pawn);

        Assert.AreSame(expectedThings, result);
        mockDesignationService.Verify(d => d.GetDesignatedThings(It.IsAny<Map>()), Times.Once);
    }
}
