using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CaptureThem.HarmonyPatches;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RimWorld;
using Verse;

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
        CaptureThemServices.DesignationService = mockDesignation.Object;
        var pawn = new Pawn();

        Pawn_GuestTracker_SetGuestStatus.Postfix(pawn, GuestStatus.Slave);
        Pawn_GuestTracker_SetGuestStatus.Postfix(pawn, GuestStatus.Guest);

        mockDesignation.Verify(d => d.TryRemoveDesignation(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
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
}
