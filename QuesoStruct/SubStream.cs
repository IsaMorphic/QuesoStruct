using System;
using System.IO;

namespace QuesoStruct
{
    public class SubStream : Stream
    {
        public Stream Parent { get; }

        public long Offset { get; }

        public long AbsoluteOffset => (Parent is SubStream) ? (Parent as SubStream).AbsoluteOffset + Offset : Offset;
        public Stream Root => (Parent is SubStream) ? (Parent as SubStream).Root : Parent;

        public override long Length { get => _length; }
        private long _length;
        private bool _locked;

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                Parent.Seek(Offset + _position, SeekOrigin.Begin);
            }
        }
        private long _position;

        public SubStream(Stream parent, long offset)
        {
            Parent = parent;
            Offset = offset;
        }

        public void Lock()
        {
            if (_locked)
                throw new InvalidOperationException("Sub-stream is already locked.");
            _locked = true;
        }

        public override bool CanRead => Parent.CanRead;
        public override bool CanWrite => Parent.CanWrite;

        public override bool CanSeek => Parent.CanSeek;

        public override bool CanTimeout => Parent.CanTimeout;

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead;
            Seek(0, SeekOrigin.Current);
            if (_locked)
            {
                if (_position + count > Length)
                    bytesRead = Parent.Read(buffer, offset, (int)(Length - _position));
                else
                    bytesRead = Parent.Read(buffer, offset, count);

                _position += bytesRead;
            }
            else
            {
                bytesRead = Parent.Read(buffer, offset, count);
                SetLength(_position += bytesRead);
            }
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Seek(0, SeekOrigin.Current);
            Parent.Write(buffer, offset, count);
            if (_locked)
            {
                if (_position + count > Length)
                {
                    throw new InvalidOperationException("Attempted to write past the stream's boundaries");
                }
                else
                {
                    _position += count;
                }
            }
            else
            {
                SetLength(_position += count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            if (_locked)
                throw new InvalidOperationException("Sub-stream is locked and its length cannot be changed.");
            if (Offset + value > Parent.Length)
                Parent.SetLength(Offset + value);
            _length = value;
        }

        public override void Flush()
        {
            return;
        }
    }
}
