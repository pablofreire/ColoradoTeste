using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Dominio.Excecoes;
using Colorado.Web.Mapeamentos.Cliente;
using Colorado.Web.Models;
using Colorado.Web.Models.Cliente;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class ClienteController : Controller
{
    private const string UsuarioWeb = "web";

    private readonly IClienteServico _clienteServico;
    private readonly ITipoTelefoneServico _tipoTelefoneServico;

    public ClienteController(IClienteServico clienteServico, ITipoTelefoneServico tipoTelefoneServico)
    {
        _clienteServico = clienteServico;
        _tipoTelefoneServico = tipoTelefoneServico;
    }

    public IActionResult Index()
    {
        return View();
    }

    // Tabela renderizada no servidor e injetada pelo JS no container da listagem.
    [HttpGet]
    public async Task<IActionResult> Listar(string? nome, string? documento, int pagina = 1)
    {
        var resultado = await _clienteServico.ListarAsync(nome, documento, pagina, 10);
        return PartialView("_TabelaClientes", ClienteViewModelMapeador.ParaLista(resultado, nome, documento));
    }

    // Formulario (novo ou edicao) renderizado no servidor e injetado no modal.
    [HttpGet]
    public async Task<IActionResult> Formulario(int? codigo)
    {
        SalvarClienteViewModel vm;
        if (codigo.HasValue)
        {
            var dto = await _clienteServico.ObterPorIdAsync(codigo.Value);
            if (dto is null)
                return NotFound();

            vm = ClienteViewModelMapeador.ParaFormulario(dto);
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
        try
        {
            var dto = ClienteViewModelMapeador.ParaDto(vm);

            if (vm.EhEdicao)
                await _clienteServico.AtualizarAsync(vm.Codigo!.Value, dto, UsuarioWeb);
            else
                await _clienteServico.CriarAsync(dto, UsuarioWeb);

            return Json(new RespostaJson { Sucesso = true, Mensagem = "Cliente salvo com sucesso." });
        }
        catch (ValidacaoException ex)
        {
            foreach (var erro in ex.Erros)
                ModelState.AddModelError(string.Empty, erro);
            return await RecarregarFormularioAsync(vm);
        }
        catch (DominioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return await RecarregarFormularioAsync(vm);
        }
        catch (NaoEncontradoException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Excluir(int codigo)
    {
        try
        {
            await _clienteServico.RemoverAsync(codigo);
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Cliente removido com sucesso." });
        }
        catch (DominioException ex)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Json(new RespostaJson { Sucesso = false, Mensagem = ex.Message });
        }
        catch (NaoEncontradoException ex)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Json(new RespostaJson { Sucesso = false, Mensagem = ex.Message });
        }
    }

    private async Task<IActionResult> RecarregarFormularioAsync(SalvarClienteViewModel vm)
    {
        await PreencherCombosAsync(vm);
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView("_FormularioCliente", vm);
    }

    private async Task PreencherCombosAsync(SalvarClienteViewModel vm)
    {
        vm.TiposDisponiveis = ClienteViewModelMapeador.ParaOpcoes(await _tipoTelefoneServico.ListarAsync());
    }
}
