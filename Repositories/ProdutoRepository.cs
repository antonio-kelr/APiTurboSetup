using APiTurboSetup.Data;
using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APiTurboSetup.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Produto>> GetByCategoriaIdAsync(int categoriaId)
        {
            return await _dbSet.Where(p => p.CategoriaId == categoriaId)
                               .Include(p => p.Categoria)
                               .Include(p => p.Imagens)
                               .Select(p => new Produto
                               {
                                   Id = p.Id,
                                   Nome = p.Nome,
                                   Slug = p.Slug,
                                   Descricao = p.Descricao,
                                   Preco = p.Preco,
                                   CategoriaId = p.CategoriaId,
                                   Marca = p.Marca,
                                   Quantidade = p.Quantidade,

                                   Categoria = new Categoria
                                   {
                                       Id = p.Categoria.Id,
                                       Nome = p.Categoria.Nome
                                   },
                                   Imagens = p.Imagens,
                               })
                               .ToListAsync();
        }

        public async Task<Produto?> GetByNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Nome.ToLower() == nome.ToLower());
        }

        public async Task<Produto?> GetBySlugAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return null;

            return await _dbSet.Include(p => p.Categoria)
                              .Include(p => p.Imagens)
                              .Select(p => new Produto
                              {
                                  Id = p.Id,
                                  Nome = p.Nome,
                                  Slug = p.Slug,
                                  Descricao = p.Descricao,
                                  Preco = p.Preco,
                                  PrecoAntigo = p.PrecoAntigo,
                                  CategoriaId = p.CategoriaId,
                                  Marca = p.Marca,
                                  Quantidade = p.Quantidade,

                                  Categoria = new Categoria
                                  {
                                      Id = p.Categoria.Id,
                                      Nome = p.Categoria.Nome
                                  },
                                  Imagens = p.Imagens,
                              })
                              .FirstOrDefaultAsync(p => p.Slug.ToLower() == slug.ToLower());
        }

        // Sobrescrevendo métodos para incluir a categoria nas consultas
        public override async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _dbSet.Include(p => p.Categoria)
                              .Include(p => p.Imagens)
                              .Select(p => new Produto
                              {
                                  Id = p.Id,
                                  Nome = p.Nome,
                                  Slug = p.Slug,
                                  Descricao = p.Descricao,
                                  Preco = p.Preco,
                                  CategoriaId = p.CategoriaId,
                                  Marca = p.Marca,
                                  Quantidade = p.Quantidade,

                                  Categoria = new Categoria
                                  {
                                      Id = p.Categoria.Id,
                                      Nome = p.Categoria.Nome
                                  },
                                  Imagens = p.Imagens,
                              })
                              .ToListAsync();
        }

        public override async Task<Produto?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(p => p.Categoria)
                              .Include(p => p.Imagens)
                              .Select(p => new Produto
                              {
                                  Id = p.Id,
                                  Nome = p.Nome,
                                  Slug = p.Slug,
                                  Descricao = p.Descricao,
                                  Preco = p.Preco,
                                  CategoriaId = p.CategoriaId,
                                  Marca = p.Marca,
                                  Quantidade = p.Quantidade,

                                  Categoria = new Categoria
                                  {
                                      Id = p.Categoria.Id,
                                      Nome = p.Categoria.Nome
                                  },
                                  Imagens = p.Imagens,
                              })
                              .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Produto>> SearchAsync(string query)
        {
            var lowerCaseQuery = query.ToLower();
            return await _context.Produtos
               .Include(p => p.Imagens) 
                .Where(p => p.Nome.ToLower().Contains(lowerCaseQuery) || 
                
                            p.Descricao.ToLower().Contains(lowerCaseQuery) || 
                            p.Marca.ToLower().Contains(lowerCaseQuery))
                .ToListAsync();
        }
    }
}