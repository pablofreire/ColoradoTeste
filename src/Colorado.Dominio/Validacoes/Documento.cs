namespace Colorado.Dominio.Validacoes;

// Validacao de CPF/CNPJ com digitos verificadores. Roda no servidor; o front
// repete a checagem em JS apenas para dar retorno imediato ao usuario.
public static class Documento
{
    public static string ApenasNumeros(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    // Normalizacao do documento: mantem letras e digitos (CNPJ alfanumerico a partir de 2026),
    // remove a pontuacao da mascara e padroniza em maiusculas.
    public static string Normalizar(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return string.Empty;

        return new string(valor.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
    }

    public static bool EhCpfValido(string cpf)
    {
        cpf = ApenasNumeros(cpf);

        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        var digitos = cpf.Select(c => c - '0').ToArray();

        int primeiro = CalcularDigito(digitos, 9, 10);
        int segundo = CalcularDigito(digitos, 10, 11);

        return digitos[9] == primeiro && digitos[10] == segundo;
    }

    // CNPJ alfanumerico (Receita, jul/2026): 12 posicoes alfanumericas + 2 digitos verificadores
    // numericos. Cada caractere vale (ASCII - 48) no calculo: '0'..'9' = 0..9, 'A'..'Z' = 17..42.
    // CNPJ numerico antigo continua valido (e subconjunto).
    public static bool EhCnpjValido(string cnpj)
    {
        cnpj = Normalizar(cnpj);

        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
            return false;

        // As 12 primeiras posicoes sao alfanumericas; as 2 ultimas (DV) sao sempre numericas.
        for (int i = 0; i < 12; i++)
            if (!char.IsLetterOrDigit(cnpj[i]))
                return false;

        if (!char.IsDigit(cnpj[12]) || !char.IsDigit(cnpj[13]))
            return false;

        var valores = cnpj.Select(c => c - '0').ToArray();
        int[] pesosPrimeiro = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] pesosSegundo = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int primeiro = CalcularDigitoCnpj(valores, pesosPrimeiro);
        int segundo = CalcularDigitoCnpj(valores, pesosSegundo);

        return valores[12] == primeiro && valores[13] == segundo;
    }

    // Para CPF o peso decresce a partir de um valor inicial (10 e depois 11).
    private static int CalcularDigito(int[] digitos, int quantidade, int pesoInicial)
    {
        int soma = 0;
        for (int i = 0; i < quantidade; i++)
            soma += digitos[i] * (pesoInicial - i);

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int CalcularDigitoCnpj(int[] digitos, int[] pesos)
    {
        int soma = 0;
        for (int i = 0; i < pesos.Length; i++)
            soma += digitos[i] * pesos[i];

        int resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
