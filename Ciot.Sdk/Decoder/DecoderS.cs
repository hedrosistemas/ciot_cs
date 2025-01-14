using System;
using System.Collections.Generic;
using System.Text;

namespace Ciot.Sdk.Decoder
{
    public class DecoderS : IDecoder
    {
        const byte startch = (byte)'{';
        const byte endch = (byte)'}';
        readonly byte[] buffer;
        int size;
        int idx;

        public DecoderState State { get; private set; }

        public DecoderS(int size = 1024)
        {
            buffer = new byte[size];
        }

        public bool Decode(byte input, out byte[] output)
        {
            output = buffer;

            if (idx < buffer.Length)
            {
                buffer[idx++] = input;
            }
            else
            {
                idx = 0;
                State = DecoderState.WaitStart;
                return false;
            }

            switch (State)
            {
                case DecoderState.WaitStart:
                    if (input == startch)
                    {
                        idx = 0;
                        State = DecoderState.WaitSize;
                    }
                    break;
                case DecoderState.WaitSize:
                    if (idx == 2)
                    {
                        idx = 0;
                        size = buffer[1] << 8 | buffer[0];
                        if(size < buffer.Length)
                        {
                            State = DecoderState.Reading;
                        }
                        else
                        {
                            State = DecoderState.WaitStart;
                        }
                    }
                    break;
                case DecoderState.Reading:
                    if (idx == size + 1 && input == endch)
                    {
                        idx = 0;
                        State = DecoderState.WaitStart;
                        output = new byte[size];
                        Array.Copy(buffer, output, output.Length);
                        return true;
                    }
                    if (idx == size + 1)
                    {
                        idx = 0;
                        State = DecoderState.WaitStart;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        public virtual byte[] Encode(byte[] data)
        {
            var bytes = new List<byte>();
            bytes.Add(startch);
            bytes.AddRange(BitConverter.GetBytes((ushort)data.Length));
            bytes.AddRange(data);
            bytes.Add(endch);
            return bytes.ToArray();
        }

        public void Reset()
        {
            idx = 0;
            State = DecoderState.WaitStart;
        }
    }
}
