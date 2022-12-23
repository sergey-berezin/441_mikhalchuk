using System;
using System.IO;
using appFace;
using System.Threading;
using System.Threading.Tasks;

class Program{
    static async Task<int> Main(string[] args){
        Console.WriteLine("Starting the compareson.");
        var img1 = File.ReadAllBytes("img/face1.png");
        var img2 = File.ReadAllBytes("img/face2.png");
        CancellationTokenSource cts = new CancellationTokenSource();
        Tuple <byte[], byte[]> pair = Tuple.Create(img1, img2);
        var mlnet = new miilvFace();
        //var result = mlnet.Compare(pair);
        Console.WriteLine("Sending Images to model async.");
        //var compareson = Task.Run(async () => await mlnet.CompareAsync(pair));
        var emb1 = Task.Run(async () => await mlnet.GetEmbeddingsFromBytesAsync(img1, cts));
        var emb2 = Task.Run(async () => await mlnet.GetEmbeddingsFromBytesAsync(img2, cts));
        await Task.WhenAll(emb1, emb2);
        Console.WriteLine("Got both images");
        var compareson = Task.Run(async () => await mlnet.CompareFromPrecalculatedAsync(emb1.Result, emb2.Result));
        Console.Write("Working -");
        while (!compareson.IsCompleted){
            await Task.Delay(1);
            Console.Write("\b\\");
            await Task.Delay(1);
            Console.Write("\b|");
            await Task.Delay(1);
            Console.Write("\b/");
            await Task.Delay(1);
            Console.Write("\b-");
        }
        Console.Write("\b\b\b\b\b\b\b\b\b");
        var result = compareson.Result;
        Console.WriteLine($"{result.Item1}, {result.Item2}");
        Console.WriteLine(emb2.Result.Length);

        return 0;
    }
    
}


