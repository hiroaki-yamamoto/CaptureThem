using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureNotificationService"/>.
/// Validates null handling and safety checks for faction warnings and bed alerts.
/// </summary>
[TestClass]
public class CaptureNotificationServiceTests
{
    [TestMethod]
    public void WarnIfCapturingWillAngerFaction_DoesNothing_WhenVictimIsNull()
    {
        var service = new CaptureNotificationService();

        // Should not throw exception
        service.WarnIfCapturingWillAngerFaction(null!);
    }

    [TestMethod]
    public void WarnIfCapturingWillAngerFaction_DoesNothing_WhenVictimFactionIsNull()
    {
        var service = new CaptureNotificationService();
        var victim = new Pawn();

        // Should safely complete when faction is null
        service.WarnIfCapturingWillAngerFaction(victim);
    }

    [TestMethod]
    public void NotifyNoPrisonerBedAvailable_DoesNothing_WhenVictimIsNull()
    {
        var service = new CaptureNotificationService();

        // Should not throw exception
        service.NotifyNoPrisonerBedAvailable(null!);
    }
}
