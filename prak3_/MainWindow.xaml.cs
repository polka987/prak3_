using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.WebRequestMethods;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace prak3_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool pognali = false;
        static bool music_on = false;
        static bool music_repeat = false;
        static bool music_random = false;
        static bool pause = false;
        Dictionary<string, string> musics = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vikl();
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.Title = "Выберите папку, где находится музыка";
            CommonFileDialogResult opened = dlg.ShowDialog();
            if (opened != CommonFileDialogResult.Ok)
                return;
            foreach (var a in Directory.GetFiles(dlg.FileName))
            {
                if (a.EndsWith("mp3"))
                {
                    string name = "";
                    for (int i = a.Length - 5; i > -1; i--)
                    {
                        if (a[i] == '\\')
                            break;
                        name += a[i];
                    }
                    string eman = "";
                    for (int i = name.Length - 1; i >= 0; i--)
                    {
                        eman += name[i];
                    }
                    if (!musics.ContainsKey(eman))
                    {
                        musics.Add(eman, a);
                        lb.Items.Add(eman);
                    }
                }
            }
            lb.SelectedIndex = 0;
            pognali = true;
        }
        void vikl()
        {
            pognali = false;
            music_on = false;
            music_repeat = false;
            music_random = false;
            time_finish.Content = "";
            time_start.Content = "";
            slider.Value = 0;
            media.Stop();
            lb.Items.Clear();
            musics.Clear();
        }
        bool stope()
        {
            if (!pognali)
            {
                MessageBox.Show("Подожди, нужно сначала выбрать папку с музыкой");
                return true;
            }
            else
                return false;
        }
        private async void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            slider.Value = 0;
            if ((sender as ListBox).SelectedIndex != -1)
            {
                music_on = true;
                await music_play();
            }
        }
        async Task<int> music_play()
        {
            media.Source = new Uri(musics[lb.SelectedItem.ToString()]);
            media.Play();
            if (slider.Value != 0)
                media.Position = new TimeSpan((int)slider.Value);
            while (music_on)
            {
                slider.Value = media.Position.Ticks;
                time_start.Content = media.Position.ToString("mm\\:ss");
                await System.Threading.Tasks.Task.Delay(1000);
            }
            return 0;
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
            time_finish.Content = media.NaturalDuration.TimeSpan.ToString("mm\\:ss");
        }

        private async void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            music_on = false;
            if (!pause)
            {
                if (music_repeat)
                {
                    if (lb.SelectedIndex + 1 != -1)
                    {
                        lb.SelectedIndex--;
                        lb.SelectedIndex++;
                    }
                    else
                    {
                        lb.SelectedIndex++;
                        lb.SelectedIndex--;
                    }
                }
                else if (music_random)
                    lb.SelectedIndex = new Random().Next(lb.Items.Count);
                else if (lb.SelectedIndex + 1 == lb.Items.Count)
                    lb.SelectedIndex = 0;
                else
                    lb.SelectedIndex++;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (stope())
                return;
            if (lb.SelectedIndex + 1 == lb.Items.Count)
                lb.SelectedIndex = 0;
            else
                lb.SelectedIndex++;
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            if (stope())
                return;
            if (lb.SelectedIndex - 1 == -1)
                lb.SelectedIndex = lb.Items.Count-1;
            else
                lb.SelectedIndex--;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (stope())
                return;
            if (music_on)
            {
                pause = true;
                music_on = false;
                media.Stop();
            }
            else
            {
                pause = false;
                music_on = true;
                music_play();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (stope())
                return;
            music_repeat = !music_repeat;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (stope())
                return;
            music_random = !music_random;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan((int)slider.Value);
        }
    }
}