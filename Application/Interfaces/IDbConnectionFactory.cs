using System.Data;

namespace Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
