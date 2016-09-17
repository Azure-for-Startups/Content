// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForwardOnlyReadStream.cs" company="Microsoft">
//   Copyright (c) 2016 Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BasicCloudCopy
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    public class ProgressEventArgs: EventArgs
    {
        #region Public Properties

        public string Error { get; internal set; }

        public double Percentage
        {
            get
            {
                return TotalLength <= 0
                    ? 100.0
                    : Position*100.0/TotalLength;
            }
        }

        public long Position { get; internal set; }
        public long TotalLength { get; internal set; }

        #endregion
    }

    public class JobProgressEventArgs: ProgressEventArgs
    {
        #region Public Properties

        public string TargetPath { get; internal set; }

        #endregion
    }

    public class ForwardOnlyReadStream: Stream
    {
        #region Private Fields

        private readonly Stream _stream;
        private readonly long _totalLength;
        private long _position;

        #endregion

        #region Constructors

        public ForwardOnlyReadStream(Stream stream, long totalLength)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (totalLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalLength));
            }

            _stream = stream;
            _totalLength = totalLength;
            _position = 0;
        }

        #endregion

        #region Public Properties

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _totalLength; }
        }

        public override long Position
        {
            get { return _position; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Public Methods

        public override void Close()
        {
            _stream.Close();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var error = "";
            var result = 0;
            try
            {
                result = _stream.Read(buffer, offset, count);
                _position += result;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                throw;
            }
            finally
            {
                OnProgressChanged(
                    new ProgressEventArgs
                    {
                        Position = _position,
                        TotalLength = _totalLength,
                        Error = error
                    });
            }

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            _stream.Dispose();
            base.Dispose(disposing);
        }

        protected virtual void OnProgressChanged(ProgressEventArgs args)
        {
            try
            {
                ProgressChanged?.Invoke(this, args);
            }
            catch
            {
                ProgressChanged = null;
            }
        }

        #endregion

        #region Other Members

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        #endregion
    }
}