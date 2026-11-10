namespace CaptureThem.Interfaces;

/// <summary>
/// Defines access to configuration settings for the Capture Them mod.
/// Facilitates Dependency Inversion by decoupling domain services and UI from the concrete ModSettings implementation.
/// </summary>
public interface ICaptureSettingsService
{
    /// <summary>
    /// Gets or sets a value indicating whether eligible downed enemy pawns should be automatically
    /// designated for capture once an enemy raid on the colony concludes.
    /// </summary>
    bool AutoCaptureDownedEnemiesAfterRaid { get; set; }
}
