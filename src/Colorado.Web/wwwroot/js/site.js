// Funcoes compartilhadas entre as telas (requisicoes, toasts e validacao de documento).

// #region [ REQUISICOES HTTP ]
const enviarRequisicao = async (url, metodo = 'GET', corpo = null) => {
    const opcoes = { method: metodo, headers: { 'Content-Type': 'application/json' } };
    if (corpo !== null) {
        opcoes.body = JSON.stringify(corpo);
    }

    const resposta = await fetch(url, opcoes);
    let dados = null;
    try {
        dados = await resposta.json();
    } catch {
        dados = null;
    }

    return { ok: resposta.ok, status: resposta.status, dados: dados };
};
// #endregion

// #region [ TOASTS ]
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
// #endregion

// #region [ VALIDACAO DE DOCUMENTO ]
const apenasNumeros = (valor) => (valor || '').replace(/\D/g, '');

// CPF: 11 digitos com os dois digitos verificadores conferidos manualmente.
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

const validarDocumento = (tipoPessoa, documento) => {
    return tipoPessoa === 'F' ? validarCpf(documento) : validarCnpj(documento);
};
// #endregion
