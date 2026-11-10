using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verse;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureJobFactory"/>.
/// Validates null guard conditions.
/// </summary>
[TestClass]
public class CaptureJobFactoryTests
{
    [TestMethod]
    public void CreateCaptureJob_ThrowsArgumentNullException_WhenCapturerIsNull()
    {
        var factory = new CaptureJobFactory();
        var victim = new Pawn();

        var ex = Assert.ThrowsException<ArgumentNullException>(() => factory.CreateCaptureJob(null!, victim, null!));
        Assert.AreEqual("capturer", ex.ParamName);
    }

    [TestMethod]
    public void CreateCaptureJob_ThrowsArgumentNullException_WhenVictimIsNull()
    {
        var factory = new CaptureJobFactory();
        var capturer = new Pawn();

        var ex = Assert.ThrowsException<ArgumentNullException>(() => factory.CreateCaptureJob(capturer, null!, null!));
        Assert.AreEqual("victim", ex.ParamName);
    }
}
