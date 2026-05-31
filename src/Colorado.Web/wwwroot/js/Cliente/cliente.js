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
    clienteModal = bootstrap.Modal.getOrCreateInstance(modalElemento);

    // Ao fechar: descarta a instancia, limpa o container e remove qualquer backdrop
    // remanescente (evita a camada escura que travava a tela).
    modalElemento.addEventListener('hidden.bs.modal', () => {
        clienteModal.dispose();
        clienteModal = null;
        elementosCliente.containerModal.innerHTML = '';
        document.querySelectorAll('.modal-backdrop').forEach((b) => b.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('overflow');
        document.body.style.removeProperty('padding-right');
    }, { once: true });

    document.getElementById('idClienteForm').addEventListener('submit', salvar);
    document.getElementById('idTelefoneAdicionar').addEventListener('click', adicionarTelefone);
    document.getElementById('idTelefoneLinhas').addEventListener('click', (evento) => {
        const remover = evento.target.closest('button[data-remover-telefone]');
        if (remover) remover.closest('tr').remove();
    });

    aplicarMascaras();
    clienteModal.show();
};

// Mascaras de exibicao (IMask). Sao apenas visuais: o servidor normaliza os valores.
const mascaraTelefone = { mask: [{ mask: '(00) 0000-0000' }, { mask: '(00) 00000-0000' }] };

const aplicarMascaraTelefone = (input) => {
    if (input && window.IMask) IMask(input, mascaraTelefone);
};

const aplicarMascaras = () => {
    const form = document.getElementById('idClienteForm');

    const documento = form.querySelector('[name="Documento"]');
    const tipoPessoa = form.querySelector('[name="TipoPessoa"]');

    // Opcoes de mascara por tipo: CPF (11 digitos) ou CNPJ alfanumerico (12 alfanum + 2 digitos).
    const opcoesDocumento = (ehFisica) => ehFisica
        ? { mask: '000.000.000-00', prepare: undefined }
        : { mask: '##.###.###/####-00', definitions: { '#': /[0-9A-Za-z]/ }, prepare: (valor) => valor.toUpperCase() };

    // Importante criar a mascara ja no tipo correto: senao o CNPJ (14) carregado na edicao
    // seria truncado pela mascara de CPF (11) antes do ajuste.
    const docMask = (window.IMask && documento)
        ? IMask(documento, opcoesDocumento(tipoPessoa.value === 'F'))
        : null;

    // Ajusta mascara, rotulos e visibilidade conforme o tipo de pessoa.
    const ajustarPorTipoPessoa = () => {
        const ehFisica = tipoPessoa.value === 'F';

        // Trocar o tipo invalida o contexto: limpa erros inline e o resumo do servidor.
        limparValidacao(form);
        const caixaErros = document.getElementById('idClienteErros');
        if (caixaErros) caixaErros.classList.add('d-none');

        if (docMask) docMask.updateOptions(opcoesDocumento(ehFisica));

        const lblDocumento = document.getElementById('lblDocumento');
        if (lblDocumento) lblDocumento.textContent = ehFisica ? 'CPF' : 'CNPJ';

        const lblRazaoSocial = document.getElementById('lblRazaoSocial');
        if (lblRazaoSocial) lblRazaoSocial.textContent = ehFisica ? 'Nome completo' : 'Razao social';

        // Pessoa fisica nao tem nome fantasia: esconde a coluna e limpa erro pendente.
        const colNomeFantasia = document.getElementById('colNomeFantasia');
        if (colNomeFantasia) {
            colNomeFantasia.classList.toggle('d-none', ehFisica);
            if (ehFisica) limparInvalido(colNomeFantasia.querySelector('[name="NomeFantasia"]'));
        }
    };

    if (tipoPessoa) {
        tipoPessoa.addEventListener('change', ajustarPorTipoPessoa);
        ajustarPorTipoPessoa();
    }

    if (window.IMask) {
        const cep = form.querySelector('[name="Cep"]');
        if (cep) IMask(cep, { mask: '00000-000' });
        form.querySelectorAll('[name$=".NumeroTelefone"]').forEach(aplicarMascaraTelefone);
    }
};

// Mostra a lista de erros do servidor dentro do modal aberto (sem reconstruir o modal).
const exibirErrosCliente = (erros) => {
    const caixa = document.getElementById('idClienteErros');
    if (!caixa) return;

    const itens = (erros && erros.length) ? erros : ['Nao foi possivel salvar o cliente.'];
    caixa.innerHTML = itens.map((e) => `<div>${e}</div>`).join('');
    caixa.classList.remove('d-none');
};

// Validacao client-side: espelha o SalvarClienteValidator. Usuario so e exigido na criacao.
const validarFormularioCliente = (form) => {
    const ehEdicao = !!form.querySelector('[name="Codigo"]')?.value;

    const ehFisica = form.querySelector('[name="TipoPessoa"]').value === 'F';

    const regras = [
        { campo: 'RazaoSocial', mensagem: ehFisica ? 'O nome completo e obrigatorio.' : 'A razao social e obrigatoria.', teste: (v) => v.trim().length > 0 },
        { campo: 'RazaoSocial', mensagem: 'Maximo de 200 caracteres.', teste: (v) => v.trim().length <= 200 },
        { campo: 'Documento', mensagem: 'O documento e obrigatorio.', teste: (v) => v.trim().length > 0 },
        {
            campo: 'Documento',
            mensagem: 'Documento invalido para o tipo de pessoa.',
            teste: (v) => validarDocumento(form.querySelector('[name="TipoPessoa"]').value, v)
        },
        { campo: 'Uf', mensagem: 'A UF e obrigatoria.', teste: (v) => v.trim().length === 2 }
    ];

    // Nome fantasia so e exigido para pessoa juridica.
    if (!ehFisica) {
        regras.push(
            { campo: 'NomeFantasia', mensagem: 'O nome fantasia e obrigatorio.', teste: (v) => v.trim().length > 0 },
            { campo: 'NomeFantasia', mensagem: 'O nome fantasia deve ter no maximo 200 caracteres.', teste: (v) => v.trim().length <= 200 }
        );
    }

    if (!ehEdicao) {
        regras.push(
            { campo: 'Usuario', mensagem: 'O usuario e obrigatorio.', teste: (v) => v.trim().length > 0 },
            { campo: 'Usuario', mensagem: 'O usuario deve ter no maximo 50 caracteres.', teste: (v) => v.trim().length <= 50 }
        );
    }

    let valido = validarRegras(form, regras);

    // Grade de telefones: valida cada linha inline (numero 8-11 digitos, tipo > 0).
    form.querySelectorAll('#idTelefoneLinhas tr').forEach((linha) => {
        const numero = linha.querySelector('[name$=".NumeroTelefone"]');
        const tipo = linha.querySelector('[name$=".CodigoTipoTelefone"]');

        if (numero) {
            limparInvalido(numero);
            const digitos = apenasNumeros(numero.value).length;
            if (digitos < 8 || digitos > 11) {
                marcarInvalido(numero, 'Informe um telefone com 8 a 11 digitos.');
                numero.addEventListener('input', () => limparInvalido(numero), { once: true });
                valido = false;
            }
        }
        if (tipo) {
            limparInvalido(tipo);
            if (!(parseInt(tipo.value, 10) > 0)) {
                marcarInvalido(tipo, 'Selecione o tipo de telefone.');
                tipo.addEventListener('change', () => limparInvalido(tipo), { once: true });
                valido = false;
            }
        }
    });

    return valido;
};

const salvar = async (evento) => {
    evento.preventDefault();
    const formulario = document.getElementById('idClienteForm');

    if (!validarFormularioCliente(formulario)) return;

    const resposta = await enviarFormulario('/Cliente/Salvar', new FormData(formulario));

    if (resposta.json && resposta.json.sucesso) {
        clienteModal.hide();
        exibirToast(resposta.json.mensagem);
        carregarTabela(paginaAtual);
        return;
    }

    exibirErrosCliente(resposta.json ? resposta.json.erros : null);
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
    const linhas = document.getElementById('idTelefoneLinhas');
    linhas.insertAdjacentHTML('beforeend', html);

    // A linha recem-clonada precisa da mascara de telefone aplicada.
    const novoTelefone = linhas.lastElementChild?.querySelector('[name$=".NumeroTelefone"]');
    aplicarMascaraTelefone(novoTelefone);
};
