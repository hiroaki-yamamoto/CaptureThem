using System.Reflection;
using System.Runtime.CompilerServices;
using CaptureThem.HarmonyPatches;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for Harmony patches in Capture Them.
/// Validates correct trigger conditions for designation removal and UI designator registration.
/// </summary>
[TestClass]
public class HarmonyPatchesTests
{
    [TestCleanup]
    public void Cleanup()
    {
        CaptureThemServices.ResetToDefaults();
    }

    [TestMethod]
    public void Pawn_GuestTracker_SetGuestStatus_RemovesDesignation_WhenGuestStatusIsPrisoner()
    {
        var mockDesignation = new Mock<ICaptureDesignationService>();
        CaptureThemServices.DesignationService = mockDesignation.Object;
        var pawn = new Pawn();

        Pawn_GuestTracker_SetGuestStatus.Postfix(pawn, GuestStatus.Prisoner);

        mockDesignation.Verify(d => d.TryRemoveDesignation(pawn, null), Times.Once);
    }

    [TestMethod]
    public void Pawn_GuestTracker_SetGuestStatus_DoesNotRemoveDesignation_WhenGuestStatusIsNotPrisoner()
    {
        var mockDesignation = new Mock<ICaptureDesignationService>();
        var mockBedAssignment = new Mock<IPrisonerBedAssignmentService>();
        CaptureThemServices.DesignationService = mockDesignation.Object;
        CaptureThemServices.BedAssignmentService = mockBedAssignment.Object;
        var pawn = new Pawn();

        Pawn_GuestTracker_SetGuestStatus.Postfix(pawn, GuestStatus.Slave);
        Pawn_GuestTracker_SetGuestStatus.Postfix(pawn, GuestStatus.Guest);

        mockDesignation.Verify(d => d.TryRemoveDesignation(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
        mockBedAssignment.Verify(b => b.InvalidateBedCache(It.IsAny<Map>()), Times.Exactly(2));
    }

    [TestMethod]
    public void Pawn_HealthTracker_MakeUndowned_RemovesDesignation()
    {
        var mockDesignation = new Mock<ICaptureDesignationService>();
        CaptureThemServices.DesignationService = mockDesignation.Object;
        var pawn = new Pawn();

        Pawn_HealthTracker_MakeUndowned.Prefix(pawn);

        mockDesignation.Verify(d => d.TryRemoveDesignation(pawn, null), Times.Once);
    }

    [TestMethod]
    public void ReverseDesignatorDatabase_InitDesignators_AddsCaptureDesignator()
    {
        var db = (ReverseDesignatorDatabase)RuntimeHelpers.GetUninitializedObject(typeof(ReverseDesignatorDatabase));
        var desListField = typeof(ReverseDesignatorDatabase).GetField("desList", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var designatorList = new List<Designator>();
        desListField?.SetValue(db, designatorList);

        ReverseDesignatorDatabase_InitDesignators.Postfix(ref db);

        Assert.IsTrue(db.AllDesignators.Any(d => d is Designator_CapturePawn));
    }

    [TestMethod]
    public void LordToil_PanicFlee_Init_NotifiesAutoCaptureService()
    {
        var mockAutoCapture = new Mock<IAutoCaptureService>();
        CaptureThemServices.AutoCaptureService = mockAutoCapture.Object;

        var toil = (LordToil_PanicFlee)RuntimeHelpers.GetUninitializedObject(typeof(LordToil_PanicFlee));
        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        typeof(LordToil).GetField("lord", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(toil, lord);

        LordToil_PanicFlee_Init.Postfix(toil);

        mockAutoCapture.Verify(a => a.NotifyRaidFleeing(lord), Times.Once);
    }

    [TestMethod]
    public void Lord_Notify_PawnDowned_NotifiesAutoCaptureService()
    {
        var mockAutoCapture = new Mock<IAutoCaptureService>();
        CaptureThemServices.AutoCaptureService = mockAutoCapture.Object;

        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        var pawn = new Pawn();

        Lord_Notify_PawnDowned.Postfix(lord, pawn);

        mockAutoCapture.Verify(a => a.NotifyRaidPawnDowned(lord, pawn), Times.Once);
    }

    [TestMethod]
    public void Lord_Destroy_NotifiesAutoCaptureService()
    {
        var mockAutoCapture = new Mock<IAutoCaptureService>();
        CaptureThemServices.AutoCaptureService = mockAutoCapture.Object;

        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));

        Lord_Destroy.Postfix(lord);

        mockAutoCapture.Verify(a => a.NotifyRaidLordDestroyed(lord), Times.Once);
    }
}
