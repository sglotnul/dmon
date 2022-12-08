namespace Dmon.Model
{
    public interface IDeleteQueryProvider : IExecutable<int>
    {
        IDeleteQueryProvider Where(string field, object value);
    }
}