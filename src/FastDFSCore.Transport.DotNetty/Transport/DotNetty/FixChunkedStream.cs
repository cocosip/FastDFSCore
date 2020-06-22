// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace FastDFSCore.Transport.DotNetty
{
    using global::DotNetty.Buffers;
    using global::DotNetty.Handlers.Streams;
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>DotNetty 修复后的ChunkedStream
    /// </summary>
    public class FixChunkedStream : IChunkedInput<IByteBuffer>
    {
        /// <summary>默认Chunk大小
        /// </summary>
        public static readonly int DefaultChunkSize = 8192;

        readonly Stream input;
        readonly int chunkSize;
        bool closed;

        /// <summary>Ctor
        /// </summary>
        public FixChunkedStream(Stream input) : this(input, DefaultChunkSize)
        {
        }

        /// <summary>Ctor
        /// </summary>
        public FixChunkedStream(Stream input, int chunkSize)
        {
            Contract.Requires(input != null);
            Contract.Requires(chunkSize > 0);

            this.input = input;
            this.chunkSize = chunkSize;
        }

        /// <summary>TransferredBytes
        /// </summary>
        public long TransferredBytes { get; private set; }

        /// <summary>流是否已经读取结束
        /// </summary>
        public bool IsEndOfInput => this.closed || (this.input.Position == this.input.Length);

        /// <summary>Close
        /// </summary>
        public void Close()
        {
            this.closed = true;
            this.input.Dispose();
        }

        /// <summary>ReadChunk
        /// </summary>
        public IByteBuffer ReadChunk(IByteBufferAllocator allocator)
        {
            if (this.IsEndOfInput)
            {
                return null;
            }

            long availableBytes = this.input.Length - this.input.Position;
            int readChunkSize = availableBytes <= 0
                ? this.chunkSize
                : (int)Math.Min(this.chunkSize, availableBytes);

            bool release = true;
            IByteBuffer buffer = allocator.Buffer(readChunkSize);
            try
            {
                // transfer to buffer
                var task = Task.Run<int>(() =>
                {
                    return buffer.SetBytesAsync(buffer.WriterIndex, this.input, readChunkSize, CancellationToken.None);
                });

                int count = task.Result;
                buffer.SetWriterIndex(buffer.WriterIndex + count);
                this.TransferredBytes += count;

                release = false;
            }
            finally
            {
                if (release)
                {
                    buffer.Release();
                }
            }

            return buffer;
        }

        /// <summary>Length
        /// </summary>
        public long Length => -1;

        /// <summary>Progress
        /// </summary>
        public long Progress => this.TransferredBytes;
    }
}
