// Tela de clientes: listagem com filtro/paginacao e cadastro com telefones.

// #region [ VARIAVEIS ]
let clienteModal = null;
let paginaAtual = 1;
let totalPaginas = 0;
const tamanhoPagina = 10;

let telefonesEditando = [];
let tiposTelefone = [];
// #endregion

// #region [ ELEMENTOS DOM ]
const elementosCliente = {
    tabela: document.getElementById('idClienteTabela'),
    resumo: document.getElementById('idClienteResumo'),
    filtroNome: document.getElementById('idClienteFiltroNome'),
    filtroDocumento: document.getElementById('idClienteFiltroDocumento'),
    btnPesquisar: document.getElementById('idClientePesquisar'),
    btnLimpar: document.getElementById('idClienteLimpar'),
    btnNovo: document.getElementById('idClienteNovo'),
    btnAnterior: document.getElementById('idClienteAnterior'),
    btnProxima: document.getElementById('idClienteProxima'),

    modal: document.getElementById('idClienteModal'),
    formulario: document.getElementById('idClienteFormulario'),
    titulo: document.getElementById('idClienteModalTitulo'),
    codigo: document.getElementById('idClienteCodigo'),
    razaoSocial: document.getElementById('idClienteRazaoSocial'),
    nomeFantasia: document.getElementById('idClienteNomeFantasia'),
    tipoPessoa: document.getElementById('idClienteTipoPessoa'),
    documento: document.getElementById('idClienteDocumento'),
    cep: document.getElementById('idClienteCep'),
    endereco: document.getElementById('idClienteEndereco'),
    complemento: document.getElementById('idClienteComplemento'),
    bairro: document.getElementById('idClienteBairro'),
    cidade: document.getElementById('idClienteCidade'),
    uf: document.getElementById('idClienteUf'),

    telefoneNumero: document.getElementById('idTelefoneNumero'),
    telefoneTipo: document.getElementById('idTelefoneTipo'),
    telefoneOperadora: document.getElementById('idTelefoneOperadora'),
    btnTelefoneAdicionar: document.getElementById('idTelefoneAdicionar'),
    telefoneLista: document.getElementById('idTelefoneLista')
};
// #endregion

// #region [ INICIALIZACAO E EVENTOS ]
const clienteInicializar = async () => {
    clienteModal = new bootstrap.Modal(elementosCliente.modal);

    elementosCliente.btnNovo.addEventListener('click', abrirNovoCliente);
    elementosCliente.btnPesquisar.addEventListener('click', pesquisarClientes);
    elementosCliente.btnLimpar.addEventListener('click', limparFiltros);
    elementosCliente.btnAnterior.addEventListener('click', () => navegarPagina(-1));
    elementosCliente.btnProxima.addEventListener('click', () => navegarPagina(1));
    elementosCliente.formulario.addEventListener('submit', salvarCliente);
    elementosCliente.btnTelefoneAdicionar.addEventListener('click', adicionarTelefone);
    elementosCliente.tabela.addEventListener('click', tratarCliqueTabela);
    elementosCliente.telefoneLista.addEventListener('click', tratarCliqueTelefone);

    await carregarTiposTelefone();
    carregarClientes();
};

const tratarCliqueTabela = (evento) => {
    const botao = evento.target.closest('button[data-acao]');
    if (!botao) {
        return;
    }

    const codigo = parseInt(botao.dataset.codigo, 10);
    if (botao.dataset.acao === 'editar') {
        abrirEdicaoCliente(codigo);
    } else if (botao.dataset.acao === 'excluir') {
        excluirCliente(codigo);
    }
};

const tratarCliqueTelefone = (evento) => {
    const botao = evento.target.closest('button[data-indice]');
    if (botao) {
        removerTelefone(parseInt(botao.dataset.indice, 10));
    }
};
// #endregion

// #region [ LISTAGEM E FILTRO ]
const pesquisarClientes = () => {
    paginaAtual = 1;
    carregarClientes();
};

const limparFiltros = () => {
    elementosCliente.filtroNome.value = '';
    elementosCliente.filtroDocumento.value = '';
    paginaAtual = 1;
    carregarClientes();
};

const navegarPagina = (passo) => {
    const destino = paginaAtual + passo;
    if (destino < 1 || destino > totalPaginas) {
        return;
    }
    paginaAtual = destino;
    carregarClientes();
};

const carregarClientes = async () => {
    const nome = encodeURIComponent(elementosCliente.filtroNome.value.trim());
    const documento = encodeURIComponent(elementosCliente.filtroDocumento.value.trim());
    const url = `/Cliente/Listar?nome=${nome}&documento=${documento}&pagina=${paginaAtual}&tamanhoPagina=${tamanhoPagina}`;

    const resposta = await enviarRequisicao(url);
    if (!resposta.ok) {
        exibirToast('Erro ao carregar clientes.', 'erro');
        return;
    }

    const pagina = resposta.dados.dados;
    totalPaginas = pagina.totalPaginas;
    montarTabela(pagina.itens);
    atualizarRodape(pagina);
};

const montarTabela = (clientes) => {
    if (clientes.length === 0) {
        elementosCliente.tabela.innerHTML = '<tr><td colspan="7" class="text-center text-muted py-4">Nenhum cliente encontrado.</td></tr>';
        return;
    }

    elementosCliente.tabela.innerHTML = clientes.map(c => `
        <tr>
            <td>${c.codigoCliente}</td>
            <td>${c.razaoSocial}</td>
            <td>${c.nomeFantasia}</td>
            <td>${c.tipoPessoa === 'F' ? 'Fisica' : 'Juridica'}</td>
            <td>${c.documento}</td>
            <td>${c.cidade ? c.cidade + '/' + c.uf : c.uf}</td>
            <td class="tabela-acoes text-end">
                <button class="btn btn-sm btn-outline-secondary" data-acao="editar" data-codigo="${c.codigoCliente}">Editar</button>
                <button class="btn btn-sm btn-outline-danger" data-acao="excluir" data-codigo="${c.codigoCliente}">Excluir</button>
            </td>
        </tr>`).join('');
};

const atualizarRodape = (pagina) => {
    elementosCliente.resumo.textContent =
        `Pagina ${pagina.pagina} de ${pagina.totalPaginas || 1} - ${pagina.totalRegistros} registro(s)`;
    elementosCliente.btnAnterior.disabled = pagina.pagina <= 1;
    elementosCliente.btnProxima.disabled = pagina.pagina >= pagina.totalPaginas;
};
// #endregion

// #region [ TIPOS DE TELEFONE (COMBO) ]
const carregarTiposTelefone = async () => {
    const resposta = await enviarRequisicao('/Cliente/TiposTelefone');
    if (!resposta.ok) {
        return;
    }

    tiposTelefone = resposta.dados.dados || [];
    elementosCliente.telefoneTipo.innerHTML = tiposTelefone
        .map(t => `<option value="${t.codigoTipoTelefone}">${t.descricaoTipoTelefone}</option>`)
        .join('');
};

const descricaoTipo = (codigo) => {
    const tipo = tiposTelefone.find(t => t.codigoTipoTelefone === codigo);
    return tipo ? tipo.descricaoTipoTelefone : '';
};
// #endregion

// #region [ FORMULARIO ]
const abrirNovoCliente = () => {
    elementosCliente.titulo.textContent = 'Novo cliente';
    elementosCliente.formulario.reset();
    elementosCliente.codigo.value = '';
    limparValidacao();
    telefonesEditando = [];
    renderTelefones();
    clienteModal.show();
};

const abrirEdicaoCliente = async (codigo) => {
    const resposta = await enviarRequisicao(`/Cliente/Obter?codigo=${codigo}`);
    if (!resposta.ok) {
        exibirToast(resposta.dados?.mensagem || 'Erro ao carregar o cliente.', 'erro');
        return;
    }

    const cliente = resposta.dados.dados;
    elementosCliente.titulo.textContent = 'Editar cliente';
    limparValidacao();

    elementosCliente.codigo.value = cliente.codigoCliente;
    elementosCliente.razaoSocial.value = cliente.razaoSocial;
    elementosCliente.nomeFantasia.value = cliente.nomeFantasia;
    elementosCliente.tipoPessoa.value = cliente.tipoPessoa;
    elementosCliente.documento.value = cliente.documento;
    elementosCliente.cep.value = cliente.cep;
    elementosCliente.endereco.value = cliente.endereco;
    elementosCliente.complemento.value = cliente.complemento;
    elementosCliente.bairro.value = cliente.bairro;
    elementosCliente.cidade.value = cliente.cidade;
    elementosCliente.uf.value = cliente.uf;

    telefonesEditando = (cliente.telefones || []).map(t => ({
        numeroTelefone: t.numeroTelefone,
        codigoTipoTelefone: t.codigoTipoTelefone,
        operadora: t.operadora,
        ativo: t.ativo
    }));
    renderTelefones();

    clienteModal.show();
};

const salvarCliente = async (evento) => {
    evento.preventDefault();
    if (!validarFormulario()) {
        return;
    }

    const dto = {
        razaoSocial: elementosCliente.razaoSocial.value.trim(),
        nomeFantasia: elementosCliente.nomeFantasia.value.trim(),
        tipoPessoa: elementosCliente.tipoPessoa.value,
        documento: elementosCliente.documento.value.trim(),
        endereco: elementosCliente.endereco.value.trim(),
        complemento: elementosCliente.complemento.value.trim(),
        bairro: elementosCliente.bairro.value.trim(),
        cidade: elementosCliente.cidade.value.trim(),
        cep: elementosCliente.cep.value.trim(),
        uf: elementosCliente.uf.value.trim().toUpperCase(),
        telefones: telefonesEditando
    };

    const codigo = elementosCliente.codigo.value;
    const url = codigo ? `/Cliente/Atualizar?codigo=${codigo}` : '/Cliente/Criar';

    const resposta = await enviarRequisicao(url, 'POST', dto);
    if (!resposta.ok) {
        exibirToast(resposta.dados?.mensagem || 'Erro ao salvar o cliente.', 'erro');
        return;
    }

    clienteModal.hide();
    exibirToast(resposta.dados.mensagem || 'Cliente salvo.');
    carregarClientes();
};

const excluirCliente = async (codigo) => {
    if (!confirm('Deseja realmente excluir este cliente?')) {
        return;
    }

    const resposta = await enviarRequisicao(`/Cliente/Excluir?codigo=${codigo}`, 'POST');
    if (!resposta.ok) {
        exibirToast(resposta.dados?.mensagem || 'Erro ao excluir.', 'erro');
        return;
    }

    exibirToast(resposta.dados.mensagem || 'Cliente removido.');
    carregarClientes();
};
// #endregion

// #region [ VALIDACAO ]
const validarFormulario = () => {
    limparValidacao();
    let valido = true;

    if (elementosCliente.razaoSocial.value.trim() === '') {
        elementosCliente.razaoSocial.classList.add('is-invalid');
        valido = false;
    }
    if (elementosCliente.nomeFantasia.value.trim() === '') {
        elementosCliente.nomeFantasia.classList.add('is-invalid');
        valido = false;
    }
    if (elementosCliente.uf.value.trim().length !== 2) {
        elementosCliente.uf.classList.add('is-invalid');
        valido = false;
    }
    if (!validarDocumento(elementosCliente.tipoPessoa.value, elementosCliente.documento.value)) {
        elementosCliente.documento.classList.add('is-invalid');
        valido = false;
    }

    return valido;
};

const limparValidacao = () => {
    [elementosCliente.razaoSocial, elementosCliente.nomeFantasia, elementosCliente.uf, elementosCliente.documento]
        .forEach(campo => campo.classList.remove('is-invalid'));
};
// #endregion

// #region [ TELEFONES ]
const adicionarTelefone = () => {
    const numero = apenasNumeros(elementosCliente.telefoneNumero.value);
    if (numero.length < 8) {
        exibirToast('Informe um numero de telefone valido.', 'erro');
        return;
    }

    if (telefonesEditando.some(t => apenasNumeros(t.numeroTelefone) === numero)) {
        exibirToast('Este numero ja foi adicionado.', 'erro');
        return;
    }

    telefonesEditando.push({
        numeroTelefone: numero,
        codigoTipoTelefone: parseInt(elementosCliente.telefoneTipo.value, 10),
        operadora: elementosCliente.telefoneOperadora.value.trim(),
        ativo: true
    });

    elementosCliente.telefoneNumero.value = '';
    elementosCliente.telefoneOperadora.value = '';
    renderTelefones();
};

const removerTelefone = (indice) => {
    telefonesEditando.splice(indice, 1);
    renderTelefones();
};

const renderTelefones = () => {
    if (telefonesEditando.length === 0) {
        elementosCliente.telefoneLista.innerHTML =
            '<tr><td colspan="4" class="text-center text-muted">Nenhum telefone adicionado.</td></tr>';
        return;
    }

    elementosCliente.telefoneLista.innerHTML = telefonesEditando.map((t, indice) => `
        <tr>
            <td>${t.numeroTelefone}</td>
            <td>${descricaoTipo(t.codigoTipoTelefone)}</td>
            <td>${t.operadora || '-'}</td>
            <td class="text-end">
                <button type="button" class="btn btn-sm btn-outline-danger" data-indice="${indice}">Remover</button>
            </td>
        </tr>`).join('');
};
// #endregion
