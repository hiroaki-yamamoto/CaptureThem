using RimWorld;
using Verse;

namespace CaptureThem;

/// <summary>
/// Provides strongly-typed definitions for the Capture Them mod.
/// Automatically initialized by RimWorld via <see cref="DefOfHelper"/>.
/// </summary>
[DefOf]
public static class CaptureThemDefOf
{
    /// <summary>
    /// The designation definition for marking foreign downed pawns for capture.
    /// </summary>
    public static DesignationDef CaptureThemCapture = null!;

    static CaptureThemDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(CaptureThemDefOf));
    }
}
