﻿using System.Linq;
using System.Text;

namespace E011
{
    public partial class Describe
    {
        string When(ShipmentTransferredToCargoBay e)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Shipment '{0}' transferred to cargo bay:",e.Shipment.Name).AppendLine();
            foreach (var carPart in e.Shipment.Cargo)
            {
                builder.AppendFormat("     {0} {1} pcs", carPart.Name, carPart.Quantity).AppendLine();
            }
            return builder.ToString();
        }

        string When(TransferShipmentToCargoBay e)
        {
            return string.Format(@"Transfer shipment '{0}' to cargo bay:{1}"
                    , e.ShipmentName
                    , e.Parts.Aggregate("", (x, y) => x + string.Format("\r\n{0}:{1}", y.Name, y.Quantity))
                );
        }

        string When(CarProduced e)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("produce car {0}:", e.CarModel).AppendLine();
            foreach (var carPart in e.Parts)
                builder.AppendFormat("     {0} {1} pcs", carPart.Name, carPart.Quantity).AppendLine();

            return builder.ToString();

        }

        string When(UnloadedFromCargoBay e)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0} unload:", e.EmployeeName).AppendLine();
            foreach (var inventoryShipment in e.InventoryShipments)
            {
                builder.AppendFormat("\tshipment name '{0}' and parts:\r\n", inventoryShipment.Name);
                foreach (var carPart in inventoryShipment.Cargo)
                    builder.AppendFormat("\t\t{0} {1} pcs", carPart.Name, carPart.Quantity).AppendLine();
            }

            return builder.ToString();
        }
    }
}