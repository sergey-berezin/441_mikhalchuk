using appFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp
{


    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Tuple<ObservableCollection<Image>, ObservableCollection<Image>> ImagePathsList = Tuple.Create(new ObservableCollection<Image>(), new ObservableCollection<Image>());
        //private ObservableCollection<ProcessedImage> ProcessedImages_1 = new ObservableCollection<ProcessedImage>();
        //private ObservableCollection<ProcessedImage> ProcessedImages_2 = new ObservableCollection<ProcessedImage>();
        //private List<List<Tuple<float, float>>> Calculations = new List<List<Tuple<float, float>>>();
        bool Canceled = false;
        bool CalculationInProgress = false;
        CancellationTokenSource cts;
        miilvFace mlnet;


        public MainWindow()
        {
            InitializeComponent();
            Load();
            this.DataContext = this;
            ImagePathsList_1.ItemsSource = ImagePathsList.Item1;
            ImagePathsList_2.ItemsSource = ImagePathsList.Item2;
            ImagePathsList_1.SelectionChanged += ImagePathsList_SelectionChanged;
            ImagePathsList_2.SelectionChanged += ImagePathsList_SelectionChanged;
            ProgressBar.Visibility = Visibility.Hidden;
            mlnet = new miilvFace();
        }
        //дальше наполняем функцию ввода изобр, потом обработки
        public event PropertyChangedEventHandler? PropertyChanged;

        private void LoadImages(object sender, bool id, RoutedEventArgs? e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff;*";
                
                if ((bool)openFileDialog.ShowDialog())
                {
                    string[] ImageList_open = openFileDialog.FileNames;
                    using (var db = new DataContext())
                    {
                        if (!id)
                        {
                            foreach (string image in ImageList_open)
                            {
                                //ProcessedImages_1.Add(new ProcessedImage(image));
                                ImagePathsList.Item1.Add(new Image(image, id));
                            }
                        }
                        else
                        {
                            foreach (string image in ImageList_open)
                            {
                                //ProcessedImages_2.Add(new ProcessedImage(image));
                                ImagePathsList.Item2.Add(new Image(image, id));
                            }
                        }
                    }

                }
                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Load()
        {
            using (var db = new DataContext())
            {
                if (db.Images.Any())
                {
                    foreach (var item in db.Images)
                    {
                        if (item.Embedding != null)
                        {
                            if (!item.Position)
                            {
                                ImagePathsList.Item1.Add(item);
                            }
                            else
                            {
                                ImagePathsList.Item2.Add(item);
                            }
                        }
                    }
                }
            }
        }
        private void LoadImages_Button_Click_1(object sender, RoutedEventArgs? e = null)
        {
            LoadImages(sender, false, e);
        }
        private void LoadImages_Button_Click_2(object sender, RoutedEventArgs? e = null)
        {
            LoadImages(sender, true, e);
        }
        /*
         * private async Task<Tuple<float, float>> Calculate(Image image1, Image image2, miilvFace mlnet, CancellationTokenSource cts)
        {

            var compareson = await Task.Run(async () =>
            {
                var pic1 = await File.ReadAllBytesAsync(image1.Path, cts.Token);
                var pic2 = await File.ReadAllBytesAsync(image2.Path, cts.Token);
                Tuple<byte[], byte[]> pair = Tuple.Create(pic1, pic2);
                var res = await mlnet.CompareAsync(pair, cts);
                return res;
            }, cts.Token);
            return compareson;
        }
        
        private async void ProcessImages_Button_Click(object sender, RoutedEventArgs? e = null)
        {
            Canceled = false;
            cts = new CancellationTokenSource();
            //mlnet = new miilvFace();
            if (ImagePathsList.Item1.Count == 0)
            {
                MessageBox.Show("No images in List 1 selected yet!");
            }
            else if (ImagePathsList.Item2.Count == 0)
            {
                MessageBox.Show("No images in List 2 selected yet!");
            }
            else if (!CalculationInProgress)
            {
                ProgressBar.Visibility = Visibility.Visible;
                CalculationInProgress = true;
                try
                {
                    miilvFace mlnet = new miilvFace();
                    ProgressBar.Maximum = ImagePathsList.Item1.Count * ImagePathsList.Item2.Count;
                    for (int i = 0; i < ImagePathsList.Item1.Count; i++)
                    {
                        List<Tuple<float, float>> line = new List<Tuple<float, float>>();
                        for (int j = 0; j < ImagePathsList.Item2.Count; j++)
                        {
                            Tuple<float, float> pair = await Calculate(ImagePathsList.Item1[i], ImagePathsList.Item2[j], mlnet, cts);
                            line.Add(pair);
                            ProgressBar.Value++;
                        }
                        Calculations.Add(line);

                    }
                    CalculationInProgress = false;
                    MessageBox.Show("Completed!");
                }
                catch (TaskCanceledException)
                {

                    MessageBox.Show("Task was cancelled!");
                    CalculationInProgress = false;
                }
            }

        }
        */
        private async void ProcessImages_Button_Click(object sender, RoutedEventArgs? e = null)
        {
            Canceled = false;
            cts = new CancellationTokenSource();
            //mlnet = new miilvFace();
            if (ImagePathsList.Item1.Count == 0)
            {
                MessageBox.Show("No images in List 1 selected yet!");
            }
            else if (ImagePathsList.Item2.Count == 0)
            {
                MessageBox.Show("No images in List 2 selected yet!");
            }
            else if (!CalculationInProgress)
            {
                ProgressBar.Visibility = Visibility.Visible;
                CalculationInProgress = true;
                try
                {
                    //miilvFace mlnet = new miilvFace();
                    using (var db = new DataContext())
                    {
                        ProgressBar.Maximum = ImagePathsList.Item1.Count + ImagePathsList.Item2.Count;
                        for (int i = 0; i < ImagePathsList.Item1.Count; i++)
                        {
                            await ImagePathsList.Item1[i].Calculate(db, mlnet, cts);
                            ProgressBar.Value++;
                        }
                        for (int j = 0; j < ImagePathsList.Item2.Count; j++)
                        {
                            await ImagePathsList.Item2[j].Calculate(db, mlnet, cts);
                            ProgressBar.Value++;
                        }
                        CalculationInProgress = false;
                        MessageBox.Show("Completed!");
                    }
                }
                catch (TaskCanceledException)
                {

                    MessageBox.Show("Task was cancelled!");
                    CalculationInProgress = false;
                }
            }

        }
        private async void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Canceled)
            {
                cts.Cancel();
                ProgressBar.Value = 0;
                CalculationInProgress = false;
                ProgressBar.Visibility = Visibility.Hidden;
                Canceled = true;
            }
            else
            {
                MessageBox.Show("Task not started!");

            }
        }
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        public void ImagePathsList_SelectionChanged(object sender, EventArgs e)
        {
            int index_1 = ImagePathsList_1.SelectedIndex;
            int index_2 = ImagePathsList_2.SelectedIndex;
            if (index_1 != -1 && index_2 != -1)
            {
                var Calculations_1 = new float[ImagePathsList.Item1[index_1].Embedding.Length / 4];
                Buffer.BlockCopy(ImagePathsList.Item1[index_1].Embedding, 0, Calculations_1, 0, ImagePathsList.Item1[index_1].Embedding.Length);
                var Calculations_2 = new float[ImagePathsList.Item2[index_2].Embedding.Length / 4];
                Buffer.BlockCopy(ImagePathsList.Item2[index_2].Embedding, 0, Calculations_2, 0, ImagePathsList.Item2[index_2].Embedding.Length);


                var compareson = Task.Run(async () => await mlnet.CompareFromPrecalculatedAsync(Calculations_1, Calculations_2));
                Similarity.Text = compareson.Result.Item2.ToString();
                Distance.Text = compareson.Result.Item1.ToString();
            }

        }
    }
}
