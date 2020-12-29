using APIAutoFeeder.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace APIAutoFeeder.Services
{
    public class MySqlInitializer : IDatabaseInitializer<ApplicationDbContext>
    {
        public void InitializeDatabase(ApplicationDbContext context)
        {
            if (!context.Database.Exists())
            {
                // se a base não existe, então cria
                context.Database.Create();
            }
            else
            {
                // verifica se a tabela __MigrationHistory existe na base 
                var migrationHistoryTableExists = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<int>(
                  "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'autofeederapi' AND table_name = '__MigrationHistory'");

                // se não existe (primeira execução do CodeFirst) deleta a base e cria novamente
                if (migrationHistoryTableExists.FirstOrDefault() == 0)
                {
                    context.Database.Delete();
                    context.Database.Create();
                }
            }
        }
    }
}