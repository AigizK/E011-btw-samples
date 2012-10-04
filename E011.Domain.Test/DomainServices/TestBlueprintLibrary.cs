using System.Collections.Generic;
using System.Linq;

namespace E011.Domain.DomainServices
{
    public class TestBlueprintLibrary<T> : ICarBlueprintLibrary where T : IIdentity
    {

        IDictionary<string, CarPart[]> _knownDesigns = new Dictionary<string, CarPart[]>();

        public IEvent<T> RecordBlueprint(string name, params CarPart[] parts)
        {
            return new SpecSetupEvent<T>(() => _knownDesigns.Add(name, parts), "Registered car design '{0}' with following requirements:\r\n{1} ", name, parts.Aggregate("", (s, c) => string.Format("{0}\r\n\t{1}: {2}", s, c.Name, c.Quantity)));
        }

        public CarBlueprint TryGetBlueprintForModelOrNull(string modelName)
        {
            return _knownDesigns.ContainsKey(modelName) ? new CarBlueprint(modelName, _knownDesigns[modelName]) : null;
        }
    }
}