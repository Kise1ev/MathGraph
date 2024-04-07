using NCalc;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MathGraph.Managers
{
    public static class GraphManager
    {
        private static readonly Dictionary<string, Func<double, double, double>> graphFunctions = new Dictionary<string, Func<double, double, double>>
        {
            { "sqrt", (x, y) => Math.Sqrt(x) },
            { "sin", (x, y) => Math.Sin(x) },
            { "cos", (x, y) => Math.Cos(x) },
            { "tan", (x, y) => Math.Tan(x) },
            { "cot", (x, y) => 1 / Math.Tan(x) },
            { "abs", (x, y) => Math.Abs(x) },
            { "exp", (x, y) => Math.Exp(x) },
            { "log", (x, y) => Math.Log(x) },
            { "log10", (x, y) => Math.Log10(x) },
            { "round", (x, y) => Math.Round(x) },
            { "floor", (x, y) => Math.Floor(x) },
            { "ceil", (x, y) => Math.Ceiling(x) },
            { "pow", (x, y) => Math.Pow(x, y)  },
            { "pi", (x, y) => Math.PI }
        };

        private static readonly List<string> forbiddenSymbols = new List<string>
        {
            "^",
            "&",
            "!",
            "@",
            "#",
            "$",
            "%",
            "`",
            "~",
            "_",
            "[",
            "]",
            "'",
            ";",
            ":",
            "?",
            "<",
            ">",
            "=",
            "№",
            "\"",
            "'",
            "|",
            "\\"
        };

        public static PlotModel MakeGraph(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                throw new NullReferenceException("Формула не может быть пустой!");

            List<string> forbiddenSymbolsInFormula = forbiddenSymbols.Where(symbol => formula.Contains(symbol)).Distinct().ToList();
            if (forbiddenSymbolsInFormula.Count > 0)
            {
                string forbiddenSymbolsString = string.Join(", ", forbiddenSymbolsInFormula);
                throw new ArgumentException($"Формула содержит запрещенные символы!\nЗапрещенные символы: {forbiddenSymbolsString}");
            }

            PlotModel plotModel = new PlotModel
            {
                Title = $"График f({formula})",
                DefaultFont = "Arial",
                DefaultFontSize = 15,
                TitleFont = "Arial",
                TitleFontSize = 20
            };

            LineSeries lineSeries = new LineSeries
            {
                Title = "График",
                Color = OxyColors.Blue,
                StrokeThickness = 3
            };

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (double x = -10; x <= 10; x += 0.1)
            {
                double y = EvaluateFormula(formula, x);
                lineSeries.Points.Add(new DataPoint(x, y));

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            double yRange = Math.Max(Math.Abs(minY), Math.Abs(maxY));
            double yStart = -yRange - 1;
            double yEnd = yRange + 1;

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = minX,
                Maximum = maxX,
                Title = "x"
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = yStart,
                Maximum = yEnd,
                Title = "y"
            });

            LineSeries xLineSeries = new LineSeries
            {
                Title = "x",
                Color = OxyColors.Black,
                StrokeThickness = 2
            };
            xLineSeries.Points.Add(new DataPoint(minX, 0));
            xLineSeries.Points.Add(new DataPoint(maxX, 0));
            plotModel.Series.Add(xLineSeries);

            LineSeries yLineSeries = new LineSeries
            {
                Title = "y",
                Color = OxyColors.Black,
                StrokeThickness = 2
            };
            yLineSeries.Points.Add(new DataPoint(0, yStart));
            yLineSeries.Points.Add(new DataPoint(0, yEnd));
            plotModel.Series.Add(yLineSeries);

            plotModel.Series.Add(lineSeries);

            return plotModel;
        }

        private static double EvaluateFormula(string formula, double x)
        {
            string modifiedFormula = Regex.Replace(formula, @"(\d)([a-zA-Z\(])", "$1*$2");
            Expression expression = new Expression(modifiedFormula);
            expression.EvaluateFunction += Expression_EvaluateFunction;
            expression.Parameters["x"] = x;
            return (double)expression.Evaluate();
        }

        private static void Expression_EvaluateFunction(string name, FunctionArgs args)
        {
            if (!graphFunctions.ContainsKey(name.ToLower()))
                throw new ArgumentException($"Функция {name.ToLower()} не поддерживается!");

            if (args.Parameters.Length < 1 || args.Parameters.Length > 2)
                throw new ArgumentException($"Неверное количество параметров в функции {name.ToLower()}!");

            double param1 = Convert.ToDouble(args.Parameters[0].Evaluate());
            double param2 = args.Parameters.Length == 2 ? Convert.ToDouble(args.Parameters[1].Evaluate()) : 0;

            Func<double, double, double> function = graphFunctions[name.ToLower()];
            args.Result = function(param1, param2);
        }
    }
}