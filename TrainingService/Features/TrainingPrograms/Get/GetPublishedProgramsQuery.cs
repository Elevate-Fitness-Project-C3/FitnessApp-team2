using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Features.TrainingPrograms.DTOs;
using TrainingService.Repositories.Interfaces;
using static TrainingService.TrainingProgram.Enums.Enums;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;

namespace TrainingService.Features.TrainingPrograms.Get
{
    public record GetPublishedProgramsQuery(
        string? Goal = null,
        string? Level = null,
        string? AccessTier = null,
        string? Search = null,
        bool HasPremiumAccess = false
    ) : IRequest<List<PublishedProgramDTO>>;

    public class GetPublishedProgramsQueryHandler : IRequestHandler<GetPublishedProgramsQuery, List<PublishedProgramDTO>>
    {
        private readonly IGenericRepository<TrainingProgramEntity> _repository;

        public GetPublishedProgramsQueryHandler(IGenericRepository<TrainingProgramEntity> repository)
        {
            _repository = repository;
        }

        public async Task<List<PublishedProgramDTO>> Handle(GetPublishedProgramsQuery request, CancellationToken cancellationToken)
        {
            var query = _repository.GetAll()
                .AsNoTracking()
                .Where(p => p.Status == ProgramStatus.Published);

            if (!string.IsNullOrWhiteSpace(request.Goal))
            {
                var goalFilter = request.Goal.Trim().ToLower();
                query = query.Where(p => p.Goal.ToLower() == goalFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.Level))
            {
                var levelFilter = request.Level.Trim().ToLower();
                query = query.Where(p => p.Level.ToLower() == levelFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.AccessTier))
            {
                if (Enum.TryParse<AccessTier>(request.AccessTier.Trim(), true, out var parsedTier))
                {
                    query = query.Where(p => p.AccessTier == parsedTier);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(term) || p.Description.ToLower().Contains(term));
            }

            // Direct LINQ Projection - zero Include overhead
            var programs = await query
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Goal,
                    p.Level,
                    p.DurationWeeks,
                    p.DaysPerWeek,
                    p.AccessTier,
                    p.Description
                })
                .ToListAsync(cancellationToken);

            return programs.Select(p => new PublishedProgramDTO
            {
                Id = p.Id,
                Title = p.Title,
                Goal = p.Goal,
                Level = p.Level,
                DurationWeeks = p.DurationWeeks,
                DaysPerWeek = p.DaysPerWeek,
                AccessTier = p.AccessTier.ToString(),
                IsLocked = p.AccessTier == AccessTier.Premium && !request.HasPremiumAccess,
                Description = p.Description
            }).ToList();
        }
    }
}
