using Dapper.FluentMap.Dommel.Mapping;
using EFVsDapperBattle.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFVsDapperBattle.Data.DapperMa
{
    public class WizzerDapperMap : DommelEntityMap<Wizzer>
    {
        public WizzerDapperMap()
        {
            ToTable("Wizzers", "dbo");

            Map(p => p.Id).ToColumn("Id")
                            .IsKey()
                            .IsIdentity()
                            .SetGeneratedOption(DatabaseGeneratedOption.Computed);

            Map(i => i.Nome);
            Map(i => i.Email);
            Map(i => i.Endereco);
            Map(i => i.Telefone);
            Map(i => i.DataNascimento);
            
        }
    }
}
