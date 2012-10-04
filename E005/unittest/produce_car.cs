using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace E005_testing_use_cases.unittest
{
    public class produce_car : factory_syntax
    {
        [Test]
        public void fry_not_assigned_to_factory()
        {
            Given(new FactoryOpened(FactoryId.ForTest));
            When(new ProduceCar(FactoryId.ForTest, "fry", "Ford"));

            Expect("unknown-employee");
        }

        [Test]
        public void part_not_found()
        {
            Given(
                     new FactoryOpened(FactoryId.ForTest),
                     Library.RecordBlueprint("Ford", new CarPart("chassis", 1)),
                     new EmployeeAssignedToFactory(FactoryId.ForTest, "fry")
                );
            When(new ProduceCar(FactoryId.ForTest, "fry", "Ford"));
            Expect("part-not-found");
        }

        [Test]
        public void cart_model_not_found()
        {
            Given(
                     new FactoryOpened(FactoryId.ForTest),
                     new EmployeeAssignedToFactory(FactoryId.ForTest, "fry")
                );
            When(new ProduceCar(FactoryId.ForTest, "fry", "Volvo"));
            Expect("car-model-not-found");
        }

        [Test]
        public void produced_car()
        {
            Given(
                    Library.RecordBlueprint("death star", new CarPart("magic box", 10)),
                    Library.RecordBlueprint("Ford", new CarPart("chassis", 1)),
                    new FactoryOpened(FactoryId.ForTest),
                    new EmployeeAssignedToFactory(FactoryId.ForTest, "fry"),
                    new UnloadedFromCargoBay(FactoryId.ForTest, "fry", new[] { new InventoryShipment("ship-1", new[] { new CarPart("chassis", 1), new CarPart("wheels", 4), new CarPart("engine", 1) }) })
                );

            When(new ProduceCar(FactoryId.ForTest, "fry", "Ford"));

            Expect(new CarProduced(FactoryId.ForTest,"fry", "Ford", new[] { new CarPart("chassis", 1), new CarPart("wheels", 4), new CarPart("engine", 1) }));
        }
    }
}
