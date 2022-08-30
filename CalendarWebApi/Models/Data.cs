using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CalendarWebApi.Models
{
    public class AppointmentSlot
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [JsonIgnore()]
        public User User { get; set; }

        [JsonPropertyName("text")]
        public string? PartnerName { set; get; }

        [JsonPropertyName("partner")]
        public string? PartnerId { set; get; }

        public string Status { get; set; } = "free";

        [NotMapped]
        public int Resource { get { return User.Id; } }

        [NotMapped]
        public string UserName { get { return User.Name; } }

    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<AppointmentSlot> Appointments { get; set; }
        public DbSet<User> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Existing User 1" });
            modelBuilder.Entity<User>().HasData(new User { Id = 2, Name = "Existing User 2" });
            modelBuilder.Entity<User>().HasData(new User { Id = 3, Name = "Existing User 3" });

        }
    }
}
