using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SimulationTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace SimulationTool.ViewModels
{
    public class ThirdViewModel
    {
        public ObservableCollection<ISeries> Series { get; set; }

        public ThirdViewModel()
        {
            MeanReversionModel model = new MeanReversionModel();
            var prices = model.SimulatePrices();

            // 创建图表数据
            var lineSeries = new LineSeries<double>
            {
                //Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                Values = prices,
                Fill = null, // 去掉填充，使线条清晰
                GeometrySize = 1
                //GeometryStroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 }
            };

            Series = new ObservableCollection<ISeries> { lineSeries };
        }
    }
}
