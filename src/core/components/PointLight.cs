namespace Flatlanders.Core.Components;

public class PointLight(Entity entity) : Light(entity)
{
    protected override Penumbra.Light CreateData()
    {
        return new Penumbra.PointLight();
    }
}