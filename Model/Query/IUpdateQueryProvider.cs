namespace Dmon.Model
{
    public interface IUpdateQueryProvider : IExecutable<int>
    {
        IUpdateQueryProvider Where(string field, object value);
    }
}