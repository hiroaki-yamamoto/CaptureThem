using Verse;

namespace CaptureThem;

/// <summary>
/// Persistent settings configuration for the Capture Them mod.
/// Serialized and restored via RimWorld's Scribe engine.
/// </summary>
public class CaptureThemSettings : ModSettings
{
    /// <summary>
    /// Backing store for whether downed hostile pawns should be automatically captured after a raid ends.
    /// Default is <c>true</c>.
    /// </summary>
    public bool autoCaptureDownedEnemiesAfterRaid = true;

    /// <inheritdoc />
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref autoCaptureDownedEnemiesAfterRaid, "autoCaptureDownedEnemiesAfterRaid", true);
    }
}
