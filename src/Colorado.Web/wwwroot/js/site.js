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

// ---------------------------------------------------------------------------
// Validacao client-side (sem postback). Marca os campos com o padrao do Bootstrap
// (.is-invalid + .invalid-feedback) para dar retorno imediato antes de chamar a API.
// ---------------------------------------------------------------------------

// Garante um <div class="invalid-feedback"> logo apos o input e o preenche.
const marcarInvalido = (input, mensagem) => {
    if (!input) return;
    input.classList.add('is-invalid');

    let feedback = input.nextElementSibling;
    if (!feedback || !feedback.classList.contains('invalid-feedback')) {
        feedback = document.createElement('div');
        feedback.className = 'invalid-feedback';
        input.insertAdjacentElement('afterend', feedback);
    }
    feedback.textContent = mensagem;
};

const limparInvalido = (input) => {
    if (!input) return;
    input.classList.remove('is-invalid');
    const feedback = input.nextElementSibling;
    if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.textContent = '';
    }
};

// Remove todos os estados de erro do formulario.
const limparValidacao = (form) => {
    form.querySelectorAll('.is-invalid').forEach(limparInvalido);
};

// Roda uma lista de regras { campo, mensagem, teste }. Marca os invalidos, foca o
// primeiro e devolve true quando tudo passou. Cada regra tambem liga um listener
// 'input' que limpa o proprio erro assim que o usuario corrige.
const validarRegras = (form, regras) => {
    limparValidacao(form);

    let primeiroInvalido = null;
    for (const regra of regras) {
        const campo = form.querySelector(`[name="${regra.campo}"]`);
        if (!campo) continue;

        if (!regra.teste(campo.value, campo)) {
            marcarInvalido(campo, regra.mensagem);
            primeiroInvalido = primeiroInvalido || campo;
            campo.addEventListener('input', () => limparInvalido(campo), { once: true });
        }
    }

    if (primeiroInvalido) {
        primeiroInvalido.focus();
        return false;
    }
    return true;
};

const apenasNumeros = (valor) => (valor || '').replace(/\D/g, '');

// Documento normalizado: mantem letras e digitos (CNPJ alfanumerico), em maiusculas.
const normalizarDocumento = (valor) => (valor || '').replace(/[^0-9A-Za-z]/g, '').toUpperCase();

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

// CNPJ alfanumerico (Receita, jul/2026): 12 posicoes alfanumericas + 2 DVs numericos. Cada
// caractere vale (charCode - 48); CNPJ numerico antigo continua valido.
const validarCnpj = (valor) => {
    const cnpj = normalizarDocumento(valor);
    if (!/^[0-9A-Z]{12}[0-9]{2}$/.test(cnpj) || /^(.)\1{13}$/.test(cnpj)) {
        return false;
    }

    const valorChar = (c) => c.charCodeAt(0) - 48;

    const calcular = (pesos) => {
        let soma = 0;
        for (let i = 0; i < pesos.length; i++) {
            soma += valorChar(cnpj[i]) * pesos[i];
        }
        const resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    };

    const primeiro = calcular([5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
    const segundo = calcular([6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);

    return primeiro === valorChar(cnpj[12]) && segundo === valorChar(cnpj[13]);
};

const validarDocumento = (tipoPessoa, documento) =>
    tipoPessoa === 'F' ? validarCpf(documento) : validarCnpj(documento);
