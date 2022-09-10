using BenchmarkDotNet.Attributes;
using ConsoleApp.Persistence.EF.Context;
using Dapper;
using Dommel;
using EFVsDapperBattle;
using EFVsDapperBattle.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp.Tests;

[SimpleJob(
    BenchmarkDotNet.Engines.RunStrategy.ColdStart,
    BenchmarkDotNet.Jobs.RuntimeMoniker.Net60,
    launchCount: 1,
    targetCount: 50,
    id: "Read")]
[MemoryDiagnoser]
[MarkdownExporterAttribute.Default]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Read
{
    private string nome;
    private int rowsCount;

    private SqlConnection connection;
    private ApplicationDbContext context;

    private int GetRandomId() => new Random().Next(1, rowsCount);

    [GlobalSetup]
    public async Task Init()
    {
        Program.InitDapper();
        var dbContextOptions = Program.InitEf();

        context = new ApplicationDbContext(dbContextOptions);
        connection = new SqlConnection(ConnectionString.Default);
        rowsCount = await context.Wizzers.CountAsync();
        nome = await context.Wizzers.OrderBy(i => Guid.NewGuid()).Select(i => i.Nome).FirstAsync();
    }


    [Benchmark(Description = "EF Find")]
    public async Task EF_Select_Student_By_Id_Linq()
    {
        await context.Wizzers.FindAsync(GetRandomId());
    }

    [Benchmark(Description = "Dapper Find")]
    public async Task DP_Select_Student_By_Id_Linq()
    {
        await connection.GetAsync<Wizzer>(GetRandomId());
    }


    [Benchmark(Description = "EF SingleOrDefault RawSql")]
    public async Task EF_Select_Student_By_Id_RawSqwl()
    {
        await context.Wizzers.FromSqlRaw("SELECT * from wizzers WHERE id = {0}", GetRandomId()).SingleOrDefaultAsync();
    }

    [Benchmark(Description = "Dapper SingleOrDefault RawSql")]
    public async Task DP_Select_Student_By_Id()
    {
        await connection.QuerySingleOrDefaultAsync<Wizzer>("SELECT * from wizzers WHERE id = @Id", new { Id = GetRandomId() });
    }


    [Benchmark(Description = "EF Filter By Nome LinQ")]
    public async Task EF_FilterBy_FirstName_LinQ()
    {
        await context.Wizzers.AsNoTracking().Where(i => i.Nome == nome).ToListAsync();
    }


    [Benchmark(Description = "Dapper Filter By Nome LinQ")]
    public async Task DP_FilterBy_FirstName_LinQ()
    {
        (await connection.SelectAsync<Wizzer>(i => i.Nome == nome)).ToList();
    }


    [Benchmark(Description = "EF Filter By Nome RawSql")]
    public async Task EF_FilterBy_FirstName_RawSql()
    {
        await context.Wizzers.FromSqlRaw("SELECT * from wizzers WHERE Nome = {0}", nome).ToListAsync();
    }

    [Benchmark(Description = "Dapper Filter By Nome RawSql")]
    public async Task DP_FilterBy_FirstName_RawSql()
    {
        (await connection.QueryAsync<Wizzer>("SELECT * from wizzers WHERE Nome = @Nome", new { Nome = nome })).ToList();
    }



    [Benchmark(Description = "EF Get ALL")]
    public async Task EF_Select_Student_ALL()
    {
        await context.Wizzers.AsNoTracking().ToListAsync();
    }


    [Benchmark(Description = "Dapper Get ALL")]
    public async Task DP_Select_Student_ALL()
    {
        (await connection.GetAllAsync<Wizzer>()).ToList();
    }

}
