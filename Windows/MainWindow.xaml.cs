﻿using MathGraph.Managers;
using OxyPlot;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MathGraph.Windows
{
    public partial class MainWindow : Window
    {
        public static MainWindow CurrentInstance { get; set; }

        private readonly App currentApplication;

        public MainWindow()
        {
            InitializeComponent();
            CurrentInstance = this;
            currentApplication = Application.Current as App;
            PlotView.Model = GraphManager.MakeGraph("cos(x) + sin(x)");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            BeginAnimation(OpacityProperty, animation);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            CloseApplication();
        }

        private void CloseApplication()
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += (s, _) => currentApplication.Shutdown();
            BeginAnimation(OpacityProperty, animation);
        }

        private void ControlPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void ButtonClose_Click(object sender, RoutedEventArgs e) => CloseApplication();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += (s, _) => WindowState = WindowState.Minimized;
            BeginAnimation(OpacityProperty, animation);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
                BeginAnimation(OpacityProperty, animation);
            }
        }

        private void MakeGraphButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formulaLower = FormulaTxt.Text.ToLower();
                PlotModel plotModel = GraphManager.MakeGraph(formulaLower) ?? 
                    throw new NullReferenceException("Пожалуйста, исправьте формулу и повторите попытку!");
                PlotView.Model = GraphManager.MakeGraph(formulaLower);
            }
            catch (ArgumentException exception)
            {
                NotificationWindow.Show(exception.Message);
            }
            catch (NullReferenceException exception)
            {
                NotificationWindow.Show(exception.Message);
            }
            catch (Exception exception)
            {
                NotificationWindow.Show($"Неизвестная ошибка!\n{exception.Message}");
            }
        }
    }
}