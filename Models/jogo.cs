namespace BoardGamesMarket.Models;

public class Jogo
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string ImagemUrl { get; set; } = string.Empty;
    public double Preco { get; set; }
    public string Estado { get; set; } = string.Empty; // 🔥 Novo campo
    public string UsuarioId { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public string TelefoneUsuario { get; set; } = string.Empty;
}
