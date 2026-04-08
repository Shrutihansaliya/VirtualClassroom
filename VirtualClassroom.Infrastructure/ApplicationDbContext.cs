//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

//namespace VirtualClassroom.Infrastructure
//{
//    internal class ApplicationDbContext
//    {
//    }
//}

using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;

namespace VirtualClassroom.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<TblUsers> TblUsers { get; set; }
        public DbSet<TblUserLogins> TblUserLogins { get; set; }
        public DbSet<TblClassroom> TblClassroom { get; set; }
        public DbSet<TblClassroomMembers> TblClassroomMembers { get; set; }
        public DbSet<TblLectures> TblLectures { get; set; }
        public DbSet<TblAssignments> TblAssignments { get; set; }
        public DbSet<TblSubmissions> TblSubmissions { get; set; }
        public DbSet<TblMaterials> TblMaterials { get; set; }
        public DbSet<TblNotifications> TblNotifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TblUsers>()
    .Property(u => u.Role)
    .HasConversion<string>();

            modelBuilder.Entity<TblUsers>()
    .HasIndex(u => u.Email)
    .IsUnique();

            modelBuilder.Entity<TblUsers>()
    .Property(u => u.CreatedAt)
    .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TblUserLogins>()
    .HasOne(ul => ul.User)
    .WithMany()
    .HasForeignKey(ul => ul.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            // TblClassroom → TblUsers (Faculty)
            modelBuilder.Entity<TblClassroom>()
                .HasOne(c => c.Faculty)
                .WithMany(u => u.CreatedClassrooms)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // TblClassroomMembers
            modelBuilder.Entity<TblClassroomMembers>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ClassroomMembers)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TblClassroomMembers>()
                .HasOne(cm => cm.Classroom)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ClassroomId)
                .OnDelete(DeleteBehavior.Cascade);

            // TblLectures
            modelBuilder.Entity<TblLectures>()
                .HasOne(l => l.Classroom)
                .WithMany(c => c.Lectures)
                .HasForeignKey(l => l.ClassroomId);

            // TblAssignments
            modelBuilder.Entity<TblAssignments>()
                .HasOne(a => a.Classroom)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.ClassroomId);

            // TblSubmissions
            modelBuilder.Entity<TblSubmissions>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId);

            // TblMaterials
            modelBuilder.Entity<TblMaterials>()
                .HasOne(m => m.Classroom)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.ClassroomId);


            // TblNotifications
            modelBuilder.Entity<TblNotifications>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<TblAssignments>()
        .HasKey(a => a.AssignmentId);

            modelBuilder.Entity<TblClassroomMembers>()
    .HasIndex(cm => new { cm.ClassroomId, cm.UserId })
    .IsUnique();

            modelBuilder.Entity<TblLectures>()
    .HasOne(l => l.Faculty)
    .WithMany()
    .HasForeignKey(l => l.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TblAssignments>()
    .HasOne(a => a.Faculty)
    .WithMany()
    .HasForeignKey(a => a.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TblSubmissions>()
    .HasOne(s => s.Student)
    .WithMany()
    .HasForeignKey(s => s.StudentId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TblMaterials>()
    .HasOne(m => m.Faculty)
    .WithMany()
    .HasForeignKey(m => m.UploadedBy)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}