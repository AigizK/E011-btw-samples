using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace E011
{
    public class FactoryAggregate
    {
        // THE Factory Journal!
        // In the Episode 4 (E004) sample code
        // we named the Journal variable "JournalOfFactoryEvents"
        // Here, we change it to its more broadly applicable production name of "Changes"
        // It is still the in memory list where we "write down" the EVENTS that HAVE HAPPENED.
        public List<IEvent> Changes = new List<IEvent>();

        // Note that we have moved the place where we keep track of the current
        // state of the Factory.  In E004, Factory state was also inside of the Factory class itself.
        // Now, we have moved all Factory state into its own "FactoryState" class.
        readonly FactoryState _state;
        public FactoryAggregate(FactoryState state)
        {
            _state = state;
        }

        // internal "state" variables

        public void OpenFactory(FactoryId id)
        {
            if (_state.Id != null)
            {
                throw DomainError.Named("factory-already-created", "Factory was already created");
                return;

            }
            Apply(new FactoryOpened(id));
        }


        public void AssignEmployeeToFactory(string employeeName)
        {
            //Print("?> Command: Assign employee {0} to factory", employeeName);

            if (_state.ListOfEmployeeNames.Contains(employeeName))
            {
                // yes, this is really weird check, but this factory has really strict rules.
                // manager should've remembered that
                throw DomainError.Named("more than 1 person", ":> the name of '{0}' only one employee can have", employeeName);

                return;
            }

            if (employeeName == "bender")
            {
                throw DomainError.Named("bender-employee", ":> Guys with name 'bender' are trouble.");
                return;
            }

            DoPaperWork("Assign employee to the factory");
            Apply(new EmployeeAssignedToFactory(_state.Id, employeeName));
        }


        public void TransferShipmentToCargoBay(string shipmentName, InventoryShipment shipment)
        {
            //Print("?> Command: transfer shipment to cargo bay");
            if (_state.ListOfEmployeeNames.Count == 0)
            {
                throw DomainError.Named("unknown-employee", ":> There has to be somebody at factory in order to accept shipment");
                return;
            }
            if (shipment.Cargo.Length == 0)
            {
                throw DomainError.Named("empty-InventoryShipments", ":> Empty InventoryShipments are not accepted!");
                return;
            }

            if (_state.ShipmentsWaitingToBeUnloaded.Count >= 2)
            {
                throw DomainError.Named("more-than-two-InventoryShipments", ":> More than two InventoryShipments can't fit into this cargo bay :(");
                return;
            }

            DoRealWork("opening cargo bay doors");
            Apply(new ShipmentTransferredToCargoBay(_state.Id, shipment));

            var totalCountOfParts = shipment.Cargo.Sum(p => p.Quantity);
            if (totalCountOfParts > 10)
            {
                Apply(new CurseWordUttered(_state.Id, "Boltov tebe v korobky peredach",
                                           "awe in the face of the amount of shipment delivered"));
            }
        }

        public void UnloadShipmentFromCargoBay(string employeeName)
        {
            //Print("?> Command: Unload Shipment From Cargo Bay");

            if (!_state.ListOfEmployeeNames.Contains(employeeName))
            {
                throw DomainError.Named("unknown-employee", ":> '{0}' not assigned to factory", employeeName);
                return;
            }

            if (_state.ShipmentsWaitingToBeUnloaded.Count == 0)
            {
                throw DomainError.Named("empty-InventoryShipments", ":> InventoryShipments not found");
                return;
            }

            DoRealWork("unload shipment");
            List<InventoryShipment> shipments = new List<InventoryShipment>();
            while (_state.ShipmentsWaitingToBeUnloaded.Count > 0)
            {
                var parts = _state.ShipmentsWaitingToBeUnloaded.First();
                shipments.Add(parts.Value);
                Apply(new UnloadedFromCargoBay(_state.Id, employeeName, shipments.ToArray()));

            }
        }

        public void ProduceCar(string employeeName, string carModel, ICarBlueprintLibrary library)
        {
            if (!_state.ListOfEmployeeNames.Contains(employeeName))
                throw DomainError.Named("unknown-employee", ":> '{0}' not assigned to factory", employeeName);

            var design = library.TryGetBlueprintForModelOrNull(carModel);

            if (design == null)
                throw DomainError.Named("car-model-not-found", "Model '{0}' not found", carModel);

            foreach (var part in design.RequiredParts)
            {
                if (_state.GetNumberOfAvailablePartsQuantity(part.Name) < part.Quantity)
                    throw DomainError.Named("part-not-found", ":> {0} not found", part.Name);

            }


            DoRealWork("produce car");

            var parts = new[] { new CarPart("chassis", 1), new CarPart("wheels", 4), new CarPart("engine", 1) };
            Apply(new CarProduced(_state.Id, employeeName, carModel, parts));
        }

        void DoPaperWork(string workName)
        {
            //Print(" > Work:  papers... {0}... ", workName);

        }
        void DoRealWork(string workName)
        {
            //Print(" > Work:  heavy stuff... {0}...", workName);

        }
        void Apply(IEvent e)
        {
            // we record by jotting down notes in our journal
            Changes.Add(e);
            // and also immediately change the state
            _state.Mutate(e);
        }
    }
    // domain service



    // FactoryState is a new class we added in this E005 sample to keep track of Factory state.
    // This moves the location of where Factory state is stored from the Factory class itself
    // to its own dedicated state class.  This is helpful because we can mark the
    // the state class properties as variables that cannot be modified outside of the FactoryState class.
    // (readonly, for example, is how we declared an instance of FactoryState at the top of this file)
    // (and the ListOfEmployeeNames and ShipmentsWaitingToBeUnloaded lists below are also declared as readonly)
    // This helps to ensure that you can ONLY MODIFY THE STATE OF THE FACTORY BY USING EVENTS that are known to have happened.

    
    
    

   
}