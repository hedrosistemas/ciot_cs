using System;
using System.Collections.Generic;
using System.Text;

namespace Ciot.Sdk.Decoder
{
    public enum DecoderState
    {
        WaitStart,
        WaitSize,
        Reading,
    }

    public interface IDecoder
    {
        DecoderState State { get; }
        bool Decode(byte input, out byte[] output);
        byte[] Encode(byte[] data);
        void Reset();
    }
}
