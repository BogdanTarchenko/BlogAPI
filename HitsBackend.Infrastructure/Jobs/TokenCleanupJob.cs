using HitsBackend.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class TokenCleanupJob : IJob
{
    private readonly IBannedTokenRepository _bannedTokenRepository;
    private readonly ILogger<TokenCleanupJob> _logger;

    public TokenCleanupJob(
        IBannedTokenRepository bannedTokenRepository,
        ILogger<TokenCleanupJob> logger)
    {
        _bannedTokenRepository = bannedTokenRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _bannedTokenRepository.ClearAllAsync();
            _logger.LogInformation("Banned tokens table cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up tokens");
            throw;
        }
    }
}