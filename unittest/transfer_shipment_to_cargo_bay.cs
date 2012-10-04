using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace E005_testing_use_cases.unittest
{
    public class transfer_shipment_to_cargo_bay : factory_syntax
    {
        [Test]
        public void empty_shipment()
        {
            Given(new FactoryOpened(FactoryId.ForTest),
                            new EmployeeAssignedToFactory(FactoryId.ForTest, "yoda"));
            When(new TransferShipmentToCargoBay(FactoryId.ForTest, "some shipment", new CarPart[0]));
            Expect("empty-InventoryShipments");
        }

        // Use Case: Empty Shipment and No Workers at the Factory
        [Test]
        public void empty_shipment_comes_to_empty_factory()
        {
            Given(new FactoryOpened(FactoryId.ForTest));
            When(new TransferShipmentToCargoBay(FactoryId.ForTest, "some shipment", new[] { new CarPart("chassis", 1) }));
            Expect("unknown-employee");
        }
        [Test]
        public void there_already_are_two_shipments()
        {
            Given(
                new FactoryOpened(FactoryId.ForTest),
                    new EmployeeAssignedToFactory(FactoryId.ForTest, "chubakka"),
                    new ShipmentTransferredToCargoBay(FactoryId.ForTest, new InventoryShipment("shipmt-11", new[] { new CarPart("engine", 3) })),
                    new ShipmentTransferredToCargoBay(FactoryId.ForTest, new InventoryShipment("shipmt-12", new[] { new CarPart("wheels", 40) }))
                );

            When(new TransferShipmentToCargoBay(FactoryId.ForTest, "shipmt-13", new[] { new CarPart("bmw6", 20) }));
            Expect("more-than-two-InventoryShipments");
        }
    }
}
