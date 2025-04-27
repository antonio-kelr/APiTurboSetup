using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;

namespace APiTurboSetup.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ProdutoImagem> ProdutoImagens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento entre Produto e Categoria
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Adicionando índice único ao campo Slug do Produto
            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // Configuração do relacionamento entre ProdutoImagem e Produto
            modelBuilder.Entity<ProdutoImagem>()
                .HasOne(pi => pi.Produto)
                .WithMany(p => p.Imagens)
                .HasForeignKey(pi => pi.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Semente de dados - categorias iniciais
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Eletrônicos" },
                new Categoria { Id = 2, Nome = "Roupas" },
                new Categoria { Id = 3, Nome = "Alimentos" }
            );
        }
    }
} 