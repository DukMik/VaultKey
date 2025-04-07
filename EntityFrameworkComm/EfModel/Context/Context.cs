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
        //var folder = Environment.SpecialFolder.LocalApplicationData;
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var path = Environment.GetFolderPath(folder);
        DbPath = Path.Combine(baseDirectory, "KeyLock.db");
        //DbPath = Path.Join(path, "blogging.db");
        
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
    
    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}