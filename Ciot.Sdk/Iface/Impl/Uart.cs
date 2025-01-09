using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Decoder;
using Ciot.Sdk.Protos.V2;
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

        public Uart(IfaceInfo info)
        {
            decoder = new DecoderS();
            status = new UartStatus();
            port = new SerialPort();
            Info = info;
        }

        public Err Start(UartCfg cfg)
        {
            this.cfg = cfg;
            port.PortName = $"COM{cfg.Num}";
            port.BaudRate = cfg.BaudRate;
            port.DtrEnable = cfg.Dtr;
            port.ReadTimeout = 5000;
            port.WriteTimeout = 5000;
            try
            {
                port.Open();
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
                Console.WriteLine("Enviando dados");
                SendBytes(data);
                if (decoder != null)
                {
                    byte[] result;
                    Console.WriteLine("Processando dados");
                    while (decoder.Decode((byte)port.ReadByte(), out result) == false) ;
                    return await Task.FromResult(result);
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
    }
}
