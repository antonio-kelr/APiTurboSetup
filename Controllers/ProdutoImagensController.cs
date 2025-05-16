using APiTurboSetup.Interfaces;
using APiTurboSetup.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace APiTurboSetup.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoImagensController : ControllerBase
    {
        private readonly IProdutoImagemRepository _produtoImagemRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly string _bucketName;

        public ProdutoImagensController(
            IProdutoImagemRepository produtoImagemRepository, 
            IProdutoRepository produtoRepository,
            IFirebaseStorageService firebaseStorageService,
            IConfiguration configuration)
        {
            _produtoImagemRepository = produtoImagemRepository;
            _produtoRepository = produtoRepository;
            _firebaseStorageService = firebaseStorageService;
            _bucketName = configuration["Firebase:BucketName"];
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoImagem>>> GetProdutoImagens()
        {
            var imagens = await _produtoImagemRepository.GetAllAsync();
            return Ok(imagens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoImagem>> GetProdutoImagem(int id)
        {
            var imagem = await _produtoImagemRepository.GetByIdAsync(id);

            if (imagem == null)
            {
                return NotFound("Imagem não encontrada.");
            }

            return Ok(imagem);
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult<IEnumerable<ProdutoImagem>>> GetImagensByProduto(int produtoId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            var imagens = await _produtoImagemRepository.GetByProdutoIdAsync(produtoId);
            return Ok(imagens);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProdutoImagem>>> UploadImagens([FromForm] int produtoId, [FromForm] List<IFormFile> imagens)
        {
            Console.WriteLine($"INÍCIO UPLOAD IMAGENS - ProdutoId: {produtoId} - Quantidade de imagens: {imagens?.Count ?? 0}");

            if (imagens == null || !imagens.Any())
                return BadRequest("Nenhuma imagem foi enviada.");

            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            try
            {
                // Upload das imagens para o Firebase Storage
                var urls = await _firebaseStorageService.UploadImagesAsync(imagens, $"produtos/{produtoId}");
                
                if (urls == null || !urls.Any())
                    return BadRequest("Falha ao fazer upload das imagens.");

                // Criar registros das imagens no banco de dados
                var produtoImagens = new List<ProdutoImagem>();
                for (int i = 0; i < urls.Count; i++)
                {
                    var produtoImagem = new ProdutoImagem
                    {
                        ProdutoId = produtoId,
                        Url = urls[i],
                        Titulo = imagens[i].FileName,
                        Ordem = i
                    };
                    produtoImagens.Add(produtoImagem);
                }

                var newImagens = new List<ProdutoImagem>();
                foreach (var imagem in produtoImagens)
                {
                    var newImagem = await _produtoImagemRepository.AddAsync(imagem);
                    newImagens.Add(newImagem);
                }

                return CreatedAtAction(nameof(GetImagensByProduto), new { produtoId }, newImagens);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao fazer upload das imagens: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProdutoImagem(int id, ProdutoImagem produtoImagem)
        {
            if (id != produtoImagem.Id)
                return BadRequest("ID da imagem não corresponde.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingImagem = await _produtoImagemRepository.GetByIdAsync(id);
            if (existingImagem == null)
                return NotFound("Imagem não encontrada.");

            var produto = await _produtoRepository.GetByIdAsync(produtoImagem.ProdutoId);
            if (produto == null)
                return BadRequest("Produto inválido.");

            await _produtoImagemRepository.UpdateAsync(produtoImagem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProdutoImagem(int id)
        {
            var success = await _produtoImagemRepository.DeleteAsync(id);
            if (!success)
                return NotFound("Imagem não encontrada.");

            return NoContent();
        }
    }
} 