// Helpers compartilhados pelas telas: toasts, chamadas ao servidor e validacao de documento.

// Mostra um toast de feedback no canto da tela.
const exibirToast = (mensagem, tipo = 'success') => {
    const container = document.getElementById('containerToast');
    if (!container) {
        alert(mensagem);
        return;
    }

    const cor = tipo === 'erro' ? 'text-bg-danger' : 'text-bg-success';
    const elemento = document.createElement('div');
    elemento.className = `toast align-items-center ${cor} border-0`;
    elemento.setAttribute('role', 'alert');
    elemento.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${mensagem}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>`;

    container.appendChild(elemento);
    const toast = new bootstrap.Toast(elemento, { delay: 4000 });
    toast.show();
    elemento.addEventListener('hidden.bs.toast', () => elemento.remove());
};

// GET que devolve o HTML de uma partial (tabela ou formulario).
const obterParcial = async (url) => {
    const resposta = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
    return { ok: resposta.ok, status: resposta.status, html: await resposta.text() };
};

// POST do formulario. O servidor responde JSON em caso de sucesso ou o HTML do
// formulario (com o resumo de validacao) em caso de erro.
const enviarFormulario = async (url, formData) => {
    const resposta = await fetch(url, {
        method: 'POST',
        body: formData,
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    const tipo = resposta.headers.get('content-type') || '';
    if (tipo.includes('application/json')) {
        return { ok: resposta.ok, json: await resposta.json() };
    }
    return { ok: resposta.ok, html: await resposta.text() };
};

// POST simples (ex.: excluir) que devolve JSON.
const postarJson = async (url, dados) => {
    const resposta = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: new URLSearchParams(dados)
    });

    let json = null;
    try {
        json = await resposta.json();
    } catch {
        json = null;
    }
    return { ok: resposta.ok, json };
};

const apenasNumeros = (valor) => (valor || '').replace(/\D/g, '');

// CPF: 11 digitos com os dois digitos verificadores conferidos.
const validarCpf = (valor) => {
    const cpf = apenasNumeros(valor);
    if (cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) {
        return false;
    }

    const calcularDigito = (qtd) => {
        let soma = 0;
        for (let i = 0; i < qtd; i++) {
            soma += parseInt(cpf[i], 10) * (qtd + 1 - i);
        }
        const resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    };

    return calcularDigito(9) === parseInt(cpf[9], 10)
        && calcularDigito(10) === parseInt(cpf[10], 10);
};

// CNPJ: 14 digitos validados com os pesos padrao da Receita.
const validarCnpj = (valor) => {
    const cnpj = apenasNumeros(valor);
    if (cnpj.length !== 14 || /^(\d)\1{13}$/.test(cnpj)) {
        return false;
    }

    const calcular = (pesos) => {
        let soma = 0;
        for (let i = 0; i < pesos.length; i++) {
            soma += parseInt(cnpj[i], 10) * pesos[i];
        }
        const resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    };

    const primeiro = calcular([5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
    const segundo = calcular([6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);

    return primeiro === parseInt(cnpj[12], 10) && segundo === parseInt(cnpj[13], 10);
};

const validarDocumento = (tipoPessoa, documento) =>
    tipoPessoa === 'F' ? validarCpf(documento) : validarCnpj(documento);
