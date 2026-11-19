using FitnessProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Infrastructure.Persistence
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(
       DbContextOptions<ProfileDbContext> options)
       : base(options)
        {
        }

        public DbSet<FitnessProfile> FitnessProfiles { get; set; }

        public DbSet<BodyMeasurement> BodyMeasurements {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ProfileDbContext).Assembly);
        }
    }
}
