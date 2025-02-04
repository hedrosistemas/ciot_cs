using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Ciot.Sdk.Iface
{
    public class IfaceRepository : IIfaceRepository
    {
        private const string filename = "ifaces.json";

        private List<IfaceInfo> ifaces;

        public IfaceRepository() 
        {
            if(File.Exists(filename))
            {
                var data = File.ReadAllText(filename);
                ifaces = JsonConvert.DeserializeObject<List<IfaceInfo>>(data);
            }
            else
            {
                ifaces = new List<IfaceInfo>();
            }
        }

        public Either<ErrorBase, IfaceInfo> CreateIface(IfaceType ifaceType)
        {
            var iface = new IfaceInfo
            {
                Id = (uint)ifaces.Count,
                Type = ifaceType,
            };
            ifaces.Add(iface);
            Save();
            return iface;
        }

        public Either<ErrorBase, IfaceInfo> GetIfaceById(uint id)
        {
            if(id < ifaces.Count)
            {
                return ifaces[(int)id];
            }
            else
            {
                return new ErrorNotFound();
            }
        }

        public Either<ErrorBase, List<IfaceInfo>> GetIfaces()
        {
            return ifaces;
        }

        public Either<ErrorBase, IfaceInfo> UpdateIface(uint id, IfaceType ifaceType)
        {
            if (id < ifaces.Count)
            {
                ifaces[(int)id].Id = id;
                ifaces[(int)id].Type = ifaceType;
                Save();
                return ifaces[(int)id];
            }
            else
            {
                return new ErrorNotFound();
            }
        }

        public Either<ErrorBase, IfaceInfo> DeleteIface(uint id)
        {
            if (id < ifaces.Count)
            {
                var iface = ifaces[(int)id];
                ifaces.RemoveAt((int)id);
                Save();
                return iface;
            }
            else
            {
                return new ErrorNotFound();
            }
        }

        private void Save()
        {
            var data = JsonConvert.SerializeObject(ifaces);
            File.WriteAllText(filename, data);
        }
    }
}
