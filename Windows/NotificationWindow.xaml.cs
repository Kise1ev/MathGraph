using System;
using System.Collections.ObjectModel;
using System.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MathGraph.Windows
{
    public partial class NotificationWindow : Window
    {
        private static readonly ObservableCollection<NotificationWindow> notificationsList = new ObservableCollection<NotificationWindow>();

        private DispatcherTimer DisplayTimer { get; }

        public NotificationWindow(string message, byte displayTime)
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Right - Width - 20;
            Top = SystemParameters.WorkArea.Bottom - Height - 20 * (notificationsList.Count + 1);
            
            DisplayTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(displayTime) };
            DisplayTimer.Tick += Timer_Tick;
            
            MessageTextBlock.Text = message.Replace(" ", "\u00A0");
            
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.2));
            BeginAnimation(OpacityProperty, animation);
            Show();
            
            notificationsList.Add(this);
            
            DisplayTimer.Start();
            
            using (SoundPlayer soundPlayer = new SoundPlayer(Properties.Resources.NotificationSound))
                soundPlayer.Play();
        }

        public static void Show(string message, byte displayTime) => new NotificationWindow(message, displayTime);

        public static void Show(string message) => new NotificationWindow(message, 5);

        private static void Close(NotificationWindow window)
        {
            if (window == null) return;
            
            window.DisplayTimer.Stop();
            
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(.2));
            animation.Completed += (s, _) =>
            {
                window.Close();
                notificationsList.Remove(window);
            };
            window.BeginAnimation(OpacityProperty, animation);
        }

        private void Timer_Tick(object sender, EventArgs e) => Close(this);

        private void ButtonClose_Click(object sender, RoutedEventArgs e) => Close(this);
    }
}