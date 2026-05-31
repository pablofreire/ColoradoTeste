using Colorado.Aplicacao.Dtos.Clientes;
using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Dominio.Paginacao;
using Colorado.Web.ApiClients;
using Colorado.Web.ApiClients.Clientes;
using Colorado.Web.ApiClients.TiposTelefone;
using Colorado.Web.Mapeamentos.Cliente;
using Colorado.Web.Models;
using Colorado.Web.Models.Cliente;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class ClienteController : Controller
{
    private readonly IClienteApiClient _clienteApi;
    private readonly ITipoTelefoneApiClient _tipoTelefoneApi;

    public ClienteController(IClienteApiClient clienteApi, ITipoTelefoneApiClient tipoTelefoneApi)
    {
        _clienteApi = clienteApi;
        _tipoTelefoneApi = tipoTelefoneApi;
    }

    public IActionResult Index()
    {
        return View();
    }

    // Tabela renderizada no servidor e injetada pelo JS no container da listagem.
    [HttpGet]
    public async Task<IActionResult> Listar(string? nome, string? documento, int pagina = 1)
    {
        var resultado = await _clienteApi.ListarAsync(nome, documento, pagina, 10);
        var dados = resultado.Dados
            ?? new ResultadoPaginado<ClienteDto>(Array.Empty<ClienteDto>(), 0, pagina, 10);

        return PartialView("_TabelaClientes", ClienteViewModelMapeador.ParaLista(dados, nome, documento));
    }

    // Formulario (novo ou edicao) renderizado no servidor e injetado no modal.
    [HttpGet]
    public async Task<IActionResult> Formulario(int? codigo)
    {
        SalvarClienteViewModel vm;
        if (codigo.HasValue)
        {
            var resultado = await _clienteApi.ObterPorIdAsync(codigo.Value);
            if (!resultado.Sucesso || resultado.Dados is null)
                return NotFound();

            vm = ClienteViewModelMapeador.ParaFormulario(resultado.Dados);
        }
        else
        {
            vm = new SalvarClienteViewModel();
        }

        await PreencherCombosAsync(vm);
        return PartialView("_FormularioCliente", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(SalvarClienteViewModel vm)
    {
        var dto = ClienteViewModelMapeador.ParaDto(vm);

        var (sucesso, mensagem, erros) = vm.EhEdicao
            ? Reduzir(await _clienteApi.AtualizarAsync(vm.Codigo!.Value, dto))
            : Reduzir(await _clienteApi.CriarAsync(dto));

        if (sucesso)
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Cliente salvo com sucesso." });

        var listaErros = erros.Count > 0
            ? erros.ToList()
            : new List<string> { mensagem ?? "Nao foi possivel salvar o cliente." };

        Response.StatusCode = StatusCodes.Status400BadRequest;
        return Json(new RespostaJson { Sucesso = false, Mensagem = mensagem, Erros = listaErros });
    }

    [HttpPost]
    public async Task<IActionResult> Excluir(int codigo)
    {
        var resultado = await _clienteApi.RemoverAsync(codigo);
        if (resultado.Sucesso)
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Cliente removido com sucesso." });

        Response.StatusCode = StatusCodes.Status400BadRequest;
        return Json(new RespostaJson
        {
            Sucesso = false,
            Mensagem = resultado.Mensagem ?? "Nao foi possivel remover o cliente."
        });
    }

    private async Task PreencherCombosAsync(SalvarClienteViewModel vm)
    {
        var resultado = await _tipoTelefoneApi.ListarAsync();
        vm.TiposDisponiveis = ClienteViewModelMapeador.ParaOpcoes(
            resultado.Dados ?? Enumerable.Empty<TipoTelefoneDto>());
    }

    private static (bool Sucesso, string? Mensagem, IReadOnlyList<string> Erros) Reduzir<T>(ResultadoApi<T> resultado)
        => (resultado.Sucesso, resultado.Mensagem, resultado.Erros ?? new List<string>());
}
