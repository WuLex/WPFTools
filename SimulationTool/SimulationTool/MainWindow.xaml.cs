using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace SimulationTool
{
    public partial class MainWindow : Window
    {
        // 模拟参数
        private const double mu = 100;    // 长期均值（μ）
        private const double theta = 0.1; // 均值回归速度（θ）
        private const double sigma = 1;   // 波动率（σ）
        private const double initialPrice = 110; // 初始价格
        private const double timeStep = 0.01; // 时间步长
        private const int numSteps = 1000; // 模拟步骤数

        public MainWindow()
        {
            InitializeComponent();
            RunSimulation();
        }

        private void RunSimulation()
        {
            var plotModel = new PlotModel { Title = "均值回归模拟" };

            // 定义一个折线图
            var lineSeries = new LineSeries
            {
                Title = "价格变化",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            // 初始化价格
            double price = initialPrice;
            Random rand = new Random();

            // 存储时间和价格的列表
            List<double> times = new List<double>();
            List<double> prices = new List<double>();

            // 进行均值回归模拟
            for (int t = 0; t < numSteps; t++)
            {
                // 随机扰动（正态分布）
                double randomNoise = NormalRandom(rand, 0, 1);

                // Ornstein-Uhlenbeck过程公式
                price += theta * (mu - price) * timeStep + sigma * Math.Sqrt(timeStep) * randomNoise;

                // 记录时间和价格
                times.Add(t * timeStep);
                prices.Add(price);

                // 向LineSeries中添加数据
                lineSeries.Points.Add(new DataPoint(t * timeStep, price));
            }

            // 将折线图添加到PlotModel
            plotModel.Series.Add(lineSeries);

            // 将PlotModel绑定到PlotView控件
            PlotView.Model = plotModel;
        }

        // 生成标准正态分布随机数
        private static double NormalRandom(Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1) random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // Box-Muller transform
            return mean + stdDev * randStdNormal;
        }
    }
}
