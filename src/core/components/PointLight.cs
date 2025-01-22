namespace Flatlanders.Core;

public class PointLight(Entity entity) : Light(entity)
{
    protected override Penumbra.Light CreateData()
    {
        return new Penumbra.PointLight();
    }
}