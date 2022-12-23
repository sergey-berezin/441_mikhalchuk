using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class DataContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        //public DbSet<Result> Results { get; set; }
        public DbSet<Hashkey> Hashkeys { get; set; }

        public DbSet<Bytedata> Bytedatas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=ImageAnalysis.db");
    }



    public class Result
    {
        public int ResultId { get; set; }

        public float[] embedding { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }

    }

    public class Hashkey
    {
        public int hashkey { get; set; }

        [Key]
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }

    public class Bytedata
    {
        public byte[] byte_image { get; set; }

        [Key]
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
