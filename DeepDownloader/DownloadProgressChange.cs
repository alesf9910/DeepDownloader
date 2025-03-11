using System;

namespace DeepDownloader
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public int Id;
        public long BytesReceived;
        public long TotalBytes;
        public DownloadState State;
        internal TimeSpan TimeElapsed;
        public double DownloadPercentage => Math.Round(TotalBytes > 0 ? BytesReceived * 100 / (double)TotalBytes : 0, 2);
        public double Velocity => TotalBytes > 0 ? BytesReceived / TimeElapsed.TotalSeconds : 0;
        public ProgressChangedEventArgs(int id, long bytesReceived, long totalBytes, DownloadState state)
        {
            Id = id;
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
            State = state;
        }
    }
}
