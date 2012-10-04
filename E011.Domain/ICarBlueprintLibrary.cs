namespace E011
{
    public interface ICarBlueprintLibrary
    {
        CarBlueprint TryGetBlueprintForModelOrNull(string modelName);
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

}