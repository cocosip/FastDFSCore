// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
 
namespace FastDFSCore.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using DotNetty.Buffers;
    using DotNetty.Handlers.Streams;

    public class FixChunkedStream : IChunkedInput<IByteBuffer>
    {
        public static readonly int DefaultChunkSize = 8192;

        readonly Stream input;
        readonly int chunkSize;
        bool closed;

        public FixChunkedStream(Stream input) : this(input, DefaultChunkSize)
        {
        }

        public FixChunkedStream(Stream input, int chunkSize)
        {
            Contract.Requires(input != null);
            Contract.Requires(chunkSize > 0);

            this.input = input;
            this.chunkSize = chunkSize;
        }

        public long TransferredBytes { get; private set; }

        public bool IsEndOfInput => this.closed || (this.input.Position == this.input.Length);

        public void Close()
        {
            this.closed = true;
            this.input.Dispose();
        }

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
                int count = AsyncHelper.RunSync(() =>
                {
                    return buffer.SetBytesAsync(buffer.WriterIndex, this.input, readChunkSize, CancellationToken.None);
                });
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

        public long Length => -1;

        public long Progress => this.TransferredBytes;
    }
}
