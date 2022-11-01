using SwiftShaderSetup;
using System.Net;

if (
    (OperatingSystem.IsWindows() && !WindowsSetup.HasPrivileges()) ||
    (OperatingSystem.IsLinux() && !LinuxSetup.HasPrivileges())
) {
    Console.WriteLine(OperatingSystem.IsLinux());
    Console.WriteLine("Application must be runned with administrator privileges.");
    Environment.Exit(-1);
}

string file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
bool downloaded = false;

using (HttpClientDownloadWithProgress client = new HttpClientDownloadWithProgress()) {
    client.ProgressChanged += (_, x) => {
        const double Mega = 1_000_000;
        const string Round = "0.00";

        string message = $"{(int)(x.TotalReadedBytes / (double)x.TotalBytes * 100):D2}% " +
            $"[{(x.TotalReadedBytes / Mega).ToString(Round)}/{(x.TotalBytes / Mega).ToString(Round)} MB] " +
            $"{(x.ReadedBytesInTimeDifference / (x.TimeDifference / 1000.0) / Mega * 8).ToString(Round)} Mb/s";

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(message);

        for (int i = message.Length; i < Console.WindowWidth; i++)
            Console.Write(' ');
    };

    foreach (string link in args.Length == 0 ? DownloadLinks.GetLinks() : args) {
        Console.WriteLine($"Downloading from: {link}");

        HttpStatusCode statusCode = await client.TryDownload(link, file);
        if (statusCode == HttpStatusCode.OK) {
            downloaded = true;
            break;
        }

        Console.WriteLine($"Unable to download. Status: {(int)statusCode}");
    }
}

if (!downloaded) {
    File.Delete(file);
    Console.WriteLine();
    Console.WriteLine("Unable to find SwiftShader package.");
    Console.WriteLine("Pass download path to first argument to download package from it.");
    Environment.Exit(-1);
} else {
    if (OperatingSystem.IsWindows()) {
        new WindowsSetup(file).Run();
    } else if (OperatingSystem.IsLinux()) {
        new LinuxSetup(file).Run();
    } else {
        File.Delete(file);
        throw new PlatformNotSupportedException();
    }

    Console.WriteLine();
    Console.WriteLine("SwiftShader was successfully installed!");
}

File.Delete(file);
