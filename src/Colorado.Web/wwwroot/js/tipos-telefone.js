// Tela de tipos de telefone: listagem e cadastro/edicao via modal.

// #region [ VARIAVEIS ]
let tipoModal = null;
// #endregion

// #region [ ELEMENTOS DOM ]
const elementosTipo = {
    tabela: document.getElementById('idTipoTabela'),
    modal: document.getElementById('idTipoModal'),
    formulario: document.getElementById('idTipoFormulario'),
    titulo: document.getElementById('idTipoModalTitulo'),
    codigo: document.getElementById('idTipoCodigo'),
    descricao: document.getElementById('idTipoDescricao'),
    btnNovo: document.getElementById('idTipoNovo')
};
// #endregion

// #region [ INICIALIZACAO E EVENTOS ]
const tipoTelefoneInicializar = () => {
    tipoModal = new bootstrap.Modal(elementosTipo.modal);

    elementosTipo.btnNovo.addEventListener('click', abrirNovoTipo);
    elementosTipo.formulario.addEventListener('submit', salvarTipo);
    elementosTipo.tabela.addEventListener('click', tratarCliqueTabela);

    carregarTipos();
};

// Delegacao de evento: evita depender de onclick inline na linha gerada.
const tratarCliqueTabela = (evento) => {
    const botao = evento.target.closest('button[data-acao]');
    if (!botao) {
        return;
    }

    const codigo = parseInt(botao.dataset.codigo, 10);
    if (botao.dataset.acao === 'editar') {
        abrirEdicaoTipo(codigo, botao.dataset.descricao);
    } else if (botao.dataset.acao === 'excluir') {
        excluirTipo(codigo);
    }
};
// #endregion

// #region [ LISTAGEM ]
const carregarTipos = async () => {
    const resposta = await enviarRequisicao('/TipoTelefone/Listar');
    if (!resposta.ok) {
        exibirToast('Erro ao carregar os tipos de telefone.', 'erro');
        return;
    }

    montarTabela(resposta.dados.dados || []);
};

const montarTabela = (tipos) => {
    if (tipos.length === 0) {
        elementosTipo.tabela.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-4">Nenhum tipo cadastrado.</td></tr>';
        return;
    }

    elementosTipo.tabela.innerHTML = tipos.map(t => `
        <tr>
            <td>${t.codigoTipoTelefone}</td>
            <td>${t.descricaoTipoTelefone}</td>
            <td class="tabela-acoes text-end">
                <button class="btn btn-sm btn-outline-secondary" data-acao="editar"
                        data-codigo="${t.codigoTipoTelefone}" data-descricao="${t.descricaoTipoTelefone}">Editar</button>
                <button class="btn btn-sm btn-outline-danger" data-acao="excluir"
                        data-codigo="${t.codigoTipoTelefone}">Excluir</button>
            </td>
        </tr>`).join('');
};
// #endregion

// #region [ CADASTRO E EDICAO ]
const abrirNovoTipo = () => {
    elementosTipo.titulo.textContent = 'Novo tipo de telefone';
    elementosTipo.codigo.value = '';
    elementosTipo.descricao.value = '';
    elementosTipo.descricao.classList.remove('is-invalid');
    tipoModal.show();
};

const abrirEdicaoTipo = (codigo, descricao) => {
    elementosTipo.titulo.textContent = 'Editar tipo de telefone';
    elementosTipo.codigo.value = codigo;
    elementosTipo.descricao.value = descricao;
    elementosTipo.descricao.classList.remove('is-invalid');
    tipoModal.show();
};

const salvarTipo = async (evento) => {
    evento.preventDefault();

    const descricao = elementosTipo.descricao.value.trim();
    if (descricao === '') {
        elementosTipo.descricao.classList.add('is-invalid');
        return;
    }
    elementosTipo.descricao.classList.remove('is-invalid');

    const codigo = elementosTipo.codigo.value;
    const corpo = { descricaoTipoTelefone: descricao };
    const url = codigo
        ? `/TipoTelefone/Atualizar?codigo=${codigo}`
        : '/TipoTelefone/Criar';

    const resposta = await enviarRequisicao(url, 'POST', corpo);
    if (!resposta.ok) {
        exibirToast(resposta.dados?.mensagem || 'Erro ao salvar.', 'erro');
        return;
    }

    tipoModal.hide();
    exibirToast(resposta.dados.mensagem || 'Tipo salvo.');
    carregarTipos();
};

const excluirTipo = async (codigo) => {
    if (!confirm('Deseja realmente excluir este tipo de telefone?')) {
        return;
    }

    const resposta = await enviarRequisicao(`/TipoTelefone/Excluir?codigo=${codigo}`, 'POST');
    if (!resposta.ok) {
        exibirToast(resposta.dados?.mensagem || 'Erro ao excluir.', 'erro');
        return;
    }

    exibirToast(resposta.dados.mensagem || 'Tipo removido.');
    carregarTipos();
};
// #endregion
