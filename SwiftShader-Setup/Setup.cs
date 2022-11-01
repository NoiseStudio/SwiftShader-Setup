using System.IO.Compression;

namespace SwiftShaderSetup;

internal abstract class Setup {

    private readonly string zipFile;

    protected Setup(string zipFile) {
        this.zipFile = zipFile;
    }

    public abstract void Run();

    protected void Extract(string destinationDirectory) {
        Console.Write("Extracting package...");

        Directory.CreateDirectory(destinationDirectory);
        ZipFile.ExtractToDirectory(zipFile, destinationDirectory);

        Console.WriteLine(" Done!");
    }

}
