using Microsoft.EntityFrameworkCore;
using TaskManager.API.Models;

namespace TaskManager.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Titulo)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.Descricao)
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue("Pendente");
            
            entity.Property(e => e.Prioridade)
                .IsRequired()
                .HasDefaultValue("Média");
            
            entity.Property(e => e.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
