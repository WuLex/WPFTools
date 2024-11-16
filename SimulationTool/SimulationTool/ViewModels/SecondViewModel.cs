using OxyPlot.Series;
using OxyPlot;
using SimulationTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationTool.ViewModels
{
    public class SecondViewModel
    {
        public PlotModel PricePlotModel { get; private set; }

        public SecondViewModel()
        {
            MeanReversionModel model = new MeanReversionModel();
            var prices = model.SimulatePrices();
            PricePlotModel = CreatePlotModel(prices, model.TimeStep);
        }

        private PlotModel CreatePlotModel(List<double> prices, double timeStep)
        {
            var plotModel = new PlotModel { Title = "均值回归模拟" };
            var lineSeries = new LineSeries
            {
                Title = "价格变化",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            for (int i = 0; i < prices.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i * timeStep, prices[i]));
            }

            plotModel.Series.Add(lineSeries);
            return plotModel;
        }
    }
}
