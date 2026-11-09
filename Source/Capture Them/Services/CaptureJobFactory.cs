using System;
using CaptureThem.Interfaces;
using RimWorld;
using Verse;
using Verse.AI;

namespace CaptureThem.Services;

/// <summary>
/// Provides default implementation for creating capture jobs and recording player knowledge.
/// </summary>
public class CaptureJobFactory : ICaptureJobFactory
{
    /// <inheritdoc />
    public Job CreateCaptureJob(Pawn capturer, Pawn victim, Building_Bed bed)
    {
        ArgumentNullException.ThrowIfNull(capturer);
        ArgumentNullException.ThrowIfNull(victim);

        var job = JobMaker.MakeJob(JobDefOf.Capture, victim, bed);
        job.count = 1;

        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);

        return job;
    }
}
