using FluentAssertions;
using Moq;
using Produto.Services;
using Xunit;

namespace Produto.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> produtoRepository;
    private readonly ProdutoService produtoService;
    public ProdutoServiceTests()
    {
        produtoRepository = new();

        produtoService = new(produtoRepository.Object);
    }

    [Theory(DisplayName = "Obter um produto pelo Id")]
    [InlineData(1)]
    public void ProdutoService_GetProduto_DeveRetornarUmProdutoPeloID(int id)
    {
        //Arrange
        var produto = new Produto
        {
            Id = id,
            Nome = "Café Pilão",
            Preco = 19.80
        };

        produtoRepository.Setup(x => x.GetById(id))
            .Returns(produto);

        //Act
        var result = produtoService.GetProduto(id);

        //Assert
        produtoRepository.Verify(x => x.GetById(id), Times.Once);
        result.Should().BeSameAs(produto).And.BeOfType(typeof(Produto));
    }

    [Fact(DisplayName = "Salvar produto válido")]
    public void Salvar_Produto_Valido()
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = 19.50

        };

        //Act
        produtoService.SalvarProduto(produto);

        //Assert

        produtoRepository.Verify(x => x.Save(produto), Times.Once);
    }

    [Fact(DisplayName = "Salvar produto Nulo")]
    public void Salvar_Produto_Nulo()
    {
        //Arrange
        Produto produto = null;

        //Act

        produtoService.Invoking(y => y.SalvarProduto(produto)).Should().Throw<ArgumentNullException>().Where(x => x.Message.StartsWith("O produto"));

        //Assert

        produtoRepository.Verify(x => x.Save(produto), Times.Never);
    }

    [Theory(DisplayName = "Salvar produto com nome Nulo ou em Branco")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Salvar_Produto_ComNomeNuloOuEmBranco(string nome)
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = nome,
            Preco = 19.50

        };

        //Act

        produtoService.Invoking(y => y.SalvarProduto(produto)).Should().Throw<ArgumentException>().Where(x => x.Message.StartsWith("O nome do produto não"));

        //Assert

        produtoRepository.Verify(x => x.Save(produto), Times.Never);
    }

    [Theory(DisplayName = "Salvar produto com preço menor ou igual a zero")]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Salvar_Produto_ComPrecoMenorOuIgualAZero(double preco)
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = preco

        };

        //Act

        produtoService.Invoking(y => y.SalvarProduto(produto)).Should().Throw<ArgumentException>().Where(x => x.Message.StartsWith("O preço do produto deve"));

        //Assert

        produtoRepository.Verify(x => x.Save(produto), Times.Never);
    }

    [Fact(DisplayName = "Atualizar produto válido")]
    public void Atualizar_Produto_Valido()
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = 19.50
        };

        produtoRepository.Setup(x => x.GetById(produto.Id))
            .Returns(produto);

        //Act
        produtoService.AtualizarProduto(produto);

        //Assert

        produtoRepository.Verify(x => x.Update(produto), Times.Once);
    }

    [Fact(DisplayName = "Atualizar produto Nulo")]
    public void Atualizar_Produto_Nulo()
    {
        //Arrange
        Produto produto = null;

        //Act

        produtoService.Invoking(y => y.AtualizarProduto(produto)).Should().Throw<ArgumentNullException>().Where(x => x.Message.StartsWith("O produto"));

        //Assert

        produtoRepository.Verify(x => x.Update(produto), Times.Never);
    }

    [Fact(DisplayName = "Atualizar produto inexistente na base de dados")]
    public void Atualizar_Produto_InexistenteNaBaseDeDados()
    {
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = 19.50
        };

        produtoRepository.Setup(x => x.GetById(2))
            .Returns(produto);

        //Act

        produtoService.Invoking(y => y.AtualizarProduto(produto)).Should().Throw<InvalidOperationException>().Where(x => x.Message.StartsWith("Não é possível atualizar"));

        //Assert

        produtoRepository.Verify(x => x.Update(produto), Times.Never);
    }

    [Theory(DisplayName = "Atualizar produto com nome Nulo ou em Branco")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Atualizar_Produto_ComNomeNuloOuEmBranco(string nome)
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = nome,
            Preco = 19.50

        };

        produtoRepository.Setup(x => x.GetById(produto.Id))
            .Returns(produto);

        //Act

        produtoService.Invoking(y => y.AtualizarProduto(produto)).Should().Throw<ArgumentException>().Where(x => x.Message.StartsWith("O nome do produto não pode"));

        //Assert

        produtoRepository.Verify(x => x.Update(produto), Times.Never);
    }

    [Theory(DisplayName = "Atualizar produto com preço menor ou igual a zero")]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Atualizar_Produto_ComPrecoMenorOuIgualAZero(double preco)
    {
        //Arrange
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = preco

        };

        produtoRepository.Setup(x => x.GetById(produto.Id))
            .Returns(produto);

        //Act

        produtoService.Invoking(y => y.AtualizarProduto(produto)).Should().Throw<ArgumentException>().Where(x => x.Message.StartsWith("O preço do produto deve"));

        //Assert

        produtoRepository.Verify(x => x.Update(produto), Times.Never);
    }

    [Fact(DisplayName = "Excluir produto válido")]
    public void Excluir_Produto_Valido()
    {
        //Arrange

        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = 19.50
        };

        produtoRepository.Setup(x => x.GetById(produto.Id))
            .Returns(produto);

        //Act

        produtoService.ExcluirProduto(produto.Id);

        //Assert

        produtoRepository.Verify(x => x.Delete(produto.Id), Times.Once);
    }

    [Fact(DisplayName = "Excluir produto inexistente na base de dados")]
    public void Excluir_Produto_InexistenteNaBaseDeDados()
    {
        var produto = new Produto
        {
            Id = 1,
            Nome = "Café Pelé",
            Preco = 19.50
        };

        produtoRepository.Setup(x => x.GetById(2))
            .Returns(produto);

        //Act

        produtoService.Invoking(y => y.ExcluirProduto(produto.Id)).Should().Throw<InvalidOperationException>().Where(x => x.Message.StartsWith("Não é possível excluir"));

        //Assert

        produtoRepository.Verify(x => x.Delete(produto.Id), Times.Never);
    }

    [Fact(DisplayName = "Obter todos produtos cadastrados")]
    public void ProdutoService_ObterTodosProdutos_DeveRetornarTodosProdutos()
    {
        //Arrange
        var produto1 = new Produto { Id = 1, Nome = "Café Pelé", Preco = 19.50 };
        var produto2 = new Produto { Id = 2, Nome = "Café Pilão", Preco = 20.49 };
        var produto3 = new Produto { Id = 3, Nome = "Café Melita", Preco = 18.13 };

        var listaProdutosBanco = new List<Produto> { produto1, produto2, produto3 };

        produtoRepository.Setup(x => x.GetAll())
            .Returns(listaProdutosBanco);

        //Act
        var result = produtoService.ObterTodosProdutos();

        //Assert
        produtoRepository.Verify(x => x.GetAll(), Times.Once);
        result.Should().HaveCount(3);
    }
}
