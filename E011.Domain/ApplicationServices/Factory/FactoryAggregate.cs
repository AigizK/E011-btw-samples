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
                throw DomainError.Named("created-factory", "Factory was already created");
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
    public interface ICarBlueprintLibrary
    {
        CarBlueprint TryGetBlueprintForModelOrNull(string modelName);
    }

  
    public class MySimpleBlueprintLibrary : ICarBlueprintLibrary
    {
        public CarBlueprint TryGetBlueprintForModelOrNull(string modelName)
        {
            switch (modelName)
            {
                case "Ford":
                    return new CarBlueprint("ford fiesta", new[] { new CarPart("chassis", 1), new CarPart("wheels", 4), new CarPart("engine", 1), });
                case "audi":
                    return new CarBlueprint("audi a8", new[] { new CarPart("chassis", 1), new CarPart("wheels", 4), new CarPart("engine", 1), new CarPart("battery", 2), });
                default:
                    return null;
            }
        }
    }

    public class CarBlueprint
    {
        public readonly string DesignName;
        public readonly CarPart[] RequiredParts;

        public CarBlueprint(string designName, CarPart[] requiredParts)
        {
            DesignName = designName;
            RequiredParts = requiredParts;
        }
    }

    // FactoryState is a new class we added in this E005 sample to keep track of Factory state.
    // This moves the location of where Factory state is stored from the Factory class itself
    // to its own dedicated state class.  This is helpful because we can mark the
    // the state class properties as variables that cannot be modified outside of the FactoryState class.
    // (readonly, for example, is how we declared an instance of FactoryState at the top of this file)
    // (and the ListOfEmployeeNames and ShipmentsWaitingToBeUnloaded lists below are also declared as readonly)
    // This helps to ensure that you can ONLY MODIFY THE STATE OF THE FACTORY BY USING EVENTS that are known to have happened.

    public class FactoryState
    {
        public FactoryState(IEnumerable<IEvent> events)
        {
            // this will load and replay the "list" of all the events that are passed into this contructor
            // this brings this FactoryState instance up to date with 
            // all events that have EVER HAPPENED to its associated Factory aggregate entity
            foreach (var @event in events)
            {
                // call my public Mutate method (defined below) to get my state up to date
                Mutate(@event);
            }
        }

        // lock our state changes down to only events that can modify these lists
        public readonly List<string> ListOfEmployeeNames = new List<string>();
        public readonly Dictionary<string, InventoryShipment> ShipmentsWaitingToBeUnloaded = new Dictionary<string, InventoryShipment>();
        private readonly Dictionary<string, int> AvailableParts = new Dictionary<string, int>();
        public readonly List<string> CreatedCars = new List<string>();
        public FactoryId Id { get; private set; }

        public int GetNumberOfAvailablePartsQuantity(string name)
        {
            return AvailableParts.ContainsKey(name) ? AvailableParts[name] : 0;
        }

        void When(FactoryOpened e)
        {
            Id = e.Id;
        }

        // announcements inside the factory
        void When(EmployeeAssignedToFactory e)
        {
            ListOfEmployeeNames.Add(e.EmployeeName);
        }
        void When(ShipmentTransferredToCargoBay e)
        {
            ShipmentsWaitingToBeUnloaded.Add(e.Shipment.Name, new InventoryShipment(e.Shipment.Name, e.Shipment.Cargo));
        }
        void When(CurseWordUttered e)
        {

        }

        void When(UnloadedFromCargoBay e)
        {
            foreach (var shipmentInCargoBay in e.InventoryShipments)
            {
                ShipmentsWaitingToBeUnloaded.Remove(shipmentInCargoBay.Name);

                foreach (var part in shipmentInCargoBay.Cargo)
                {
                    if (!AvailableParts.ContainsKey(part.Name))
                        AvailableParts.Add(part.Name, part.Quantity);
                    else
                        AvailableParts[part.Name] += part.Quantity;
                }
            }
        }

        void When(CarProduced e)
        {
            CreatedCars.Add(e.CarModel);

            foreach (var carPart in e.Parts)
            {
                var removed = carPart.Quantity;

                var quantitied = GetNumberOfAvailablePartsQuantity(carPart.Name);
                if (quantitied > 0)
                    AvailableParts[carPart.Name] = quantitied > removed ? quantitied - removed : 0;
            }

            var emptyPartKeys = AvailableParts.Where(x => x.Value == 0).Select(x => x.Key).ToList();

            foreach (var emptyPartKey in emptyPartKeys)
            {
                AvailableParts.Remove(emptyPartKey);
            }
        }

        // This is the very important Mutate method that provides the only public
        // way for factory state to be modified.  Mutate ONLY ACCEPTS EVENTS that have happened.
        // It then CHANGES THE STATE of the factory by calling the methods above
        // that wrap the readonly state variables that should be modified only when the associated event(s)
        // that they care about have occured.
        public void Mutate(IEvent e)
        {
            // we also announce this event inside of the factory.
            // this way, all workers will immediately know
            // what is going on inside the factory.  We are telling the compiler
            // to call one of the "When" methods defined above.
            // The "dynamic" syntax below is just a shortcut we are using so we don't
            // have to create a large if/else block for a bunch of specific event types.
            // This shortcut "dynamic" syntax means:
            // "Call this FactoryState's instance of the When method
            // that has a method signature of:
            // When(WhateverTheCurrentTypeIsOfThe-e-EventThatWasPassedIntoMutate)".
            ((dynamic)this).When((dynamic)e);
        }


    }
    
    

    public sealed class FactoryApplicationService : IFactoryApplicationService, IApplicationService
    {
        private readonly IEventStore _eventStore;
        private readonly ICarBlueprintLibrary _library;

        public FactoryApplicationService(IEventStore eventStore, ICarBlueprintLibrary library)
        {
            _eventStore = eventStore;
            _library = library;
        }

        public void Execute(object command)
        {
            ((dynamic)this).When((dynamic)command);
        }

        void Update(ICommand<FactoryId> c, Action<FactoryAggregate> action)
        {
            var eventStream = _eventStore.LoadEventStream(c.Id);
            var state = new FactoryState(eventStream.Events);
            var agg = new FactoryAggregate(state);
            action(agg);
            _eventStore.AppendEventsToStream(c.Id, eventStream.StreamVersion, agg.Changes);
        }

        public void When(ProduceCar c)
        {
            Update(c, ar => ar.ProduceCar(c.EmployeeName, c.CarModel, _library));
        }

        public void When(AssignEmployeeToFactory c)
        {
            Update(c, ar => ar.AssignEmployeeToFactory(c.EmployeeName));
        }

        public void When(CurseWordUttered c)
        {
            //throw new NotImplementedException();
        }

        public void When(TransferShipmentToCargoBay c)
        {
            Update(c,
                   ar =>
                   ar.TransferShipmentToCargoBay(c.ShipmentName, new InventoryShipment(c.ShipmentName, c.Parts)));
        }

        public void When(UnloadShipmentFromCargoBay c)
        {
            Update(c, ar => ar.UnloadShipmentFromCargoBay(c.EmployeeName));
        }

        public void When(OpenFactory c)
        {
            //throw new NotImplementedException();
        }
    }
}