using ImageWatermarkTool.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageWatermarkTool.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BitmapImage _displayedImage;

        public BitmapImage DisplayedImage
        {
            get => _displayedImage;
            set
            {
                _displayedImage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectImageCommand { get; set; }
        public ICommand AddTextWatermarkCommand { get; set; }
        public ICommand AddImageWatermarkCommand { get; set; }
        public ICommand ExportImageCommand { get; set; }

        private readonly ImageProcessingService _imageProcessingService;

        public MainViewModel()
        {
            _imageProcessingService = new ImageProcessingService();
            SelectImageCommand = new RelayCommand(SelectImage);
            AddTextWatermarkCommand = new RelayCommand(AddTextWatermark);
            AddImageWatermarkCommand = new RelayCommand(AddImageWatermark);
            ExportImageCommand = new RelayCommand(ExportImage);
        }

        private void SelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imageProcessingService.LoadMainImage(openFileDialog.FileName);
                UpdateDisplayedImage();
            }
        }

        private void AddTextWatermark()
        {
            var watermarkText = "Sample Watermark";
            _imageProcessingService.AddTextWatermark(watermarkText);
            UpdateDisplayedImage();
        }

        //private void AddImageWatermark()
        //{
        //    var watermarkPath = @"Images\watermark.png";
        //    _imageProcessingService.AddImageWatermark(watermarkPath);
        //    UpdateDisplayedImage();
        //}
        private void AddImageWatermark()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imageProcessingService.AddImageWatermark(openFileDialog.FileName);
                UpdateDisplayedImage();
            }
        }

        private void ExportImage()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _imageProcessingService.ExportImage(saveFileDialog.FileName);
            }
        }
        //private void ExportImage()
        //{
        //    var outputPath = @"Images\output.png";
        //    _imageProcessingService.ExportImage(outputPath);
        //}

        private void UpdateDisplayedImage()
        {
            var updatedBitmap = _imageProcessingService.GetPreviewImage();
            DisplayedImage = BitmapToImageSource(updatedBitmap);
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
