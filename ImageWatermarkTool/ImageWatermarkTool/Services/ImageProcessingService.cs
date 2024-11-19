using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageWatermarkTool.Services
{
    public class ImageProcessingService
    {
        private Bitmap _mainImage;
        private List<Watermark> _watermarks;

        public ImageProcessingService()
        {
            _watermarks = new List<Watermark>();
            // Load the main image
            //_mainImage = new Bitmap("path\\to\\mainImage.jpg");
        }

        public void LoadMainImage(string imagePath)
        {
            _mainImage = new Bitmap(imagePath);
            _watermarks.Clear();
        }


        public void AddTextWatermark(string text)
        {
            var watermark = new Watermark
            {
                Type = WatermarkType.Text,
                Text = text,
                Position = new PointF(100, 100),
                Font = new Font("Arial", 36),
                Transparency = 0.5f
            };
            _watermarks.Add(watermark);
            //DrawWatermarks();
        }

        public void AddImageWatermark(string imagePath)
        {
            var watermarkImage = new Bitmap(imagePath);
            var watermark = new Watermark
            {
                Type = WatermarkType.Image,
                Image = watermarkImage,
                Position = new PointF(100, 100),
                Transparency = 0.5f
            };
            _watermarks.Add(watermark);
            //DrawWatermarks();
        }

        //private void DrawWatermarks()
        //{
        //    using (var g = Graphics.FromImage(_mainImage))
        //    {
        //        foreach (var watermark in _watermarks)
        //        {
        //            if (watermark.Type == WatermarkType.Text)
        //            {
        //                var textBrush = new SolidBrush(Color.FromArgb((int)(watermark.Transparency * 255), Color.Black));
        //                g.DrawString(watermark.Text, watermark.Font, textBrush, watermark.Position);
        //            }
        //            else if (watermark.Type == WatermarkType.Image)
        //            {
        //                var imageBrush = new TextureBrush(watermark.Image);
        //                imageBrush.Transform = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, watermark.Position.X, watermark.Position.Y);
        //                g.FillRectangle(imageBrush, new RectangleF(watermark.Position, new SizeF(100, 100)));
        //            }
        //        }
        //    }
        //}
        public Bitmap GetPreviewImage()
        {
            var previewImage = (Bitmap)_mainImage.Clone();

            using (var g = Graphics.FromImage(previewImage))
            {
                foreach (var watermark in _watermarks)
                {
                    if (watermark.Type == WatermarkType.Text)
                    {
                        var brush = new SolidBrush(Color.FromArgb((int)(watermark.Transparency * 255), Color.Black));
                        g.DrawString(watermark.Text, watermark.Font, brush, watermark.Position);
                    }
                    else if (watermark.Type == WatermarkType.Image)
                    {
                        var attributes = new System.Drawing.Imaging.ImageAttributes();
                        var matrix = new System.Drawing.Imaging.ColorMatrix
                        {
                            Matrix33 = watermark.Transparency
                        };
                        attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                        g.DrawImage(watermark.Image, new Rectangle((int)watermark.Position.X, (int)watermark.Position.Y,
                            watermark.Image.Width, watermark.Image.Height), 0, 0, watermark.Image.Width, watermark.Image.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }

            return previewImage;
        }
        //public void ExportImage(string outputPath)
        //{
        //    _mainImage.Save(outputPath, ImageFormat.Png);
        //}

        public void ExportImage(string outputPath)
        {
            var finalImage = GetPreviewImage();
            finalImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public enum WatermarkType
    {
        Text,
        Image
    }

    public class Watermark
    {
        public WatermarkType Type { get; set; }
        public string Text { get; set; }
        public Bitmap Image { get; set; }
        public PointF Position { get; set; }
        public float Transparency { get; set; }
        public Font Font { get; set; }
    }
}
