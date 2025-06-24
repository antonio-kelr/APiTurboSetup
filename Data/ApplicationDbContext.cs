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
        public DbSet<User> Users { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<ItemCarrinho> ItensCarrinho { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<TrocaEmail> TrocasEmail { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }

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

            // Configuração da tabela Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Senha)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.Telefone)
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(20);

            // Configuração do relacionamento entre User e Endereco
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enderecos)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Semente de dados - categorias iniciais
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Eletrônicos" },
                new Categoria { Id = 2, Nome = "Roupas" },
                new Categoria { Id = 3, Nome = "Alimentos" }
            );

            // Configurações do Carrinho
            modelBuilder.Entity<Carrinho>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurações do ItemCarrinho
            modelBuilder.Entity<ItemCarrinho>()
                .HasOne(i => i.Carrinho)
                .WithMany(c => c.Itens)
                .HasForeignKey(i => i.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemCarrinho>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurações dos Favoritos
            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Produto)
                .WithMany()
                .HasForeignKey(f => f.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único para evitar duplicatas de favoritos
            modelBuilder.Entity<Favorito>()
                .HasIndex(f => new { f.UserId, f.ProdutoId })
                .IsUnique();
        }
    }
} 