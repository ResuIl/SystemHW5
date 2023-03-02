using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SystemHW5;

public partial class MainWindow : Window
{
    private int _numValue = 0;

    public int NumValue
    {
        get { return _numValue; }
        set
        {
            _numValue = value;
            tBox_Num.Text = value.ToString();
        }
    }

    public ObservableCollection<Thread> Created { get; set; }
    public ObservableCollection<Thread> Waiting { get; set; }
    public ObservableCollection<Thread> Working { get; set; }

    private readonly Semaphore semaphore;

    int count = 0;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        tBox_Num.Text = _numValue.ToString();
        Created = new();
        Waiting = new();
        Working = new();

        semaphore = new(3, 3, "SEMAPHORE");
    }

    private void btn_DownClick(object sender, RoutedEventArgs e)
    {
        if (NumValue > 0)
            NumValue--;
        else
            MessageBox.Show("Semaphore places must greater than 0", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void btn_UpClick(object sender, RoutedEventArgs e)
    {
        if (NumValue < 15)
            NumValue++;
        else
            MessageBox.Show("Semaphore places must less than 15", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (tBox_Num == null)
            return;

        if (!int.TryParse(tBox_Num.Text, out _numValue))
            tBox_Num.Text = _numValue.ToString();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var t = new Thread(ThreadWork);
        t.Name = "Thread " + count++;

        Created.Add(t);
    }

    private void ThreadWork(object? semaphore)
    {
        if (semaphore is Semaphore s)
        {
            Thread.Sleep(4000);
            if (s.WaitOne())
            {
                try
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() => Waiting.Remove(t));
                    Dispatcher.Invoke(() => Working.Add(t));
                    Thread.Sleep(8000);
                    Dispatcher.Invoke(() => Working.Remove(t));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    s.Release();
                }


            }
        }
    }

    private void lBox_CreatedThreads_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (lBox_CreatedThreads.SelectedItem is Thread t)
        {
            Created.Remove(t);

            Waiting.Add(t);
            t.Start(semaphore);
        }
    }
}
