using BenchmarkDotNet.Attributes;
using ConsoleApp.Persistence.EF.Context;
using Dapper;
using Dommel;
using EFVsDapperBattle;
using EFVsDapperBattle.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Perfolizer.Mathematics.SignificanceTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp.Tests;

[SimpleJob(
    BenchmarkDotNet.Engines.RunStrategy.ColdStart,
    BenchmarkDotNet.Jobs.RuntimeMoniker.Net60,
    launchCount: 1,
    targetCount: 100,
    id: "Update")]
[MemoryDiagnoser]
[MarkdownExporterAttribute.Default]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Update
{
    private List<Wizzer> wizzerList;
    private SqlConnection connection;
    private ApplicationDbContext context;

    private readonly string rawSqlDP = @"UPDATE wizzers SET Nome = @Nome WHERE Id = @Id";
    private readonly string rawSqlEF = @"UPDATE wizzers SET Nome = {1} WHERE Id = {0}";

    private int rowsCount;


    private Wizzer GetRandomWizzer()
    {
        var wizzer = wizzerList.OrderBy(i => Guid.NewGuid()).First();
        wizzerList.Remove(wizzer);
        return wizzer;
    }

    private int GetRandomId() => new Random().Next(1, rowsCount);

    [GlobalSetup]
    public async Task Init()
    {
        Program.InitDapper();
        var dbContextOptions = Program.InitEf();
        connection = new SqlConnection(ConnectionString.Default);
        context = new ApplicationDbContext(dbContextOptions);
        rowsCount = await context.Wizzers.CountAsync();
        wizzerList = await context.Wizzers.OrderBy(i => Guid.NewGuid()).Take(1000).ToListAsync();
    }

    [Benchmark(Description = "EF One Update")]
    public async Task UpdateSingleEF()
    {
        var wizzer = GetRandomWizzer();

        wizzer.Nome = wizzer.Nome.ToUpper();
        context.Update(wizzer);
        await context.SaveChangesAsync();
    }

    [Benchmark(Description = "Dapper One Update")]
    public async Task UpdateSingleDP()
    {
        var wizzer = GetRandomWizzer();
        wizzer.Nome = wizzer.Nome.ToUpper();
        await connection.UpdateAsync(wizzer);
    }

    [Benchmark(Description = "EF One Update RawSQL")]
    public async Task UpdateSingleEFRaw()
    {
        await context.Database.ExecuteSqlRawAsync(rawSqlEF, GetRandomId(), "Jose das Candongas");
    }

    [Benchmark(Description = "Dapper One Update RawSQL")]
    public async Task UpdateSingleDPRaw()
    {
        await connection.ExecuteAsync(rawSqlDP, new { Nome = "Jose das Candongas", Id = GetRandomId() });
    }

}
