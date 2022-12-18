using appFace;
using System;
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
        private List<List<Tuple<float, float>>> Calculations = new List<List<Tuple<float, float>>>();
        bool Canceled = false;
        bool CalculationInProgress = false;
        CancellationTokenSource cts;
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ImagePathsList_1.ItemsSource = ImagePathsList.Item1;
            ImagePathsList_2.ItemsSource = ImagePathsList.Item2;
            ImagePathsList_1.SelectionChanged += ImagePathsList_SelectionChanged;
            ImagePathsList_2.SelectionChanged += ImagePathsList_SelectionChanged;
            ProgressBar.Visibility = Visibility.Hidden;
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
                if (ImagePathsList.Item1.Count != 0 && ImagePathsList.Item2.Count != 0)
                {
                    ImagePathsList = Tuple.Create(new ObservableCollection<Image>(), new ObservableCollection<Image>());
                    Calculations = new List<List<Tuple<float, float>>>();
                }
                if ((bool)openFileDialog.ShowDialog())
                {
                    string[] ImageList_open = openFileDialog.FileNames;
                    if (!id)
                    {
                        foreach (string image in ImageList_open)
                        {
                            //ProcessedImages_1.Add(new ProcessedImage(image));
                            ImagePathsList.Item1.Add(new Image(image));
                        }
                    }
                    else
                    {
                        foreach (string image in ImageList_open)
                        {
                            //ProcessedImages_2.Add(new ProcessedImage(image));
                            ImagePathsList.Item2.Add(new Image(image));
                        }
                    }

                }
                ProgressBar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                
                cts = new CancellationTokenSource();
                MessageBox.Show(ex.Message);
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
        private async Task<Tuple<float, float>> Calculate(Image image1, Image image2, miilvFace mlnet, CancellationTokenSource cts)
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
                Similarity.Text = Calculations[index_1][index_2].Item2.ToString();
                Distance.Text = Calculations[index_1][index_2].Item1.ToString();
            }
           
        }
    }
}
