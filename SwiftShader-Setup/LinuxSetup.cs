namespace SwiftShaderSetup;

internal class LinuxSetup : Setup {

    private const string DestinationDirectory = "/etc/vulkan/icd.d";

    public LinuxSetup(string zipFile) : base(zipFile) {
    }

    public static bool HasPrivileges() {
        try {
            string path = Path.Combine(DestinationDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(path);
            Directory.Delete(path);
            return true;
        } catch {
            return false;
        }
    }

    public override void Run() {
        Extract(DestinationDirectory);
    }

}
