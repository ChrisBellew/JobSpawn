namespace JobSpawn.Proxy
{
    public interface IProxyBuilder
    {
        TContract BuildProxy<TContract>();
    }
}