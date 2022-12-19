using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using appFace;

namespace WpfApp
{
    public class Image
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public Image(string _Path)
        {
            Path = _Path;
            var PathSplit = Path.Split("\\");
            Name = PathSplit.Last();
        }
        /*
        public Tuple<float, float>? Result { get; set; }
        
        public void LoadCalculations (Tuple<float, float> Calulations)
        {
            Result = Calulations;
        }
        public async Task Calculate(CancellationTokenSource cts)
        {
            var mlnet = new miilvFace();
            var compareson = Task.Run(async () => await mlnet.CompareAsync(pair, cts));
            //бардак
            //нам надо в мэйнвиндов зафигачить этот таск с двумя выбранными пикчами
        }
        */
    }

}
