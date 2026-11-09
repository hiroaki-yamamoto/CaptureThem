using System;
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
    public void JobOnThing_ReturnsNull_WhenTargetIsNotPawn()
    {
        var sut = CreateSut();

        var result = sut.JobOnThing(null!, null!, false);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void PotentialWorkThingsGlobal_DelegatesToDesignationService()
    {
        var sut = CreateSut();
        var expectedThings = new List<Thing>();
        mockDesignationService
            .Setup(d => d.GetDesignatedThings(It.IsAny<Map>()))
            .Returns(expectedThings);

        // Pawn with null map will pass null to GetDesignatedThings
        // We verify the call is forwarded without throwing
        var pawn = new Pawn();
        var result = sut.PotentialWorkThingsGlobal(pawn);

        Assert.AreSame(expectedThings, result);
        mockDesignationService.Verify(d => d.GetDesignatedThings(It.IsAny<Map>()), Times.Once);
    }
}
