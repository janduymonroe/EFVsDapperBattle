using BenchmarkDotNet.Attributes;
using ConsoleApp.Persistence.EF.Context;
using Dapper;
using Dommel;
using EFVsDapperBattle;
using EFVsDapperBattle.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp.Tests;

[SimpleJob(
    BenchmarkDotNet.Engines.RunStrategy.ColdStart,
    BenchmarkDotNet.Jobs.RuntimeMoniker.Net60,
    launchCount: 2,
    targetCount: 50,
    id: "Delete")]
[MemoryDiagnoser]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Delete
{
    private List<Wizzer> wizzersList;
    private SqlConnection connection;
    private ApplicationDbContext context;
    private int rowsCount;

    private readonly string rawSqlDP = @"DELETE FROM wizzers WHERE Id = @Id";
    private readonly string rawSqlEF = @"DELETE FROM wizzers WHERE Id = {0}";

    private Wizzer GetRandomStudent()
    {
        var wizzers = wizzersList.OrderBy(i => Guid.NewGuid()).First();
        wizzersList.Remove(wizzers);
        return wizzers;
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
        wizzersList = await context.Wizzers.OrderBy(i => Guid.NewGuid()).Take(1000).ToListAsync();
    }

    [Benchmark(Description = "EF One Delete")]
    public async Task DeleteSingleEF()
    {
        var wizzer = GetRandomStudent();

        context.Wizzers.Remove(wizzer);
        await context.SaveChangesAsync();
    }

    [Benchmark(Description = "Dapper One Delete")]
    public async Task DeleteSingleDP()
    {
        var wizzer = GetRandomStudent();
        await connection.DeleteAsync(wizzer);
    }


    [Benchmark(Description = "EF One Delete RawSQL")]
    public async Task DeleteSingleEFRaw()
    {
        await context.Database.ExecuteSqlRawAsync(rawSqlEF, GetRandomId());
    }

    [Benchmark(Description = "Dapper One Delete RawSQL")]
    public async Task DeleteSingleDPRaw()
    {
        await connection.ExecuteAsync(rawSqlDP, new { Id = GetRandomId() });
    }

}
