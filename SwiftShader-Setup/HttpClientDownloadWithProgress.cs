using System.Net;

namespace SwiftShaderSetup;

internal class HttpClientDownloadWithProgress : IDisposable {

    private readonly HttpClient httpClient = new HttpClient();

    public long NotifyFrequency { get; set; } = 200;

    public event EventHandler<HttpClientDownloadProgressChanged>? ProgressChanged;

    public async Task<HttpStatusCode> TryDownload(string link, string destinationFile) {
        using HttpResponseMessage response = await httpClient.GetAsync(link, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
            return response.StatusCode;

        long? totalBytes = response.Content.Headers.ContentLength;
        if (totalBytes is null)
            throw new NullReferenceException();

        using Stream contentStream = await response.Content.ReadAsStreamAsync();
        await ProcessContentStream(totalBytes.Value, contentStream, destinationFile);

        return response.StatusCode;
    }

    public void Dispose() {
        httpClient.Dispose();
    }

    private async Task ProcessContentStream(long totalBytes, Stream contentStream, string destinationFile) {
        long totalReadedBytes = 0;
        byte[] buffer = new byte[65365];
        bool isMoreToRead = true;

        long lastTime = Environment.TickCount64;
        long lastReadedBytes = totalReadedBytes;

        using FileStream fileStream =
            new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true);

        do {
            int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) {
                isMoreToRead = false;
                NotifyProgressChanged(
                    totalBytes, totalReadedBytes, Environment.TickCount64 - lastTime,
                    totalReadedBytes - lastReadedBytes
                );
                break;
            }

            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalReadedBytes += bytesRead;

            long currentTime = Environment.TickCount64;
            long difference = currentTime - lastTime;

            if (difference >= NotifyFrequency) {
                lastTime = currentTime;
                NotifyProgressChanged(totalBytes, totalReadedBytes, difference, totalReadedBytes - lastReadedBytes);
                lastReadedBytes = totalReadedBytes;
            }
        } while (isMoreToRead);
    }

    private void NotifyProgressChanged(
        long totalBytes, long totalReadedBytes, long timeDifference, long readedBytesInTimeDifference
    ) {
        ProgressChanged?.Invoke(this, new HttpClientDownloadProgressChanged(
            totalBytes, totalReadedBytes, timeDifference, readedBytesInTimeDifference
        ));
    }

}
