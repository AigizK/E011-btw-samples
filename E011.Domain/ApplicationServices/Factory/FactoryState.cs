using System.Collections.Generic;
using System.Linq;

namespace E011
{
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
}