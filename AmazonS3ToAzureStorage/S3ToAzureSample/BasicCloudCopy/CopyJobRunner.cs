// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyJobRunner.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace BasicCloudCopy
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Multi thread copy job runner executes jobs and tracks competition to start subsequent copy jobs according to
    ///     maximum thread amount limit.
    /// </summary>
    public class CopyJobRunner
    {
        #region Private Fields

        private readonly ICopyJobFactory _copyJobFactory;
        private readonly int _maxThreads;
        private readonly Dictionary<Task, ICopyJob> _tasks = new Dictionary<Task, ICopyJob>();

        #endregion

        #region Constructors

        public CopyJobRunner(ICopyJobFactory copyJobFactory, int maxThreads)
        {
            if (copyJobFactory == null)
            {
                throw new ArgumentNullException(nameof(copyJobFactory));
            }

            if (maxThreads <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThreads));
            }

            _copyJobFactory = copyJobFactory;
            _maxThreads = maxThreads;
        }

        #endregion

        #region Public Properties

        public bool HasErrors { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Executes a copying operation
        /// </summary>
        public async Task CopyAsync()
        {
            HasErrors = false;
            await StartTasksAsync();
            while (_tasks.Count > 0)
            {
                await Task.Delay(1000);
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnProgressChanged(JobProgressEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args?.Error))
                {
                    HasErrors = true;
                }

                ProgressChanged?.Invoke(this, args);
            }
            catch
            {
                ProgressChanged = null;
            }
        }

        #endregion

        #region Private Methods

        private async Task HandleTaskResultAsync(Task task)
        {
            var job = _tasks[task];
            _tasks.Remove(task);
            await StartTasksAsync();
            if (task.IsFaulted)
            {
                if (job.RequestRetry())
                {
                    _tasks.Add(job.CopyAsync(), job);
                }
                else
                {
                    throw new Exception("Retry amount is exceed the maximum limit");
                }
            }

            if (task.IsCanceled)
            {
                // TODO Handle canceled task here
            }
        }

        private async Task StartTasksAsync()
        {
            var threads = _maxThreads;
            do
            {
                if (_tasks.Count < threads)
                {
                    var job = await _copyJobFactory.CreateNextCopyJobAsync();
                    if (job != null)
                    {
                        var task = job.CopyAsync();
                        _tasks.Add(task, job);
                        job.ProgressChanged += (sender, args) => { OnProgressChanged(args); };
                        // Execute a task in a separate thread
                        task.ContinueWith(async t => { await HandleTaskResultAsync(t); });
                    }
                    else
                    {
                        threads = 0;
                    }
                }
                else
                {
                    break;
                }
            } while (threads > 0);
        }

        #endregion

        #region Other Members

        public event EventHandler<JobProgressEventArgs> ProgressChanged;

        #endregion
    }
}