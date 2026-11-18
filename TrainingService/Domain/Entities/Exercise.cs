namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Enums;

public class Exercise
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Instructions { get; private set; } = string.Empty;
    public TargetMuscleGroup TargetMuscleGroup { get; private set; }
    public TrackingType TrackingType { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // Parameterless constructor for EF Core reflection
    private Exercise() { }

    public static Exercise Create(
        string name,
        string instructions,
        TargetMuscleGroup targetMuscleGroup,
        TrackingType trackingType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(instructions, nameof(instructions));

        return new Exercise
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Instructions = instructions.Trim(),
            TargetMuscleGroup = targetMuscleGroup,
            TrackingType = trackingType,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string instructions,
        TargetMuscleGroup targetMuscleGroup,
        TrackingType trackingType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(instructions, nameof(instructions));

        Name = name.Trim();
        Instructions = instructions.Trim();
        TargetMuscleGroup = targetMuscleGroup;
        TrackingType = trackingType;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
