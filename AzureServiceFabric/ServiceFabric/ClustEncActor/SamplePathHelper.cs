using System;
using System.IO;
using System.Text.RegularExpressions;
using ClustEncActor.Interfaces;

namespace ClustEncActor
{
    public static class SamplePathHelper
    {
        public static string SampleFileNameToString(Guid taskId, DateTime createdAt, NamedState state)
        {
            var sampleFileName = new SampleFileName { TaskId = taskId, CreatedAt = createdAt, State = state };
            var createdAtEpoch = (long)(sampleFileName.CreatedAt - new DateTime(1970, 1, 1)).TotalSeconds;
            return $"{sampleFileName.TaskId.ToString("N")}_{sampleFileName.State}_{createdAtEpoch}";
        }

        public static SampleFileName ParseSampleFileName(string name)
        {
            var sampleFileName = new SampleFileName();
            name = Regex.Replace(name, @".*\/", ""); // Remove path
            name = Regex.Replace(name, @"\.[^\.]+$", ""); // Remove extension
            var parts = name.Split('_');
            if (parts.Length == 3)
            {
                Guid.TryParseExact(parts[0], "N", out sampleFileName.TaskId);
                Enum.TryParse(parts[1], out sampleFileName.State);
                long createdAtEpoch;
                Int64.TryParse(parts[2], out createdAtEpoch);
                sampleFileName.CreatedAt = new DateTime(1970, 1, 1).AddSeconds(createdAtEpoch);
            }

            return sampleFileName;
        }

        public static string LocalPath(string fileName)
        {
            var dataDir = Directory.GetCurrentDirectory() + "/../temp";
            return Cleanup(Path.Combine(dataDir, fileName));
        }

        public static string RemotePath(string fileName, string userId)
        {
            return Cleanup(Path.Combine(userId, fileName));
        }

        private static string Cleanup(string path)
        {
            return path.Replace("\\", "/").Replace("//", "/").TrimStart('/').TrimEnd('.');
        }
    }
}