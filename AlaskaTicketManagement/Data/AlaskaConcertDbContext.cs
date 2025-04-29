using AlaskaTicketManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace AlaskaTicketManagement.Data
{

    public class AlaskaConcertDbContext : DbContext
    {
        public AlaskaConcertDbContext(DbContextOptions<AlaskaConcertDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship between Event and Venue. One Venue to many events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany()
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);     

            // Relationship between Ticket and Event. One event has many tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);            
            
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Tickets)
                .WithOne()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
