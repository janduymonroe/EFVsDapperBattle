using BenchmarkDotNet.Running;
using ConsoleApp.Tests;
using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using EFVsDapperBattle.Data.DapperMa;
using Microsoft.EntityFrameworkCore;

namespace EFVsDapperBattle;

public class Program
{
    public static void Main(string[] args)
    {

        //BenchmarkRunner.Run<Create>();
        //BenchmarkRunner.Run<Read>();
        //BenchmarkRunner.Run<Update>();
        //BenchmarkRunner.Run<Delete>();
    }

    public static void InitDapper()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        FluentMapper.Initialize(config =>
        {
            config.AddMap(new WizzerDapperMap());
            config.ForDommel();
        });
    }

    public static DbContextOptions InitEf()
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseSqlServer(ConnectionString.Default);
        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        return builder.Options;
    }
}