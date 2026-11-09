using CaptureThem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureThem.Tests;

/// <summary>
/// Unit tests for <see cref="ThreatAssessmentService"/>.
/// Validates boundary and safety checks when evaluating danger.
/// </summary>
[TestClass]
public class ThreatAssessmentServiceTests
{
    [TestMethod]
    public void IsDangerNear_ReturnsFalse_WhenTargetOrCapturerIsNull()
    {
        var service = new ThreatAssessmentService();

        Assert.IsFalse(service.IsDangerNear(null!, null!, 40f));
    }
}
