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
    tipoModal = bootstrap.Modal.getOrCreateInstance(modalElemento);

    // Ao fechar: descarta a instancia, limpa o container e remove qualquer backdrop
    // remanescente (evita a camada escura que travava a tela).
    modalElemento.addEventListener('hidden.bs.modal', () => {
        tipoModal.dispose();
        tipoModal = null;
        elementosTipo.containerModal.innerHTML = '';
        document.querySelectorAll('.modal-backdrop').forEach((b) => b.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('overflow');
        document.body.style.removeProperty('padding-right');
    }, { once: true });

    document.getElementById('idTipoForm').addEventListener('submit', salvarTipo);
    tipoModal.show();
};

// Mostra a lista de erros do servidor dentro do modal aberto (sem reconstruir o modal).
const exibirErrosTipo = (erros) => {
    const caixa = document.getElementById('idTipoErros');
    if (!caixa) return;

    const itens = (erros && erros.length) ? erros : ['Nao foi possivel salvar o tipo de telefone.'];
    caixa.innerHTML = itens.map((e) => `<div>${e}</div>`).join('');
    caixa.classList.remove('d-none');
};

// Validacao client-side: espelha o FluentValidator. Usuario so e exigido na criacao
// (a API ignora Usuario na atualizacao).
const validarFormularioTipo = (formulario) => {
    const ehEdicao = !!formulario.querySelector('[name="Codigo"]')?.value;

    const regras = [
        { campo: 'Descricao', mensagem: 'A descricao e obrigatoria.', teste: (v) => v.trim().length > 0 },
        { campo: 'Descricao', mensagem: 'A descricao deve ter no maximo 50 caracteres.', teste: (v) => v.trim().length <= 50 }
    ];

    if (!ehEdicao) {
        regras.push(
            { campo: 'Usuario', mensagem: 'O usuario e obrigatorio.', teste: (v) => v.trim().length > 0 },
            { campo: 'Usuario', mensagem: 'O usuario deve ter no maximo 50 caracteres.', teste: (v) => v.trim().length <= 50 }
        );
    }

    return validarRegras(formulario, regras);
};

const salvarTipo = async (evento) => {
    evento.preventDefault();
    const formulario = document.getElementById('idTipoForm');

    if (!validarFormularioTipo(formulario)) return;

    const resposta = await enviarFormulario('/TipoTelefone/Salvar', new FormData(formulario));

    if (resposta.json && resposta.json.sucesso) {
        tipoModal.hide();
        exibirToast(resposta.json.mensagem);
        carregarTabelaTipo();
        return;
    }

    exibirErrosTipo(resposta.json ? resposta.json.erros : null);
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
