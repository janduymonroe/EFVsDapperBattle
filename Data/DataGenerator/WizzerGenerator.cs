using Bogus;
using EFVsDapperBattle.Entity;
using System.Collections.Generic;
using System.Linq;

namespace EFVsDapperBattle.DataGenerator;

public class WizzerGenerator
{
    public static ICollection<Wizzer> GetWizzers(int qt)
    {
        var wizzerFaker = new Faker<Wizzer>("pt_BR")
            .RuleFor(w => w.Nome, f => f.Person.FullName)
            .RuleFor(w => w.Email, f => f.Person.Email)
            .RuleFor(w => w.Endereco, f => f.Address.FullAddress())
            .RuleFor(w => w.Telefone, f => f.Person.Phone)
            .RuleFor(w => w.DataNascimento, f => f.Person.DateOfBirth.Date);

        return wizzerFaker.Generate(qt);
    }

    public static Wizzer GetWizzer()
    {
        return GetWizzers(1).First();
    }
}
