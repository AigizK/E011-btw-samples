using System;

namespace E011
{
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