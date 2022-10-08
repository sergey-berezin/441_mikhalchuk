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
        Tuple <byte[], byte[]> pair = Tuple.Create(img1, img2);
        var mlnet = new miilvFace();
        //var result = mlnet.Compare(pair);
        Console.WriteLine("Sending Images to model.");
        var compareson = Task.Run(async () => await mlnet.CompareAsync(pair));
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

        return 0;
    }
    
}


