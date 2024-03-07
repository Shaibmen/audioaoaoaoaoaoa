using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Path = System.IO.Path;

namespace audioaoaoaoaoaoao
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Кнопочек истории и рандома нет :((

        MediaPlayer mediaPlayer = new MediaPlayer();
        private List<string> audioFiles; 
        private int currentIndex = 0; 
        private DispatcherTimer positionUpdateTimer;
        private TimeSpan currentPosition;



        public MainWindow()
        {
            InitializeComponent();
            positionUpdateTimer = new DispatcherTimer();
            positionUpdateTimer.Interval = TimeSpan.FromMilliseconds(200); 
            positionUpdateTimer.Tick += PositionTimer_Tick;
            positionUpdateTimer.Start();


        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                PositionSlider.Value = mediaPlayer.Position.TotalSeconds;
                currentTimeLabel.Content = mediaPlayer.Position.ToString(@"m\:ss");
                remainingTimeLabel.Content = "-" + (mediaPlayer.NaturalDuration.TimeSpan - mediaPlayer.Position).ToString(@"m\:ss");
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog { IsFolderPicker = true };
            CommonFileDialogResult folderresult = folderDialog.ShowDialog();
            if (folderresult == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(folderDialog.FileName);
                string[] musicFiles = files.Where(file => Path.GetExtension(file) == ".mp3" || Path.GetExtension(file) == ".wav").ToArray(); // можно к этому доабвить много всяких расширений файлов, оно работает.
                List<string> musicFileList = new List<string>(musicFiles);
                PlaylistListBox.ItemsSource = musicFileList;
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistListBox.SelectedItem is string selectedFilePath)
            {
                if (mediaPlayer.Source != null && mediaPlayer.Position != TimeSpan.Zero)
                {
                    currentPosition = mediaPlayer.Position;
                    mediaPlayer.Stop();
                }
                else
                {
                    mediaPlayer.Open(new Uri(selectedFilePath));
                    mediaPlayer.Position = currentPosition;
                    mediaPlayer.Play();
                }
            }
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistListBox.SelectedItem is string selectedFilePath)
            {
                audioFiles = (List<string>)PlaylistListBox.ItemsSource;
                if (audioFiles.Count > 0 && currentIndex > 0)
                {
                    currentIndex--;
                    string previousFilePath = audioFiles[currentIndex];

                    if (mediaPlayer.Position != TimeSpan.Zero)
                    {
                        mediaPlayer.Open(new Uri(previousFilePath));
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(new Uri(previousFilePath));
                        mediaPlayer.Play();
                    }
                    positionUpdateTimer.Start();
                    PlaylistListBox.SelectedIndex = currentIndex;
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistListBox.SelectedItem is string selectedFilePath)
            {
                audioFiles = (List<string>)PlaylistListBox.ItemsSource;
                if (audioFiles.Count > 0 && currentIndex < audioFiles.Count - 1)
                {
                    currentIndex++;
                    string nextFilePath = audioFiles[currentIndex];

                    if (mediaPlayer.Position != TimeSpan.Zero)
                    {
                        mediaPlayer.Open(new Uri(nextFilePath));
                        mediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.Open(new Uri(nextFilePath));
                        mediaPlayer.Play();
                    }
                    positionUpdateTimer.Start();
                    PlaylistListBox.SelectedIndex = currentIndex;
                }
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistListBox.SelectedItem is string selectedFilePath)
            {
                mediaPlayer.Open(new Uri(selectedFilePath));
                mediaPlayer.Play();
                positionUpdateTimer.Start();
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan newPosition = TimeSpan.FromSeconds(PositionSlider.Value);
                mediaPlayer.Position = newPosition;
                Application.Current.Dispatcher.Invoke(() => // вроде работает, а в потоке или нет, кто его знает
                {
                    PositionSlider.Value = newPosition.TotalSeconds;
                    PositionSlider.Minimum = 0;
                    PositionSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                });
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue / 100.0; 
        }
    }
}