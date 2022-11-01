namespace SwiftShaderSetup;

internal class HttpClientDownloadProgressChanged : EventArgs {

    public long TotalBytes { get; }
    public long TotalReadedBytes { get; }
    public long TimeDifference { get; }
    public long ReadedBytesInTimeDifference { get; }

    public HttpClientDownloadProgressChanged(
        long totalBytes, long totalReadedBytes, long timeDifference, long readedBytesInTimeDifference
    ) {
        TotalBytes = totalBytes;
        TotalReadedBytes = totalReadedBytes;
        TimeDifference = timeDifference;
        ReadedBytesInTimeDifference = readedBytesInTimeDifference;
    }

}
