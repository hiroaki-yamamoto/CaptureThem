using System.Reflection;
using System.Runtime.CompilerServices;
using CaptureThem.Interfaces;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="AutoCaptureService"/>.
/// Validates raid detection, option state verification, and post-raid capture designation logic.
/// </summary>
[TestClass]
public class AutoCaptureServiceTests
{
    private Mock<ICaptureSettingsService> mockSettings = null!;
    private Mock<ICaptureEligibilityService> mockEligibility = null!;
    private Mock<ICaptureDesignationService> mockDesignation = null!;

    [TestInitialize]
    public void SetUp()
    {
        mockSettings = new Mock<ICaptureSettingsService>();
        mockEligibility = new Mock<ICaptureEligibilityService>();
        mockDesignation = new Mock<ICaptureDesignationService>();

        mockSettings.Setup(s => s.AutoCaptureDownedEnemiesAfterRaid).Returns(true);
    }

    [TestCleanup]
    public void TearDown()
    {
        CaptureThemServices.ResetToDefaults();
    }

    [TestMethod]
    public void Constructor_ResolvesDefaults_WhenServicesOmitted()
    {
        var service = new AutoCaptureService();
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void IsHostileColonyRaid_ReturnsFalse_WhenLordOrComponentsAreNull()
    {
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);

        Assert.IsFalse(service.IsHostileColonyRaid(null));

        var emptyLord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        Assert.IsFalse(service.IsHostileColonyRaid(emptyLord));
    }

    [TestMethod]
    public void IsHostileColonyRaid_ReturnsFalse_WhenFactionIsNotHostile()
    {
        var nonHostileFaction = (Faction)RuntimeHelpers.GetUninitializedObject(typeof(Faction));
        var service = new AutoCaptureService(
            mockSettings.Object,
            mockEligibility.Object,
            mockDesignation.Object,
            isHostileToPlayerPredicate: _ => false);

        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        var lordManager = (LordManager)RuntimeHelpers.GetUninitializedObject(typeof(LordManager));
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));

        typeof(LordManager).GetField("map", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lordManager, map);
        typeof(Lord).GetField("lordManager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, lordManager);
        typeof(Lord).GetField("faction", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, nonHostileFaction);
        typeof(Lord).GetField("curJob", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, RuntimeHelpers.GetUninitializedObject(typeof(LordJob_AssaultColony)));

        Assert.IsFalse(service.IsHostileColonyRaid(lord));
    }

    [TestMethod]
    public void IsHostileColonyRaid_ReturnsTrue_WhenHostileRaidLordJob()
    {
        var hostileFaction = (Faction)RuntimeHelpers.GetUninitializedObject(typeof(Faction));
        var service = new AutoCaptureService(
            mockSettings.Object,
            mockEligibility.Object,
            mockDesignation.Object,
            isHostileToPlayerPredicate: f => f == hostileFaction);

        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        var lordManager = (LordManager)RuntimeHelpers.GetUninitializedObject(typeof(LordManager));
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));

        typeof(LordManager).GetField("map", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lordManager, map);
        typeof(Lord).GetField("lordManager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, lordManager);
        typeof(Lord).GetField("faction", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, hostileFaction);
        typeof(Lord).GetField("curJob", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, RuntimeHelpers.GetUninitializedObject(typeof(LordJob_AssaultColony)));

        Assert.IsTrue(service.IsHostileColonyRaid(lord));
    }

    [TestMethod]
    public void AutoCaptureDownedEnemiesAfterRaid_ReturnsZero_WhenDisabledInSettings()
    {
        mockSettings.Setup(s => s.AutoCaptureDownedEnemiesAfterRaid).Returns(false);
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));

        int result = service.AutoCaptureDownedEnemiesAfterRaid(map);

        Assert.AreEqual(0, result);
        mockDesignation.Verify(d => d.Designate(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void AutoCaptureDownedEnemiesAfterRaid_ReturnsZero_WhenMapIsNull()
    {
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);

        int result = service.AutoCaptureDownedEnemiesAfterRaid(null);

        Assert.AreEqual(0, result);
        mockDesignation.Verify(d => d.Designate(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void AutoCaptureDownedEnemiesAfterRaid_DesignatesEligiblePawns()
    {
        var hostileFaction = (Faction)RuntimeHelpers.GetUninitializedObject(typeof(Faction));
        var otherFaction = (Faction)RuntimeHelpers.GetUninitializedObject(typeof(Faction));

        var service = new AutoCaptureService(
            mockSettings.Object,
            mockEligibility.Object,
            mockDesignation.Object,
            isHostileToPlayerPredicate: f => f == hostileFaction);

        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        var mapPawns = (MapPawns)RuntimeHelpers.GetUninitializedObject(typeof(MapPawns));

        typeof(Map).GetField("mapPawns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValue(map, mapPawns);

        var eligibleHostilePawn = new Pawn();
        var ineligiblePawn = new Pawn();
        var otherFactionPawn = new Pawn();

        var factionField = typeof(Thing).GetField("factionInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        factionField?.SetValue(eligibleHostilePawn, hostileFaction);
        factionField?.SetValue(ineligiblePawn, hostileFaction);
        factionField?.SetValue(otherFactionPawn, otherFaction);

        var spawnedPawns = new List<Pawn> { eligibleHostilePawn, ineligiblePawn, otherFactionPawn };
        typeof(MapPawns).GetField("pawnsSpawned", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(mapPawns, spawnedPawns);

        mockEligibility.Setup(e => e.CanDesignateThing(eligibleHostilePawn, map))
            .Returns(AcceptanceReport.WasAccepted);
        mockEligibility.Setup(e => e.CanDesignateThing(ineligiblePawn, map))
            .Returns(AcceptanceReport.WasRejected);

        int count = service.AutoCaptureDownedEnemiesAfterRaid(map, hostileFaction);

        Assert.AreEqual(1, count);
        mockDesignation.Verify(d => d.Designate(eligibleHostilePawn, map), Times.Once);
        mockDesignation.Verify(d => d.Designate(ineligiblePawn, map), Times.Never);
        mockDesignation.Verify(d => d.Designate(otherFactionPawn, map), Times.Never);
    }

    [TestMethod]
    public void NotifyRaidFleeing_DoesNothing_WhenLordIsNotHostileColonyRaid()
    {
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);
        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));

        service.NotifyRaidFleeing(lord);

        mockDesignation.Verify(d => d.Designate(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void NotifyRaidPawnDowned_DoesNothing_WhenLordIsNotHostileColonyRaid()
    {
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);
        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));

        service.NotifyRaidPawnDowned(lord, new Pawn());

        mockDesignation.Verify(d => d.Designate(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void NotifyRaidLordDestroyed_DoesNothing_WhenLordIsNotHostileColonyRaid()
    {
        var service = new AutoCaptureService(mockSettings.Object, mockEligibility.Object, mockDesignation.Object);
        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));

        service.NotifyRaidLordDestroyed(lord);

        mockDesignation.Verify(d => d.Designate(It.IsAny<Thing>(), It.IsAny<Map>()), Times.Never);
    }

    [TestMethod]
    public void NotifyRaidFleeing_InvokesAutoCapture_WhenHostileRaid()
    {
        var hostileFaction = (Faction)RuntimeHelpers.GetUninitializedObject(typeof(Faction));
        var service = new AutoCaptureService(
            mockSettings.Object,
            mockEligibility.Object,
            mockDesignation.Object,
            isHostileToPlayerPredicate: f => f == hostileFaction);

        var lord = (Lord)RuntimeHelpers.GetUninitializedObject(typeof(Lord));
        var lordManager = (LordManager)RuntimeHelpers.GetUninitializedObject(typeof(LordManager));
        var map = (Map)RuntimeHelpers.GetUninitializedObject(typeof(Map));
        var mapPawns = (MapPawns)RuntimeHelpers.GetUninitializedObject(typeof(MapPawns));

        typeof(LordManager).GetField("map", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lordManager, map);
        typeof(Lord).GetField("lordManager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, lordManager);
        typeof(Lord).GetField("faction", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, hostileFaction);
        typeof(Lord).GetField("curJob", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(lord, RuntimeHelpers.GetUninitializedObject(typeof(LordJob_AssaultColony)));
        typeof(Map).GetField("mapPawns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValue(map, mapPawns);

        var downedHostile = new Pawn();
        typeof(Thing).GetField("factionInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(downedHostile, hostileFaction);

        typeof(MapPawns).GetField("pawnsSpawned", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            ?.SetValue(mapPawns, new List<Pawn> { downedHostile });

        mockEligibility.Setup(e => e.CanDesignateThing(downedHostile, map)).Returns(AcceptanceReport.WasAccepted);

        service.NotifyRaidFleeing(lord);

        mockDesignation.Verify(d => d.Designate(downedHostile, map), Times.Once);
    }
}
