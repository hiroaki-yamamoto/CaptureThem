using System;
using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="CaptureJobFactory"/>.
/// Validates null guard conditions.
/// </summary>
[TestClass]
public class CaptureJobFactoryTests
{
    [TestMethod]
    public void CreateCaptureJob_ThrowsArgumentNullException_WhenCapturerOrVictimIsNull()
    {
        var factory = new CaptureJobFactory();

        Assert.ThrowsException<ArgumentNullException>(() => factory.CreateCaptureJob(null!, null!, null!));
    }
}
