﻿using Microsoft.AspNetCore.Mvc;
using BoardGamesMarket.Models;
using BoardGamesMarket.Data;
using Dapper;

namespace BoardGamesMarket.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DbConnectionFactory _connectionFactory;

    public AuthController(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Usuario usuario)
    {
        using var connection = _connectionFactory.CreateConnection();

        usuario.Id = Guid.NewGuid().ToString();

        var sql = @"INSERT INTO Usuarios (Id, Nome, Email, SenhaHash,Cidade,Telefone) 
                VALUES (@Id, @Nome, @Email, @SenhaHash, @Cidade, @Telefone)";

        await connection.ExecuteAsync(sql, usuario);
        return Ok(usuario);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string senha)
    {
        using var connection = _connectionFactory.CreateConnection();
        var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM Usuarios WHERE Email = @Email AND SenhaHash = @Senha",
            new { Email = email, Senha = senha });

        return usuario is not null ? Ok(usuario) : Unauthorized();
    }


    [HttpGet("usuarios")]
    public async Task<IActionResult> usuarios()
    {
        using var connection = _connectionFactory.CreateConnection();
        var usuario = await connection.QueryAsync<Usuario>("SELECT * FROM Usuarios ");

        return usuario is not null ? Ok(usuario) : BadRequest();
    }
}
