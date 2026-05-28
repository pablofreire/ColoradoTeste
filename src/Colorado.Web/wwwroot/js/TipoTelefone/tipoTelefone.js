// Tela de tipos de telefone. Tabela e formulario sao partials do servidor.

let tipoModal = null;

const elementosTipo = {
    btnNovo: document.getElementById('idTipoNovo'),
    containerTabela: document.getElementById('containerTabela'),
    containerModal: document.getElementById('containerModal')
};

const tipoTelefoneInicializar = () => {
    elementosTipo.btnNovo.addEventListener('click', () => abrirFormularioTipo(null));
    elementosTipo.containerTabela.addEventListener('click', tratarCliqueTabelaTipo);

    carregarTabelaTipo();
};

const tratarCliqueTabelaTipo = (evento) => {
    const botao = evento.target.closest('button[data-acao]');
    if (!botao) return;

    const codigo = parseInt(botao.dataset.codigo, 10);
    if (botao.dataset.acao === 'editar') abrirFormularioTipo(codigo);
    else if (botao.dataset.acao === 'excluir') excluirTipo(codigo);
};

const carregarTabelaTipo = async () => {
    const resposta = await obterParcial('/TipoTelefone/Listar');
    if (!resposta.ok) {
        exibirToast('Erro ao carregar os tipos de telefone.', 'erro');
        return;
    }
    elementosTipo.containerTabela.innerHTML = resposta.html;
};

const abrirFormularioTipo = async (codigo) => {
    const url = codigo ? `/TipoTelefone/Formulario?codigo=${codigo}` : '/TipoTelefone/Formulario';
    const resposta = await obterParcial(url);
    if (!resposta.ok) {
        exibirToast('Erro ao abrir o formulario.', 'erro');
        return;
    }
    montarFormularioTipo(resposta.html);
};

const montarFormularioTipo = (html) => {
    elementosTipo.containerModal.innerHTML = html;

    const modalElemento = document.getElementById('idTipoModal');
    tipoModal = new bootstrap.Modal(modalElemento);
    modalElemento.addEventListener('hidden.bs.modal', () => {
        elementosTipo.containerModal.innerHTML = '';
    });

    document.getElementById('idTipoForm').addEventListener('submit', salvarTipo);
    tipoModal.show();
};

const salvarTipo = async (evento) => {
    evento.preventDefault();
    const formulario = document.getElementById('idTipoForm');

    const resposta = await enviarFormulario('/TipoTelefone/Salvar', new FormData(formulario));

    if (resposta.json && resposta.json.sucesso) {
        tipoModal.hide();
        exibirToast(resposta.json.mensagem);
        carregarTabelaTipo();
        return;
    }

    if (resposta.html) {
        montarFormularioTipo(resposta.html);
    }
};

const excluirTipo = async (codigo) => {
    if (!confirm('Deseja realmente excluir este tipo de telefone?')) {
        return;
    }

    const resposta = await postarJson('/TipoTelefone/Excluir', { codigo });
    if (!resposta.ok) {
        exibirToast(resposta.json?.mensagem || 'Erro ao excluir.', 'erro');
        return;
    }

    exibirToast(resposta.json.mensagem);
    carregarTabelaTipo();
};
