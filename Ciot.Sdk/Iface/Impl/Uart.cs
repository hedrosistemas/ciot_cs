using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Decoder;
using LanguageExt;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Ciot.Sdk.Iface.Impl
{
    public class Uart : IIface
    {
        public event EventHandler<Event> OnEvent;
        public IfaceInfo Info { get; private set; }

        private readonly SerialPort port;
        private readonly IDecoder decoder;
        private UartCfg cfg;
        private UartStatus status;
        private TaskCompletionSource<byte[]> decode;

        private byte[] decoded;

        public Uart(IfaceInfo info)
        {
            decoder = new DecoderS();
            status = new UartStatus();
            port = new SerialPort();
            Info = info;
            decoded = new byte[1023];
            decode = new TaskCompletionSource<byte[]>();
            port.DataReceived += Port_DataReceived;
        }

        public Err Start(UartCfg cfg)
        {
            this.cfg = cfg;
            port.PortName = $"COM{cfg.Num}";
            port.BaudRate = (int)cfg.BaudRate;
            port.DtrEnable = cfg.Dtr;
            port.ReadTimeout = 5000;
            port.WriteTimeout = 5000;
            try
            {
                port.Open();
                port.ReadExisting();
                OnEvent?.Invoke(this, new Event
                {
                    Type = EventType.Started,
                });
                return Err.Ok;
            }
            catch (Exception)
            {
                return Err.Exception;
            }
        }

        public Err Stop()
        {
            try
            {
                port.Close();
                return Err.Ok;
            }
            catch (Exception)
            {
                return Err.Exception;
            }
        }

        public Err GetData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.Uart)
            {
                return Err.InvalidType;
            }

            switch (data.Uart.TypeCase)
            {
                case UartData.TypeOneofCase.Config:
                    data.Uart.Config = cfg;
                    break;
                case UartData.TypeOneofCase.Status:
                    data.Uart.Status = status;
                    break;
                default:
                    return Err.InvalidType;
            }

            return Err.Ok;
        }

        public Err ProcessData(MsgData data)
        {
            if (data.TypeCase != MsgData.TypeOneofCase.Uart)
            {
                return Err.InvalidType;
            }

            switch (data.Uart.TypeCase)
            {
                case UartData.TypeOneofCase.Config:
                    Start(data.Uart.Config);
                    break;
                case UartData.TypeOneofCase.Request:
                    return Err.NotImplemented;
                default:
                    return Err.InvalidType;
            }

            return Err.Ok;
        }

        public async Task<Either<ErrorBase, byte[]>> SendData(byte[] data)
        {
            try
            {
                if (!port.IsOpen) port.Open();
                port.ReadExisting();
                SendBytes(data);
                if (decoder != null)
                {
                    return await decode.Task;
                }
                else
                {
                    return new ErrorInternal("Invalid decoder");
                }
            }
            catch (Exception ex)
            {
                return new ErrorException(ex);
            }
        }

        public void SendBytes(byte[] bytes)
        {
            if(port.IsOpen)
            {
                var decoded = decoder.Encode(bytes);
                port.Write(decoded, 0, decoded.Length);
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[port.BytesToRead];
            port.Read(data, 0, data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                if (decoder.Decode(data[i], out decoded))
                {
                    if(decode.Task.IsCompleted == false)
                    {
                        decode.SetResult(decoded);
                    }
                    var ev = new Event
                    {
                        Type = EventType.Data,
                        Msg = Msg.Parser.ParseFrom(decoded),
                    };
                    OnEvent?.Invoke(this, ev);
                }
            }
        }
    }
}
