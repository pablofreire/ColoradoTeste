using Colorado.Aplicacao.Excecoes;
using Colorado.Aplicacao.Interfaces;
using Colorado.Dominio.Excecoes;
using Colorado.Web.Mapeamentos.TipoTelefone;
using Colorado.Web.Models;
using Colorado.Web.Models.TipoTelefone;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class TipoTelefoneController : Controller
{
    private const string UsuarioWeb = "web";

    private readonly ITipoTelefoneServico _servico;

    public TipoTelefoneController(ITipoTelefoneServico servico)
    {
        _servico = servico;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var tipos = await _servico.ListarAsync();
        return PartialView("_TabelaTiposTelefone", TipoTelefoneViewModelMapeador.ParaLista(tipos));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario(int? codigo)
    {
        SalvarTipoTelefoneViewModel vm;
        if (codigo.HasValue)
        {
            var dto = await _servico.ObterPorIdAsync(codigo.Value);
            if (dto is null)
                return NotFound();

            vm = TipoTelefoneViewModelMapeador.ParaFormulario(dto);
        }
        else
        {
            vm = new SalvarTipoTelefoneViewModel();
        }

        return PartialView("_FormularioTipoTelefone", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Salvar(SalvarTipoTelefoneViewModel vm)
    {
        try
        {
            var dto = TipoTelefoneViewModelMapeador.ParaDto(vm);

            if (vm.EhEdicao)
                await _servico.AtualizarAsync(vm.Codigo!.Value, dto);
            else
                await _servico.CriarAsync(dto, UsuarioWeb);

            return Json(new RespostaJson { Sucesso = true, Mensagem = "Tipo de telefone salvo com sucesso." });
        }
        catch (ValidacaoException ex)
        {
            foreach (var erro in ex.Erros)
                ModelState.AddModelError(string.Empty, erro);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_FormularioTipoTelefone", vm);
        }
        catch (DominioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_FormularioTipoTelefone", vm);
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
            await _servico.RemoverAsync(codigo);
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Tipo de telefone removido com sucesso." });
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
}
