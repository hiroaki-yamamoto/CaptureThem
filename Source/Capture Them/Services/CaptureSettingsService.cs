using CaptureThem.Interfaces;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for querying and updating Capture Them settings.
/// Bridges the domain services to <see cref="CaptureThemMod.Settings"/> with support for runtime testing overrides.
/// </summary>
public class CaptureSettingsService : ICaptureSettingsService
{
    private bool? overrideAutoCapture;

    /// <inheritdoc />
    public bool AutoCaptureDownedEnemiesAfterRaid
    {
        get => overrideAutoCapture ?? CaptureThemMod.Settings?.autoCaptureDownedEnemiesAfterRaid ?? true;
        set
        {
            if (CaptureThemMod.Settings != null)
            {
                CaptureThemMod.Settings.autoCaptureDownedEnemiesAfterRaid = value;
            }

            overrideAutoCapture = value;
        }
    }
}
