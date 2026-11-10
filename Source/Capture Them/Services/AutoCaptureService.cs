using System;
using CaptureThem.Interfaces;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CaptureThem.Services;

/// <summary>
/// Default implementation of <see cref="IAutoCaptureService"/>.
/// Coordinates raid detection and automated capture designation in accordance with SOLID principles.
/// </summary>
public class AutoCaptureService : IAutoCaptureService
{
    private readonly Func<ICaptureSettingsService> settingsServiceAccessor;
    private readonly Func<ICaptureEligibilityService> eligibilityServiceAccessor;
    private readonly Func<ICaptureDesignationService> designationServiceAccessor;
    private readonly Func<Faction?, bool> isHostileToPlayerPredicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCaptureService"/> class.
    /// </summary>
    /// <param name="settingsService">Optional settings service override.</param>
    /// <param name="eligibilityService">Optional eligibility service override.</param>
    /// <param name="designationService">Optional designation service override.</param>
    /// <param name="isHostileToPlayerPredicate">Optional hostile faction evaluation predicate override.</param>
    public AutoCaptureService(
        ICaptureSettingsService? settingsService = null,
        ICaptureEligibilityService? eligibilityService = null,
        ICaptureDesignationService? designationService = null,
        Func<Faction?, bool>? isHostileToPlayerPredicate = null)
    {
        settingsServiceAccessor = settingsService != null
            ? () => settingsService
            : () => CaptureThemServices.SettingsService;

        eligibilityServiceAccessor = eligibilityService != null
            ? () => eligibilityService
            : () => CaptureThemServices.EligibilityService;

        designationServiceAccessor = designationService != null
            ? () => designationService
            : () => CaptureThemServices.DesignationService;

        this.isHostileToPlayerPredicate = isHostileToPlayerPredicate ?? IsHostileToPlayerDefault;
    }

    private ICaptureSettingsService SettingsService => settingsServiceAccessor();
    private ICaptureEligibilityService EligibilityService => eligibilityServiceAccessor();
    private ICaptureDesignationService DesignationService => designationServiceAccessor();

    /// <inheritdoc />
    public bool IsHostileColonyRaid(Lord? lord)
    {
        var map = SafeGetMap(lord);
        if (lord == null || map == null || lord.faction == null || lord.LordJob == null)
        {
            return false;
        }

        if (!isHostileToPlayerPredicate(lord.faction))
        {
            return false;
        }

        var job = lord.LordJob;
        if (job is LordJob_AssaultColony ||
            job is LordJob_Siege ||
            job is LordJob_StageThenAttack ||
            job is LordJob_SleepThenAssaultColony ||
            job is LordJob_BossgroupAssaultColony)
        {
            return true;
        }

        var jobTypeName = job.GetType().Name;
        return jobTypeName.Contains("AssaultColony") ||
               jobTypeName.Contains("Siege") ||
               jobTypeName.Contains("StageThenAttack");
    }

    /// <inheritdoc />
    public int AutoCaptureDownedEnemiesAfterRaid(Map? map, Faction? raidFaction = null)
    {
        if (map == null || !SettingsService.AutoCaptureDownedEnemiesAfterRaid)
        {
            return 0;
        }

        var allPawns = map.mapPawns?.AllPawnsSpawned;
        if (allPawns == null)
        {
            return 0;
        }

        int designatedCount = 0;
        for (int i = 0; i < allPawns.Count; i++)
        {
            var pawn = allPawns[i];
            if (pawn?.Faction == null)
            {
                continue;
            }

            if (raidFaction != null && pawn.Faction != raidFaction)
            {
                continue;
            }

            if (!isHostileToPlayerPredicate(pawn.Faction))
            {
                continue;
            }

            if (EligibilityService.CanDesignateThing(pawn, map).Accepted)
            {
                DesignationService.Designate(pawn, map);
                designatedCount++;
            }
        }

        if (designatedCount > 0)
        {
            try
            {
                Messages.Message(
                    "CaptureThem_AutoCapturedEnemiesMessage".Translate(designatedCount),
                    MessageTypeDefOf.NeutralEvent,
                    historical: false);
            }
            catch
            {
                // Silently ignore if message queue or DefOf is uninitialized in isolated testing environments
            }
        }

        return designatedCount;
    }

    /// <inheritdoc />
    public void NotifyRaidFleeing(Lord? lord)
    {
        var map = SafeGetMap(lord);
        if (!IsHostileColonyRaid(lord) || map == null)
        {
            return;
        }

        AutoCaptureDownedEnemiesAfterRaid(map, lord?.faction);
    }

    /// <inheritdoc />
    public void NotifyRaidPawnDowned(Lord? lord, Pawn? downedPawn)
    {
        var map = SafeGetMap(lord);
        if (!IsHostileColonyRaid(lord) || map == null)
        {
            return;
        }

        var ownedPawns = lord?.ownedPawns;
        if (ownedPawns != null)
        {
            bool anyMobile = false;
            for (int i = 0; i < ownedPawns.Count; i++)
            {
                var p = ownedPawns[i];
                if (p != null && p.Spawned && !p.Downed && !p.Dead)
                {
                    anyMobile = true;
                    break;
                }
            }

            if (!anyMobile)
            {
                AutoCaptureDownedEnemiesAfterRaid(map, lord?.faction);
            }
        }
    }

    /// <inheritdoc />
    public void NotifyRaidLordDestroyed(Lord? lord)
    {
        var map = SafeGetMap(lord);
        if (!IsHostileColonyRaid(lord) || map == null)
        {
            return;
        }

        AutoCaptureDownedEnemiesAfterRaid(map, lord?.faction);
    }

    private static Map? SafeGetMap(Lord? lord)
    {
        if (lord == null)
        {
            return null;
        }

        try
        {
            return lord.Map;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsHostileToPlayerDefault(Faction? faction)
    {
        if (faction == null)
        {
            return false;
        }

        try
        {
            var playerFaction = Find.FactionManager?.OfPlayer;
            if (playerFaction == null)
            {
                // If FactionManager is not available in unit test environments, fallback to relationWithPlayer check if present
                var relation = faction.RelationWith(Faction.OfPlayerSilentFail, false);
                return relation?.kind == FactionRelationKind.Hostile;
            }

            return faction.HostileTo(playerFaction);
        }
        catch
        {
            // If RimWorld game world is not initialized (e.g. mock test), check direct hostile fields or relations if available
            try
            {
                var rel = faction.RelationKindWith(Faction.OfPlayerSilentFail);
                return rel == FactionRelationKind.Hostile;
            }
            catch
            {
                return false;
            }
        }
    }
}
