namespace Finance.Api.Services;

public interface IFinanceService
{
    Task<decimal> GetTotalBalance(int accountId);
}