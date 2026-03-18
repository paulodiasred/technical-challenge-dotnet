using Dapper;
using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Execute("CREATE TABLE IF NOT EXISTS contacorrente ( " +
                               "idcontacorrente TEXT(37) PRIMARY KEY," +
                               "numero INTEGER(10) NOT NULL UNIQUE," +
                               "nome TEXT(100) NOT NULL," +
                               "ativo INTEGER(1) NOT NULL default 0," +
                               "CHECK(ativo in (0, 1)) " +
                               ");");

            connection.Execute("CREATE TABLE IF NOT EXISTS movimento ( " +
                               "idmovimento TEXT(37) PRIMARY KEY," +
                               "idcontacorrente TEXT(37) NOT NULL," +
                               "datamovimento TEXT(25) NOT NULL," +
                               "tipomovimento TEXT(1) NOT NULL," +
                               "valor REAL NOT NULL," +
                               "CHECK(tipomovimento in ('C', 'D')), " +
                               "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente) " +
                               ");");

            connection.Execute("CREATE TABLE IF NOT EXISTS idempotencia ( " +
                               "chave_idempotencia TEXT(100) PRIMARY KEY," +
                               "requisicao TEXT NOT NULL," +
                               "resultado TEXT NOT NULL" +
                               ");");

            var hasAccounts = connection.ExecuteScalar<long>("SELECT COUNT(1) FROM contacorrente;");
            if (hasAccounts > 0)
            {
                return;
            }

            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('B6BAFC09-6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);");
        }
    }
}
