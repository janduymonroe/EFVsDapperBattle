using BenchmarkDotNet.Attributes;
using ConsoleApp.Persistence.EF.Context;
using Dapper;
using Dommel;
using EFVsDapperBattle;
using EFVsDapperBattle.DataGenerator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp.Tests;

[SimpleJob(
    BenchmarkDotNet.Engines.RunStrategy.ColdStart,
    BenchmarkDotNet.Jobs.RuntimeMoniker.Net60,
    launchCount: 10,
    targetCount: 100,
    id: "Create")]
[MarkdownExporterAttribute.Default]
[MemoryDiagnoser]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Create
{

    private readonly string rawSqlDP = @"INSERT INTO Wizzers (Nome, Email, Endereco, Telefone, DataNascimento) 
                                            VALUES (@Nome, @Email, @Endereco, @Telefone, @DataNascimento)";

    private readonly string rawSqlEF = @"INSERT INTO Wizzers (Nome, Email, Endereco, Telefone, DataNascimento) 
                                            VALUES ({0}, {1}, {2}, {3}, {4})";

    private SqlConnection connection;
    private ApplicationDbContext context;

    [GlobalSetup]
    public void Init()
    {
        Program.InitDapper();
        var dbContextOptions = Program.InitEf();

        connection = new SqlConnection(ConnectionString.Default);
        context = new ApplicationDbContext(dbContextOptions);

        context.Wizzers.Count();
    }

    [Benchmark(Description = "EntityFramework")]
    public async Task InsertEF()
    {
        var wizzer = WizzerGenerator.GetWizzer();

        await context.AddAsync(wizzer);
        await context.SaveChangesAsync();
    }


    [Benchmark(Description = "Dapper")]
    public async Task InsertDP()
    {
        var wizzer = WizzerGenerator.GetWizzer();

        await connection.InsertAsync(wizzer);
    }


    [Benchmark(Description = "EntityFramework Raw SQL")]
    public async Task InsertEFRawSQL()
    {
        var wizzer = WizzerGenerator.GetWizzer();

        await context.Database.ExecuteSqlRawAsync(rawSqlEF, wizzer.Nome, wizzer.Email, wizzer.Endereco, wizzer.Telefone, wizzer.DataNascimento);
    }

    [Benchmark(Description = "Dapper Raw SQL")]
    public async Task InsertDPRawSQL()
    {
        var wizzer = WizzerGenerator.GetWizzer();

        await connection.ExecuteAsync(rawSqlDP, wizzer);
    }
}
