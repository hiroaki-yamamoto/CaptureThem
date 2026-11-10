using UnityEngine;
using Verse;

namespace CaptureThem;

/// <summary>
/// Root Mod entry point for Capture Them mod.
/// Exposes mod configuration settings to the RimWorld Mod Options menu.
/// </summary>
public class CaptureThemMod : Mod
{
    /// <summary>
    /// Gets the current loaded settings instance for Capture Them.
    /// </summary>
    public static CaptureThemSettings Settings { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureThemMod"/> class.
    /// Automatically invoked by RimWorld on assembly load.
    /// </summary>
    /// <param name="content">The mod content pack descriptor provided by RimWorld.</param>
    public CaptureThemMod(ModContentPack content)
        : base(content)
    {
        Settings = GetSettings<CaptureThemSettings>();
    }

    /// <inheritdoc />
    public override string SettingsCategory()
    {
        return "CaptureThem_ModName".Translate();
    }

    /// <inheritdoc />
    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);

        var listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);

        listingStandard.CheckboxLabeled(
            "CaptureThem_AutoCaptureAfterRaid_Label".Translate(),
            ref Settings.autoCaptureDownedEnemiesAfterRaid,
            "CaptureThem_AutoCaptureAfterRaid_Desc".Translate());

        listingStandard.End();
    }
}
