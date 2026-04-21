namespace Viamatica.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }

    void SoftDelete();
    void Restore();
}
