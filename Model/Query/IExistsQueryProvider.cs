namespace Dmon.Model
{
    public interface IExistsQueryProvider : IExecutable<bool>
    {
        IExistsQueryProvider Where(string column, object value);
    }
}
