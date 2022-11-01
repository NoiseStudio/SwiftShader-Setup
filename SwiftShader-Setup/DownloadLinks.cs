using System.Runtime.InteropServices;

namespace SwiftShaderSetup;

internal static class DownloadLinks {

    private const string GitHub =
        "https://github.com/NoiseStudio/SwiftShader-Builds/releases/latest/download/SwiftShader-";

    public static IEnumerable<string> GetLinks() {
        return RuntimeInformation.OSArchitecture switch {
            Architecture.X64 => GetLinksAmd64(),
            _ => throw new PlatformNotSupportedException(),
        };
    }

    private static IEnumerable<string> GetLinksAmd64() {
        if (OperatingSystem.IsWindows()) {
            yield return GitHub + "Windows-AMD64.zip";
        } else if (OperatingSystem.IsLinux()) {
            yield return GitHub + "Linux-AMD64.zip";
        } else {
            throw new PlatformNotSupportedException();
        }
    }

}
