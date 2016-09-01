// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICopyJob.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    #region Usings

    using System;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Basic interface for single thread copy job
    /// </summary>
    public interface ICopyJob
    {
        #region Public Methods

        /// <summary>
        ///     Executes a copying operation
        /// </summary>
        Task CopyAsync();

        /// <summary>
        ///     Should be used to determine if the retry operation is possible in case previous copying job task had failed
        /// </summary>
        /// <returns>true in case retry operation is allowed</returns>
        bool RequestRetry();

        #endregion

        #region Other Members

        /// <summary>
        ///     Progress event
        /// </summary>
        event EventHandler<JobProgressEventArgs> ProgressChanged;

        #endregion
    }
}