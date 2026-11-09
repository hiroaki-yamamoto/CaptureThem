using CaptureThem.Interfaces;
using RimWorld;
using Verse;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for presenting capture-related messages and alerts to the player.
/// Encapsulates interaction with RimWorld's <see cref="Messages"/> system.
/// </summary>
public class CaptureNotificationService : ICaptureNotificationService
{
    /// <inheritdoc />
    public void WarnIfCapturingWillAngerFaction(Pawn victim)
    {
        if (victim?.Faction != null &&
            victim.Faction != Faction.OfPlayer &&
            !victim.Faction.Hidden &&
            !victim.Faction.HostileTo(Faction.OfPlayer) &&
            !victim.IsPrisonerOfColony &&
            victim.RaceProps.Humanlike)
        {
            Messages.Message(
                "MessageCapturingWillAngerFaction".Translate(victim.Named("PAWN")).AdjustedFor(victim),
                victim,
                MessageTypeDefOf.CautionInput,
                false);
        }
    }

    /// <inheritdoc />
    public void NotifyNoPrisonerBedAvailable(Pawn victim)
    {
        if (victim == null)
        {
            return;
        }

        Messages.Message(
            "CannotCapture".Translate() + ": " + "NoPrisonerBed".Translate(),
            victim,
            MessageTypeDefOf.RejectInput,
            false);
    }
}
