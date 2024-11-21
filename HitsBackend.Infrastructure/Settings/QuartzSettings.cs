namespace Infrastructure.Settings;

public class QuartzSettings
{
    public required TokenCleanupJobSettings TokenCleanupJob { get; init; }
}

public class TokenCleanupJobSettings
{
    public required string CronSchedule { get; init; }
} 