// Tela de clientes. A tabela e o formulario sao partials renderizadas no servidor;
// aqui so cuidamos de buscar, injetar e enviar.

let clienteModal = null;
let paginaAtual = 1;
let contadorTelefone = 0;

const elementosCliente = {
    filtroNome: document.getElementById('idClienteFiltroNome'),
    filtroDocumento: document.getElementById('idClienteFiltroDocumento'),
    btnNovo: document.getElementById('idClienteNovo'),
    btnPesquisar: document.getElementById('idClientePesquisar'),
    btnLimpar: document.getElementById('idClienteLimpar'),
    containerTabela: document.getElementById('containerTabela'),
    containerModal: document.getElementById('containerModal')
};

const clienteInicializar = () => {
    elementosCliente.btnNovo.addEventListener('click', () => abrirFormulario(null));
    elementosCliente.btnPesquisar.addEventListener('click', () => carregarTabela(1));
    elementosCliente.btnLimpar.addEventListener('click', limparFiltros);
    elementosCliente.containerTabela.addEventListener('click', tratarCliqueTabela);

    carregarTabela(1);
};

const limparFiltros = () => {
    elementosCliente.filtroNome.value = '';
    elementosCliente.filtroDocumento.value = '';
    carregarTabela(1);
};

// Clique nos botoes da tabela (editar/excluir) e na paginacao.
const tratarCliqueTabela = (evento) => {
    const botaoAcao = evento.target.closest('button[data-acao]');
    if (botaoAcao) {
        const codigo = parseInt(botaoAcao.dataset.codigo, 10);
        if (botaoAcao.dataset.acao === 'editar') abrirFormulario(codigo);
        else if (botaoAcao.dataset.acao === 'excluir') excluir(codigo);
        return;
    }

    const botaoPagina = evento.target.closest('button[data-pagina]');
    if (botaoPagina && !botaoPagina.disabled) {
        carregarTabela(parseInt(botaoPagina.dataset.pagina, 10));
    }
};

const carregarTabela = async (pagina) => {
    paginaAtual = pagina;
    const nome = encodeURIComponent(elementosCliente.filtroNome.value.trim());
    const documento = encodeURIComponent(elementosCliente.filtroDocumento.value.trim());

    const resposta = await obterParcial(`/Cliente/Listar?nome=${nome}&documento=${documento}&pagina=${pagina}`);
    if (!resposta.ok) {
        exibirToast('Erro ao carregar a lista de clientes.', 'erro');
        return;
    }
    elementosCliente.containerTabela.innerHTML = resposta.html;
};

const abrirFormulario = async (codigo) => {
    const url = codigo ? `/Cliente/Formulario?codigo=${codigo}` : '/Cliente/Formulario';
    const resposta = await obterParcial(url);
    if (!resposta.ok) {
        exibirToast('Erro ao abrir o formulario.', 'erro');
        return;
    }
    montarFormulario(resposta.html);
};

// Injeta o HTML do formulario no container, liga os eventos e abre o modal.
const montarFormulario = (html) => {
    elementosCliente.containerModal.innerHTML = html;

    const modalElemento = document.getElementById('idClienteModal');
    clienteModal = new bootstrap.Modal(modalElemento);
    modalElemento.addEventListener('hidden.bs.modal', () => {
        elementosCliente.containerModal.innerHTML = '';
    });

    document.getElementById('idClienteForm').addEventListener('submit', salvar);
    document.getElementById('idTelefoneAdicionar').addEventListener('click', adicionarTelefone);
    document.getElementById('idTelefoneLinhas').addEventListener('click', (evento) => {
        const remover = evento.target.closest('button[data-remover-telefone]');
        if (remover) remover.closest('tr').remove();
    });

    clienteModal.show();
};

const salvar = async (evento) => {
    evento.preventDefault();
    const formulario = document.getElementById('idClienteForm');

    const tipoPessoa = formulario.querySelector('[name="TipoPessoa"]').value;
    const documento = formulario.querySelector('[name="Documento"]').value;
    if (!validarDocumento(tipoPessoa, documento)) {
        exibirToast('Documento invalido para o tipo de pessoa.', 'erro');
        return;
    }

    const resposta = await enviarFormulario('/Cliente/Salvar', new FormData(formulario));

    if (resposta.json && resposta.json.sucesso) {
        clienteModal.hide();
        exibirToast(resposta.json.mensagem);
        carregarTabela(paginaAtual);
        return;
    }

    // Erro de validacao: o servidor devolve o formulario com o resumo de erros.
    if (resposta.html) {
        montarFormulario(resposta.html);
    }
};

const excluir = async (codigo) => {
    if (!confirm('Deseja realmente excluir este cliente?')) {
        return;
    }

    const resposta = await postarJson('/Cliente/Excluir', { codigo });
    if (!resposta.ok) {
        exibirToast(resposta.json?.mensagem || 'Erro ao excluir.', 'erro');
        return;
    }

    exibirToast(resposta.json.mensagem);
    carregarTabela(paginaAtual);
};

// Clona a linha-template, gerando uma chave unica para o model binding da lista.
const adicionarTelefone = () => {
    const template = document.getElementById('idTelefoneTemplate');
    const chave = `n${contadorTelefone++}`;
    const html = template.innerHTML.replaceAll('__key__', chave);
    document.getElementById('idTelefoneLinhas').insertAdjacentHTML('beforeend', html);
};
