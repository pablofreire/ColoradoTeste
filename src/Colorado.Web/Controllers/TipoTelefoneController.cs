using Colorado.Aplicacao.Dtos.TiposTelefone;
using Colorado.Web.ApiClients;
using Colorado.Web.ApiClients.TiposTelefone;
using Colorado.Web.Mapeamentos.TipoTelefone;
using Colorado.Web.Models;
using Colorado.Web.Models.TipoTelefone;
using Microsoft.AspNetCore.Mvc;

namespace Colorado.Web.Controllers;

public class TipoTelefoneController : Controller
{
    private readonly ITipoTelefoneApiClient _tipoTelefoneApi;

    public TipoTelefoneController(ITipoTelefoneApiClient tipoTelefoneApi)
    {
        _tipoTelefoneApi = tipoTelefoneApi;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var resultado = await _tipoTelefoneApi.ListarAsync();
        var tipos = resultado.Dados ?? Enumerable.Empty<TipoTelefoneDto>();
        return PartialView("_TabelaTiposTelefone", TipoTelefoneViewModelMapeador.ParaLista(tipos));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario(int? codigo)
    {
        SalvarTipoTelefoneViewModel vm;
        if (codigo.HasValue)
        {
            var resultado = await _tipoTelefoneApi.ObterPorIdAsync(codigo.Value);
            if (!resultado.Sucesso || resultado.Dados is null)
                return NotFound();

            vm = TipoTelefoneViewModelMapeador.ParaFormulario(resultado.Dados);
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
        var dto = TipoTelefoneViewModelMapeador.ParaDto(vm);

        var (sucesso, mensagem, erros) = vm.EhEdicao
            ? Reduzir(await _tipoTelefoneApi.AtualizarAsync(vm.Codigo!.Value, dto))
            : Reduzir(await _tipoTelefoneApi.CriarAsync(dto));

        if (sucesso)
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Tipo de telefone salvo com sucesso." });

        var listaErros = erros.Count > 0
            ? erros.ToList()
            : new List<string> { mensagem ?? "Nao foi possivel salvar o tipo de telefone." };

        Response.StatusCode = StatusCodes.Status400BadRequest;
        return Json(new RespostaJson { Sucesso = false, Mensagem = mensagem, Erros = listaErros });
    }

    [HttpPost]
    public async Task<IActionResult> Excluir(int codigo)
    {
        var resultado = await _tipoTelefoneApi.RemoverAsync(codigo);
        if (resultado.Sucesso)
            return Json(new RespostaJson { Sucesso = true, Mensagem = "Tipo de telefone removido com sucesso." });

        Response.StatusCode = StatusCodes.Status400BadRequest;
        return Json(new RespostaJson
        {
            Sucesso = false,
            Mensagem = resultado.Mensagem ?? "Nao foi possivel remover o tipo de telefone."
        });
    }

    private static (bool Sucesso, string? Mensagem, IReadOnlyList<string> Erros) Reduzir<T>(ResultadoApi<T> resultado)
        => (resultado.Sucesso, resultado.Mensagem, resultado.Erros ?? new List<string>());
}
