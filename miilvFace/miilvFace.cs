using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace appFace{
    public class miilvFace
    {
        private InferenceSession session; 
        static Semaphore semaphore = new Semaphore(1, 1);
        float Length(float[] v) => (float)Math.Sqrt(v.Select(x => x*x).Sum());
        float[] Normalize(float[] v) 
        {
            var len = Length(v);
            return v.Select(x => x / len).ToArray();
        }
        DenseTensor<float> ImageToTensor(Image<Rgb24> img)
    {
        var w = img.Width;
        var h = img.Height;
        var t = new DenseTensor<float>(new[] { 1, 3, h, w });

        img.ProcessPixelRows(pa => 
            {
                for (int y = 0; y < h; y++)
                {           
                    Span<Rgb24> pixelSpan = pa.GetRowSpan(y);
                    for (int x = 0; x < w; x++)
                    {
                        t[0, 0, y, x] = pixelSpan[x].R;
                        t[0, 1, y, x] = pixelSpan[x].G;
                        t[0, 2, y, x] = pixelSpan[x].B;
                    }
                }
            });
            
            return t;
        }
        
        float[] GetEmbeddings(Image<Rgb24> face) 
        {
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("data", ImageToTensor(face)) };
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);
            return Normalize(results.First(v => v.Name == "fc1").AsEnumerable<float>().ToArray());
        }
        float Distance(float[] v1, float[] v2) => Length(v1.Zip(v2).Select(p => p.First - p.Second).ToArray());

        float Similarity(float[] v1, float[] v2) => v1.Zip(v2).Select(p => p.First * p.Second).Sum();
        public Tuple<float, float> Compare(Tuple<byte[], byte[]> img){
            using var face1 = Image.Load<Rgb24>(img.Item1);
            using var face2 = Image.Load<Rgb24>(img.Item2);
            var embeddings1 = GetEmbeddings(face1);
            var embeddings2 = GetEmbeddings(face2);
            return Tuple.Create(Distance(embeddings1, embeddings2), Similarity(embeddings1, embeddings2));
        }

        public async Task<Tuple<float, float>> CompareAsync(Tuple<byte[], byte[]> img, CancellationTokenSource cts){
            var task0 = Task.Run(async () => 
            {
                var stream1 = new MemoryStream(img.Item1);
                return await Image.LoadAsync<Rgb24>(stream1, cts.Token);
            });
            var task1 = Task.Run(async () => 
            {
                var stream2 = new MemoryStream(img.Item2);
                return await Image.LoadAsync<Rgb24>(stream2, cts.Token);
            });
            await Task.WhenAll(task0, task1);
            if(cts.Token.IsCancellationRequested){
                return Tuple.Create<float, float>(0,0);
            }
            Image<Rgb24> face1 = task0.Result;
            Image<Rgb24> face2 = task1.Result;
            float[] embeddings1 = await Task.Run(() => GetEmbeddings(face1), cts.Token);
            if(cts.Token.IsCancellationRequested){
                return Tuple.Create<float, float>(0,0);
            }
            float[] embeddings2 = await Task.Run(() => GetEmbeddings(face2), cts.Token);
            if(cts.Token.IsCancellationRequested){
                return Tuple.Create<float, float>(0,0);
            }
            return Tuple.Create(Distance(embeddings1, embeddings2), Similarity(embeddings1, embeddings2));
        }

         async private Task<float[]> GetEmbeddingsAsync(Image<Rgb24> face) 
        {
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("data", ImageToTensor(face)) };
           var output = new float[0];
            await Task.Factory.StartNew(() =>
            {
                semaphore.WaitOne();   
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);
                semaphore.Release();
                output = Normalize(results.First(v => v.Name == "fc1").AsEnumerable<float>().ToArray());
            }, TaskCreationOptions.LongRunning);
            //using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);
            return output;
        }
        public miilvFace(){
            using var modelStream = typeof(miilvFace).Assembly.GetManifestResourceStream("arcface.onnx");
            using var memoryStream = new MemoryStream();
            if (modelStream != null)
                modelStream.CopyTo(memoryStream);
            session = new InferenceSession(memoryStream.ToArray());
            //session = new InferenceSession("miilvFace/arcfaceresnet100-8.onnx");
            //cts = new CancellationTokenSource();
        }
    }
}