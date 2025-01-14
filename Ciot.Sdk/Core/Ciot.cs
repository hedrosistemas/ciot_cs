using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using Ciot.Sdk.Iface;
using LanguageExt;
using System;
using System.Threading.Tasks;

namespace Ciot.Sdk.Core
{
    public class Ciot : IIface
    {
        public event EventHandler<Event> OnEvent;
        public IfaceInfo Info { get; private set; }

        private readonly IIfaceRepository ifaceRepository;

        private Status status;
        private Task task;

        public Ciot(IIfaceRepository ifaceRepository, IfaceInfo info) 
        { 
            this.ifaceRepository = ifaceRepository;
            
            Info = info;
            status = new Status();
            task = new Task(Task);

            task.Start();
        }

        public Err Stop()
        {
            return Err.NotImplemented;
        }

        public Err GetData(MsgData data)
        {
            return Err.NotImplemented;
        }

        public Err ProcessData(MsgData data)
        {
            return Err.NotImplemented;
        }

        public Task<Either<ErrorBase, byte[]>> SendData(byte[] data)
        {
            throw new NotImplementedException();
        }

        private void Task()
        {
            while (true)
            {
                switch (status.State)
                {
                    case State.Idle:
                        break;
                    case State.Starting:
                        break;
                    case State.Started:
                        break;
                    case State.Busy:
                        break;
                    case State.Error:
                        break;
                    default:
                        break;
                }
            }
        }

        private void StartingTask()
        {

        }
    }
}
