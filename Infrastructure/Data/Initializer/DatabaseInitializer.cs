using Application.Interfaces;
using Dapper;
using Infrastructure.Data.Initializer.Helpers;

namespace Infrastructure.Data.Initializer
{
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void InitializeDatabase()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        // Verifica se o esquema 'dbo' existe, e o cria caso não exista
                        var createSchemaSql = "CREATE SCHEMA IF NOT EXISTS dbo;";
                        connection.Execute(createSchemaSql, transaction: transaction);

                        // Verifica se a tabela existe
                        var tableExists = DatabaseHelper.TableExists(connection, "payment", transaction);

                        if (!tableExists)
                        {
                            // Comando SQL para criar a tabela
                            var createTableSql = @"
                            CREATE TABLE dbo.Payment (
                                id SERIAL PRIMARY KEY,
                                order_id int,
                                payment_method VARCHAR(50),
                                payment_status int,
                                payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	                            payment_date_processed TIMESTAMP,
	                            in_store_order_id VARCHAR(500),
	                            qr_data VARCHAR(500),
	                            order_number VARCHAR(50)
                            );";

                            // Executa o comando de criação da tabela
                            connection.Execute(createTableSql, transaction: transaction);

                            // Comando SQL para inserir dados iniciais
                            var seedDataSql = @"
                            INSERT INTO dbo.Payment (order_id, payment_method, payment_status, payment_date, payment_date_processed, in_store_order_id, qr_data, order_number) VALUES
                            (1, 'credit_card', 2, '2024-07-29 21:52:52.757', '2024-07-29 21:59:51.824', 'b2d0f023-acbc-4121-a4ce-f2d12c9edb51', '00020101021243650016COM.MERCADOLIBRE020130636b2d0f023-acbc-4121-a4ce-f2d12c9edb515204000053039865802BR5909Test Test6009SAO PAULO62070503***6304456D', '26368');";

                            // Executa o comando de inserção de dados
                            connection.Execute(seedDataSql, transaction: transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
