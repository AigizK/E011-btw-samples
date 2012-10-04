namespace E011
{
    public interface ICarBlueprintLibrary
    {
        CarBlueprint TryGetBlueprintForModelOrNull(string modelName);
    }
}