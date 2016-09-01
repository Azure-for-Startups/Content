// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICopyJobFactory.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    #region Usings

    using System.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Basic interface for the copy job factory.
    /// </summary>
    public interface ICopyJobFactory
    {
        #region Public Methods

        /// <summary>
        ///     Creates a subsequent copy job
        /// </summary>
        /// <returns>Returns null in case no new job can be created</returns>
        Task<ICopyJob> CreateNextCopyJobAsync();

        #endregion
    }
}