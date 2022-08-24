using System.Diagnostics.CodeAnalysis;
using DesafioDtiBack.Models;

namespace DesafioDtiBack;

public static class Rules
{
    // Crédito: https://www.macoratti.net/11/09/c_val1.htm
    // Adaptado para usar novas features do C#
    private static bool ValidateCnpj(string? cnpj)
    {
        if (string.IsNullOrEmpty(cnpj)) return false;
        var multiplicador1 = new[] {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
        var multiplicador2 = new[] {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
        cnpj = cnpj.Trim();
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        if (cnpj.Length != 14) return false;
        var tempCnpj = cnpj[..12];
        var soma = 0;
        for (var i = 0; i < 12; i++) soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        var digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;
        for (var i = 0; i < 13; i++) soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();
        return cnpj.EndsWith(digito);
    }

    private static bool ValidateCpf(string? cpf)
    {
        if (string.IsNullOrEmpty(cpf)) return false;
        var multiplicador1 = new[] {10, 9, 8, 7, 6, 5, 4, 3, 2};
        var multiplicador2 = new[] {11, 10, 9, 8, 7, 6, 5, 4, 3, 2};
        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");
        if (cpf.Length != 11) return false;
        var tempCpf = cpf[..9];
        var soma = 0;
        for (var i = 0; i < 9; i++) soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;
        for (var i = 0; i < 10; i++) soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();
        return cpf.EndsWith(digito);
    }
    
    public static bool CheckRequest(LoanRequest loan, out string message)
    {
        // rules
        switch (loan.Type)
        {
            // if PF, document must be an valid CPF
            case PersonType.PF when !ValidateCpf(loan.Document):
                message = "CPF inválido";
                return false;
            // if PJ, document must be an valid CNPJ
            case PersonType.PJ when !ValidateCnpj(loan.Document):
                message = "CNPJ inválido";
                return false;
        }
        
        loan.Name = (loan.Name ?? "").Trim();
    
        // name must have at least 3 characters and maximum of 100
        if (loan.Name.Length is < 3 or > 100)
        {
            message = "Nome inválido";
            return false;
        }
    
        // if requested loan is greater than 50000, deny
        if (loan.RequestedLoan > 50000)
        {
            message = "Valor pedido é maior que R$ 50.000,00";
            return false;
        }
    
        // if requested loan is greater than half of the current debt, deny
        if (loan.RequestedLoan > loan.CurrentDebt / 2)
        {
            message = "Valor pedido é maior que a metade da dívida ativa";
            return false;
        }
    
        // else, approve
        message = "Empréstimo aprovado";
        return true;
    }
}