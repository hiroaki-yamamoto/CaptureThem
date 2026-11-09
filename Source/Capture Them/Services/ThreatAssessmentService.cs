using CaptureThem.Interfaces;
using Verse;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for assessing proximate danger around a target pawn.
/// Evaluates hostility, threat state, visibility (fog of war), and horizontal distance.
/// </summary>
public class ThreatAssessmentService : IThreatAssessmentService
{
    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// <b>Time Complexity:</b> O(N) where N is the number of potential attack targets registered in the map's attack target cache for the capturer pawn.
    /// In typical RimWorld maps, N is bounded by the active hostile count (rarely exceeding hundreds).
    /// </para>
    /// <para>
    /// <b>Space Complexity:</b> O(1) auxiliary space, performing in-place enumeration without additional allocations.
    /// </para>
    /// </remarks>
    public bool IsDangerNear(Pawn capturer, Pawn target, float radius)
    {
        if (capturer == null || target == null || !target.Spawned || target.Map == null)
        {
            return false;
        }

        var isTargetFogged = target.Position.Fogged(target.Map);
        var potentialTargets = target.Map.attackTargetsCache.GetPotentialTargetsFor(capturer);

        foreach (var attackTarget in potentialTargets)
        {
            if (attackTarget == null || attackTarget.ThreatDisabled(capturer))
            {
                continue;
            }

            var targetThing = attackTarget.Thing;
            if (targetThing?.Map == null)
            {
                continue;
            }

            var isAttackerFogged = targetThing.Position.Fogged(targetThing.Map);
            if (!isTargetFogged && isAttackerFogged)
            {
                continue;
            }

            if (target.Position.InHorDistOf(targetThing.Position, radius))
            {
                return true;
            }
        }

        return false;
    }
}
