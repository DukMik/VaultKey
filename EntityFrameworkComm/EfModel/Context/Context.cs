using EntityFrameworkComm.EfModel.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkComm.EfModel.Context;

public class Context : DbContext
{
    public DbSet<EncryptedData> EncryptedData { get; set; }
    public DbSet<Entrie> Entrie { get; set; }
    public DbSet<Log> Log { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Vault> Vault { get; set; }
    
   
    private string DbPath { get; set; } = string.Empty;

    private void InitializeDbPath()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        DbPath = Path.Combine(baseDirectory, "KeyLock.db");
        Console.WriteLine($"Le fichier sera créé à : {DbPath}");
    }

    public Context()
    {
        InitializeDbPath();
    }
    
    public Context(DbContextOptions<Context> options) : base(options)
    {
        InitializeDbPath();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}