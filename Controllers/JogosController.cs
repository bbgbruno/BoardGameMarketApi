using Microsoft.AspNetCore.Mvc;
using BoardGamesMarket.Models;
using BoardGamesMarket.Data;
using Dapper;

namespace BoardGamesMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogosController : ControllerBase
{
    private readonly DbConnectionFactory _connectionFactory;

    public JogosController(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? titulo)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"SELECT 
                            j.Id,
                                j.Titulo,
                                j.Descricao,
                                j.Preco,
                                j.Estado,
                                j.ImagemUrl,
                                u.Nome as NomeUsuario,
                                u.Telefone as TelefoneUsuario
                            FROM Jogos j
                            JOIN Usuarios u ON u.Id = j.UsuarioId";
        if (!string.IsNullOrEmpty(titulo))
            sql += " WHERE Titulo LIKE @Titulo";

        var jogos = await connection.QueryAsync<Jogo>(sql, new { Titulo = $"%{titulo}%" });
        return Ok(jogos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var jogo = await connection.QueryFirstOrDefaultAsync<Jogo>(
            "SELECT * FROM Jogos WHERE Id = @Id", new { Id = id });

        return jogo is not null ? Ok(jogo) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Jogo jogo)
    {
        jogo.Id = Guid.NewGuid().ToString();
        jogo.ImagemUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3K0wYjv3jpP-eIaJEgwaNMSCKuAXGRorHiQ&s";

        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Jogos (Id,Titulo, Descricao, Estado, Preco, ImagemUrl, UsuarioId)
                    VALUES (@Id, @Titulo, @Descricao, @Estado, @Preco, @ImagemUrl, @UsuarioId)";

        await connection.ExecuteAsync(sql, jogo);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Jogos WHERE Id = @Id", new { Id = id });
        return Ok();
    }

    // 🔥 ✅ Endpoint para buscar jogos por usuário
    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetJogosPorUsuario(string usuarioId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"SELECT 
                            j.Id,
                                j.Titulo,
                                j.Descricao,
                                j.Preco,
                                j.Estado,
                                j.ImagemUrl,
                                u.Nome as NomeUsuario,
                                u.Telefone as TelefoneUsuario
                            FROM Jogos j
                            JOIN Usuarios u ON u.Id = j.UsuarioId
                            where  j.UsuarioId = @UsuarioId";


        var jogos = await connection.QueryAsync<Jogo>(sql, new { UsuarioId = usuarioId });

        return Ok(jogos);
    }
}
