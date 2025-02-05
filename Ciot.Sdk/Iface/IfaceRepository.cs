using Ciot.Protos.V2;
using Ciot.Sdk.Common.Error;
using LanguageExt;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ciot.Sdk.Iface
{
    public class IfaceRepository : IIfaceRepository
    {
        private const string filename = "ifaces.json";

        public Either<ErrorBase, Msg> CreateIface(Msg msg)
        {
            var ifaces = GetIfacesDic();
            uint nextId = ifaces.Any() ? ifaces.Keys.Max() + 1 : 1;
            msg.Id = nextId;
            msg.Iface.Id = nextId;
            ifaces[nextId] = msg;
            SaveIfaces(ifaces.Values.ToList());
            return msg;
        }

        public Either<ErrorBase, Msg> DeleteIface(uint id)
        {
            var ifaces = GetIfacesDic();

            if (!ifaces.ContainsKey(id))
                return new ErrorNotFound($"Iface with ID {id} not found.");

            var deletedIface = ifaces[id];
            ifaces.Remove(id);
            SaveIfaces(ifaces.Values.ToList());

            return deletedIface;
        }

        public Either<ErrorBase, Msg> GetIfaceById(uint id)
        {
            var ifaces = GetIfacesDic();

            if(ifaces.TryGetValue(id, out var iface))
            {
                return iface;
            }
            else
            {
                return new ErrorNotFound($"Iface with ID {id} not found.");
            }
        }

        public Either<ErrorBase, List<Msg>> GetIfaces()
        {
            var ifaces = GetIfacesDic();
            return ifaces.Values.ToList();
        }

        public Either<ErrorBase, Msg> UpdateIface(Msg msg)
        {
            var ifaces = GetIfacesDic();

            if (!ifaces.ContainsKey(msg.Id))
                return new ErrorNotFound($"Iface with ID {msg.Id} not found.");

            ifaces[msg.Id] = msg;
            SaveIfaces(ifaces.Values.ToList());

            return msg;
        }

        private Dictionary<uint, Msg> GetIfacesDic()
        {
            if (!File.Exists(filename))
                return new Dictionary<uint, Msg>();

            var data = File.ReadAllText(filename);
            var ifaces = IfacesList.Parser.ParseJson(data).Items.ToList() ?? new List<Msg>();

            return ifaces.ToDictionary(iface => iface.Id);
        }

        private void SaveIfaces(List<Msg> ifaces)
        {
            var list = new IfacesList();
            list.Items.AddRange(ifaces);
            File.WriteAllText(filename, list.ToString());
        }
    }
}
