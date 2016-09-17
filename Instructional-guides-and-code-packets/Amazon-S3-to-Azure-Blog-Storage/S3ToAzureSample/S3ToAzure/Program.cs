// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace S3ToAzure
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BasicCloudCopy;
    using CommandLine;

    #endregion

    internal class Program
    {
        #region Private Static Fields and Constants

        private static readonly Dictionary<string, int> Lines = new Dictionary<string, int>();
        private static int _cursorLeft;
        private static int _cursorTop;

        #endregion

        #region Internal Methods

        internal static int Main(string[] args)
        {
            _cursorTop = Console.CursorTop;
            _cursorLeft = Console.CursorLeft;

            // Parse command line and execute a CopyAsyncAndReturnExitCode in case lexical parsing is successful
            var result = Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    CopyAsyncAndReturnExitCode,
                    a => 1);
#if DEBUG
            Console.WriteLine("Press any key...");
            Console.ReadKey();
#endif
            return result;
        }

        #endregion

        #region Private Static Methods

        private static void ConsoleWriteLine(string value, int line)
        {
            Console.SetCursorPosition(_cursorLeft, _cursorTop + line);
            Console.WriteLine(value);
        }

        private static async Task CopyAsync(Options options)
        {
            var factory = new S3ToAzureCopyJobFactory(options, options, options, options, options);
            var copyJobRunner = new CopyJobRunner(factory, options.ThreadCount);
            copyJobRunner.ProgressChanged += OnProgressChanged;
            await copyJobRunner.CopyAsync();
            if (copyJobRunner.HasErrors)
            {
                throw new Exception("Copy operations finished with errors");
            }
        }

        private static int CopyAsyncAndReturnExitCode(Options options)
        {
            try
            {
                // Execute an asynchronous operation and wait for result
                Task.Run(async () => await CopyAsync(options)).Wait();
                ConsoleWriteLine("DONE", Lines.Count + 1);
                return 0;
            }
            catch (AggregateException e)
            {
                ConsoleWriteLine("FAILED", Lines.Count + 1);
                Console.WriteLine(e.InnerExceptions.First().Message);
            }

            return 1;
        }

        private static void OnProgressChanged(object sender, JobProgressEventArgs args)
        {
            lock (Lines)
            {
                if (!Lines.ContainsKey(args.TargetPath))
                {
                    Lines.Add(args.TargetPath, Lines.Count);
                }

                var text = $"{args.TargetPath} : " + (string.IsNullOrEmpty(args.Error)
                    ? $"{(int) args.Percentage}%"
                    : $"ERROR ({args.Error})");
                ConsoleWriteLine(text, Lines[args.TargetPath]);
            }
        }

        #endregion
    }
}