using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClustEncActor
{
    public enum SampleEncoderResult
    {
        Success,
        Error
    }

    public class SampleFfmpegEncoder
    {
        public async Task<SampleEncoderResult> EncodeAsync(string sourceUrl, string targetPath, string parameters)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "Ffmpeg/ffmpeg.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                Arguments = $"-i \"{sourceUrl}\" {parameters} {targetPath}"
            };

            try
            {
                int returnCode = -1;
                using (Process exeProcess = Process.Start(startInfo))
                {
                    if (exeProcess == null) throw new InvalidOperationException();
                    string log = "";
                    while (!exeProcess.HasExited)
                    {
                        log += exeProcess?.StandardError.ReadToEnd();
                        await Task.Delay(1000);
                    }
                    //TODO Use error output
                    returnCode = exeProcess.ExitCode;
                }
                return returnCode == 0 ? SampleEncoderResult.Success : SampleEncoderResult.Error;
            }
            catch (Exception)
            {
                // TODO Handle error.
            }

            return SampleEncoderResult.Error;
        }
    }
}