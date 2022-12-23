using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using appFace;
using SixLabors.ImageSharp.Memory;

namespace WpfApp
{
    public class Image
    {
        public string Path { get; set; }
        public int Id { get; set; }
        public bool Position { get; set; }
        public string Name { get; set; }
        public byte[] Embedding { get; set; }
        public Image(string _Path, bool pos)
        {
            Path = _Path;
            var PathSplit = Path.Split("\\");
            Name = PathSplit.Last();
            Embedding = new byte[512*4];
            Position = pos;
        }
        public Image()
        {
            Embedding = null;
            Position = false;
        }
        
        public void LoadCalculations (float[] Calculations)
        {
            Embedding = new byte[Calculations.Length * 4];
            Buffer.BlockCopy(Calculations, 0, Embedding, 0, Embedding.Length);
        }
        public async Task Calculate(DataContext db, miilvFace mlnet, CancellationTokenSource cts)
        {
            byte[] img = await File.ReadAllBytesAsync(Path, cts.Token);
            var img_hashcode = img.GetHashCode();
            if (!db.Hashkeys.Any(h => h.hashkey.Equals(img_hashcode)))
            {
                if (!db.Bytedatas.Any(b => b.byte_image.Equals(img)))
                {
                    var Calculations = Task.Run(async () => await mlnet.GetEmbeddingsFromBytesAsync(img, cts)).Result;
                    Embedding = new byte[Calculations.Length * 4];
                    Buffer.BlockCopy(Calculations, 0, Embedding, 0, Embedding.Length);
                    db.Images.Add(this);
                    db.SaveChanges();
                    var image = db.Images.OrderBy(d => d.Id).Last();
                    db.Hashkeys.Add(new Hashkey { hashkey = img_hashcode, ImageId = image.Id });
                    db.Bytedatas.Add(new Bytedata { byte_image = img, ImageId = image.Id });
                    db.SaveChanges();
                }
            }
        }

    }

}
