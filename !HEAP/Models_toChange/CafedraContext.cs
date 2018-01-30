using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;
namespace GoMySql.Models
{
    class CafedraContext : DbContext
    {
        public DbSet<admin> admins { get; set; }
        public DbSet<avtor> avtors { get; set; }
        public DbSet<book> books { get; set; }
        public DbSet<disciplina> disciplinas { get; set; }
        public DbSet<group> groups { get; set; }
        public DbSet<kafedra> kafedras { get; set; }
        public DbSet<mail> mails { get; set; }
        public DbSet<person> people { get; set; }
        public DbSet<plat> plats { get; set; }
        public DbSet<specialistic> specialistics { get; set; }
        public DbSet<student> students { get; set; }
        public DbSet<teacher> teachers { get; set; }
        public DbSet<thread> threads { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<users2> users2 { get; set; }
        public DbSet<variant> variants { get; set; }
        public DbSet<vedet> vedets { get; set; }
        public DbSet<vid_grifa> vid_grifa { get; set; }
        public DbSet<vid_izdatelstva> vid_izdatelstva { get; set; }
        public DbSet<vidacha> vidachas { get; set; }
        public DbSet<work> works { get; set; }
        public DbSet<zadanie> zadanies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseMySQL(@"server=localhost;user id=root;password=kagura;persistsecurityinfo=True;database=www0005_base;");
        }
    }
}
