using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeepDownloader
{
    public class Downloader : IDisposable
    {
        public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;
        private readonly HttpClient client;
        private CancellationTokenSource cancellationTokenSource;
        private PartData[] partsData;
        private long fileSize;

        public string Url { get; private set; }
        public string Path { get; private set; }
        public int TaskCount { get; private set; }

        public Downloader(string url, string path, int taskCount, HttpClient client)
        {
            this.client = client;
            this.Url = url;
            this.Path = path;
            this.TaskCount = taskCount;
        }

        public Downloader(string url, string path, int taskCount)
        {
            this.client = new HttpClient();
            this.Url = url;
            this.Path = path;
            this.TaskCount = taskCount;
        }

        private void UpdateProgress(int id, long bytesReceived, long totalBytes, DownloadState state)
        {
            OnProgressChanged?.Invoke(this,
                new ProgressChangedEventArgs(id, bytesReceived, totalBytes, state));
        }
    
        public async Task StartAsync()
        {
            cancellationTokenSource = new CancellationTokenSource();
            if (partsData == null) await BuildPartData();
            Task[] tasks = new Task[TaskCount];
            for (int i = 0; i < TaskCount; i++) tasks[i] = Download(i);
            try
            {
                await Task.WhenAll(tasks);
                await Merge();
                UpdateProgress(-1, fileSize, fileSize, DownloadState.Completed);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
        }

        public void SaveState(Stream stream)
        {
            using TextWriter writer = new StreamWriter(stream);
            writer.Write(EncodePartsData());
        }

        public async Task SaveStateAsync(Stream stream)
        {
            using TextWriter writer = new StreamWriter(stream);
            await writer.WriteAsync(EncodePartsData());
        }

        public async Task LoadStateAsync(Stream stream)
        {
            using TextReader reader = new StreamReader(stream);
            DecodePartsData(await reader.ReadToEndAsync());
        }

        public void LoadState(Stream stream)
        {
            using TextReader reader = new StreamReader(stream);
            DecodePartsData(reader.ReadToEnd());
        }

        public void Dispose()
        {
            client.Dispose();
            cancellationTokenSource?.Dispose();
        }

        private string EncodePartsData()
        {
            var sb = new StringBuilder();
            foreach (var partData in partsData)
            {
                sb.Append($"{partData.Start}:{partData.End},");
            }
            return sb.ToString();
        }

        private void DecodePartsData(string partsDataText)
        {
            partsData = partsDataText.Split(",").Select(partData =>
            {
                var partDataSplited = partData.Split(':');
                return new PartData { Start = long.Parse(partDataSplited[0]), End = long.Parse(partDataSplited[1]) };
            }).ToArray();
        }
        
        private async Task BuildPartData()
        {
            using var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, Url));
            response.EnsureSuccessStatusCode();
            fileSize = response.Content.Headers.ContentLength.GetValueOrDefault(0);
            if (fileSize == 0) throw new Exception("Invalid file size");
            long max_size = fileSize / TaskCount;
            long rest = fileSize % TaskCount;
            partsData = new PartData[TaskCount];
            long start = 0;
            for(int i = 0; i < TaskCount; i++)
            {
                var size = max_size + (rest > 0 ? 1 : 0);
                partsData[i] = new PartData
                {
                    Start = start,
                    End = start + size 
                };
                rest--;
                start += size;
            }
        }

        private async Task Download(int i)
        {
            if (partsData[i].Start >= partsData[i].End) return;
            var request = new HttpRequestMessage(HttpMethod.Get, Url);
            request.Headers.Range = new RangeHeaderValue(partsData[i].Start, partsData[i].End);
            var response = await client.SendAsync(request);
            using FileStream fileStream = new FileStream(Path + $".{i}", FileMode.Append);
            using Stream read = await response.Content.ReadAsStreamAsync();
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = await read.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                partsData[i].Start += bytesRead;
                UpdateProgress(i, partsData[i].Start, partsData[i].End, DownloadState.Downloading);
                if (cancellationTokenSource.IsCancellationRequested) throw new OperationCanceledException();
            }
        }

        private async Task Merge()
        {
            using FileStream fileStream = new FileStream(Path, FileMode.Create);
            long merge = 0;
            for (int i = 0; i < TaskCount; i++)
            {
                string file = Path + $".{i}";
                using FileStream read = new FileStream(file, FileMode.Open);
                byte[] buffer = new byte[10240];
                int bytesRead;
                while ((bytesRead = await read.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    merge += bytesRead;
                    UpdateProgress(-1, merge, fileSize, DownloadState.Mixing);
                }
            }
        }
    }
}
