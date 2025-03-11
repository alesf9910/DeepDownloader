using DeepDownloader;

var url = "http://ucistore.uci.cu/software/7-Zip/7-Zip_24.09_W_ANY_32_LL.exe";
Downloader downloader = new Downloader(url, "D:\\7Zip.exe", 4);
downloader.OnProgressChanged += OnProgressChanged;

void OnProgressChanged(object? sender, ProgressChangedEventArgs e)
{
    switch (e.State)
    {
        case DownloadState.Downloading:
            Console.WriteLine("{2}: {0} - {1} -> {3}", e.BytesReceived, e.TotalBytes, e.Id, e.DownloadPercentage);
            break;
        case DownloadState.Mixing:
            Console.WriteLine("{2}: {0} - {1} -> {3}", e.BytesReceived, e.TotalBytes, e.Id, e.DownloadPercentage);
            break;
        case DownloadState.Completed:
            Console.WriteLine("Descarga Completada");
            Console.WriteLine("{2}: {0} - {1} -> {3}", e.BytesReceived, e.TotalBytes, e.Id, e.DownloadPercentage);
            break;
    }
}

await downloader.StartAsync();