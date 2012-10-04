using E005_testing_use_cases;

namespace E011.Domain.ApplicationServices.Factory
{
    public abstract class factory_syntax : spec_syntax<FactoryId>
    {
        public TestBlueprintLibrary<FactoryId> Library;


        protected override void SetupServices()
        {
            Library = new TestBlueprintLibrary<FactoryId>();
        }

        protected override void ExecuteCommand(IEventStore store, ICommand<FactoryId> cmd)
        {
            new FactoryApplicationService(store,Library).Execute(cmd);
        }
    }
}
