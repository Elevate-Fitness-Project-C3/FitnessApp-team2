using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutritionService.Common;
using NutritionService.Common.Abstractions;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Errors;
using NutritionService.Domain.Repositories;

namespace NutritionService.Features.NutritionGoals.SetNutritionGoal;

public sealed class SetNutritionGoalHandler
    : IRequestHandler<SetNutritionGoalCommand, Result>
{
    private readonly INutritionPreferenceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISnowflakeIdGenerator _idGenerator;

    public SetNutritionGoalHandler(
        INutritionPreferenceRepository repository,
        IUnitOfWork unitOfWork,
        ISnowflakeIdGenerator idGenerator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _idGenerator = idGenerator;
    }

    public async Task<Result> Handle(
        SetNutritionGoalCommand command,
        CancellationToken cancellationToken)
    {
        var preference = await _repository.GetByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (preference is null)
        {
            var creationResult = NutritionPreference.Create(
                _idGenerator.NextId(),
                command.UserId,
                command.NutritionGoal!.Value);

            if (creationResult.IsFailure)
            {
                return Result.Failure(creationResult.Error);
            }

            _repository.Add(creationResult.Value);
        }
        else
        {
            var changeResult = preference.ChangeGoal(
                command.NutritionGoal!.Value);

            if (changeResult.IsFailure)
            {
                return changeResult;
            }
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlEx
                  && sqlEx.Number is 2601 or 2627)
        {
            return Result.Failure(NutritionPreferenceErrors.ConcurrentCreate);
        }

        return Result.Success();
    }
}
