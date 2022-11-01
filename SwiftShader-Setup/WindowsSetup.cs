using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

namespace SwiftShaderSetup;

[SuppressMessage("Interoperability", "CA1416: Validate platform compatibility")]
internal class WindowsSetup : Setup {

    private const string DriversKey = "SOFTWARE\\Khronos\\Vulkan\\Drivers\\";

    private string DestinationDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "NoiseStudio", "SwiftShader"
    );

    public WindowsSetup(string zipFile) : base(zipFile) {
    }

    public static bool HasPrivileges() {
        Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
        WindowsPrincipal principal = (WindowsPrincipal)(Thread.CurrentPrincipal ?? throw new NullReferenceException());
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public override void Run() {
        string destinationDirectory = DestinationDirectory;
        Extract(destinationDirectory);

        Console.Write("Creating registry key... ");

        RegistryKey key = Registry.LocalMachine.CreateSubKey(DriversKey);
        key.SetValue(Directory.GetFiles(destinationDirectory, "*.json").First(), 0, RegistryValueKind.DWord);
        key.Close();

        Console.WriteLine(" Done!");
    }

}
