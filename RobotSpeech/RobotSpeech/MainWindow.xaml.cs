using System.IO;
using System.Text;
using System.Windows;
using Microsoft.ProjectOxford.SpeechRecognition;
using Microsoft.ServiceBus.Messaging;

namespace RobotSpeech
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static string QueueName = "robotcar1";
        private static QueueClient _queueClient;
        private MicrophoneRecognitionClient _micClient;

        public MainWindow()
        {
            InitializeComponent();

            MyInit();
        }

        private void MyInit()
        {
           // IniWithoutLuis();

            InitWithLuis();
            _queueClient = QueueClient.Create(QueueName);
        }

        // ReSharper disable once UnusedMember.Local
        private void IniWithoutLuis()
        {
            const string language = "en-US";
            const string subscriptionKey = "30f97626b3144136a6c9f398172a61ac";

            _micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                SpeechRecognitionMode.ShortPhrase,
                language,
                subscriptionKey,
                subscriptionKey);

            _micClient.OnMicrophoneStatus += OnMicrophoneStatus;
            _micClient.OnPartialResponseReceived += OnPartialResponseReceived;
            _micClient.OnResponseReceived += OnResponseReceived;
            _micClient.OnConversationError += OnConversationError;
        }

        // ReSharper disable once UnusedMember.Local
        private void InitWithLuis()
        {
            const string language = "en-US";
            const string subscriptionKey = "30f97626b3144136a6c9f398172a61ac";
            const string luisAppId = "0090b094-7126-4ff8-81f3-06cd2a32a1de";
            const string luisSubscriptionId = "0d47bd4a03a149f490247fd01827401c";

            _micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClientWithIntent(
                language,
                subscriptionKey,
                subscriptionKey,
                luisAppId,
                luisSubscriptionId);

            _micClient.OnIntent += OnIntent;

            _micClient.OnMicrophoneStatus += OnMicrophoneStatus;
            _micClient.OnPartialResponseReceived += OnPartialResponseReceived;
            _micClient.OnResponseReceived += OnResponseReceived;
            _micClient.OnConversationError += OnConversationError;
        }

        private void OnConversationError(object sender, SpeechErrorEventArgs e)
        {
            WriteToLog($"SpeechErrorCode: {e.SpeechErrorCode}. SpeechErrorText: {e.SpeechErrorText}");
        }

        private void OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _micClient.EndMicAndRecognition();
                StartButton.IsEnabled = true;
            });
          
            var log = "";

            for (var i = 0; i < e.PhraseResponse.Results.Length; i++)
            {
                log +=
                    $"{i}. Confidence: {e.PhraseResponse.Results[i].Confidence}, Text: \"{e.PhraseResponse.Results[i].DisplayText}\"\r\n";
            }

            WriteToLog(log);
        }

        private void OnPartialResponseReceived(object sender, PartialSpeechResponseEventArgs e)
        {
            WriteToLog($"Partial Result: {e.PartialResult}");
        }

        private void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            WriteToLog(e.Recording
                ? "****************** I'm recording ************************"
                : "================== I'm NO recording =====================");
        }

        private void OnIntent(object sender, SpeechIntentEventArgs e)
        {
            WriteToLog(e.Payload);
            //_queueClient.Send(new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(e.Payload))));
        }

        private void StartOnClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            _micClient.StartMicAndRecognition();
        }

        private void WriteToLog(string log)
        {
            Dispatcher.Invoke(() =>
            {
                Log.Text += log + "\r\n";
            });
        }

    }
}
