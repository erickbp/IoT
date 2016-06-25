using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.ServiceBus.Messaging;

namespace RemoteController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static QueueClient _queueClient;
        private static string QueueName = "Robot";

        public MainWindow()
        {
            InitializeComponent();
            
            _queueClient = QueueClient.Create(QueueName);

        }

        private void GoFowardButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("F"))));
        }

        private void StopButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("S"))));
        }

        private void TurnRightButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("R"))));
        }

        private void GoBackwardButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("B"))));
        }

        private void TrunLeftButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("L"))));
        }

        private void TopRightButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("TR"))));
        }

        private void TopLeftButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("TL"))));
        }

        private void BottomRightButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("BR"))));
        }

        private void BottomLeftButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("BL"))));
        }

        private void StepperLeftButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("SML"))));
        }
        private void StepperRightButton(object sender, RoutedEventArgs e)
        {
            _queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes("SMR"))));
        }
    }
}
