using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace E011
{
    [DataContract, Serializable]
    public sealed class InventoryShipment
    {
        public string Name { get; private set; }
        public CarPart[] Cargo { get; private set; }

        public InventoryShipment(string name, CarPart[] cargo)
        {
            Name = name;
            Cargo = cargo;
        }
    }
}
