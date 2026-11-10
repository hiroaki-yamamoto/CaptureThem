using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CaptureThem.Tests;

/// <summary>
/// Assembly-level test lifecycle coordinator.
/// Configures dynamic assembly resolution so that RimWorld assemblies can be located during unit test execution.
/// </summary>
[TestClass]
public class TestInitializer
{
    private static bool isInitialized;

    /// <summary>
    /// Registers dynamic assembly resolution hooks for RimWorld dependencies before unit tests run.
    /// </summary>
    /// <param name="context">The MSTest execution context, or <c>null</c> if invoked from custom runner.</param>
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext? context = null)
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;

        AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
        {
            var assemblyName = new AssemblyName(eventArgs.Name).Name;
            if (string.IsNullOrEmpty(assemblyName))
            {
                return null;
            }

            string rimWorldDir = Path.GetFullPath(@"D:\SteamLibrary\steamapps\common\RimWorld");
            string candidate = Path.Combine(rimWorldDir, assemblyName + ".dll");
            if (File.Exists(candidate))
            {
                return Assembly.LoadFrom(candidate);
            }

            string workshopHarmony = Path.GetFullPath(@"D:\SteamLibrary\steamapps\workshop\content\294100\2009463077\1.7\Assemblies\0Harmony.dll");
            if (assemblyName == "0Harmony" && File.Exists(workshopHarmony))
            {
                return Assembly.LoadFrom(workshopHarmony);
            }

            return null;
        };

        // Do not reference any Verse/RimWorld types directly in this method
        // so the JIT doesn't eagerly resolve Assembly-CSharp before the hook is registered.
        InitializeRimWorldEnvironment();
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private static void InitializeRimWorldEnvironment()
    {
        try
        {
            var customSaveDataPathField = typeof(Verse.GenFilePaths).GetField("saveDataPath", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (customSaveDataPathField != null)
            {
                customSaveDataPathField.SetValue(null, Path.GetTempPath());
            }

            if (CaptureThemDefOf.CaptureThemCapture == null)
            {
                CaptureThemDefOf.CaptureThemCapture = new Verse.DesignationDef { defName = "CaptureThemCapture" };
            }
        }
        catch
        {
            // Ignore if path or def setup fails
        }
    }
}
