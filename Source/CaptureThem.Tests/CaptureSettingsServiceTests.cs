using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureSettingsService"/>.
/// Validates setting value accessors, default values, and runtime modification.
/// </summary>
[TestClass]
public class CaptureSettingsServiceTests
{
    [TestCleanup]
    public void Cleanup()
    {
        CaptureThemServices.ResetToDefaults();
    }

    [TestMethod]
    public void AutoCaptureDownedEnemiesAfterRaid_DefaultsToTrue()
    {
        var settingsService = new CaptureSettingsService();

        Assert.IsTrue(settingsService.AutoCaptureDownedEnemiesAfterRaid);
    }

    [TestMethod]
    public void AutoCaptureDownedEnemiesAfterRaid_CanBeToggled()
    {
        var settingsService = new CaptureSettingsService();

        settingsService.AutoCaptureDownedEnemiesAfterRaid = false;
        Assert.IsFalse(settingsService.AutoCaptureDownedEnemiesAfterRaid);

        settingsService.AutoCaptureDownedEnemiesAfterRaid = true;
        Assert.IsTrue(settingsService.AutoCaptureDownedEnemiesAfterRaid);
    }
}
