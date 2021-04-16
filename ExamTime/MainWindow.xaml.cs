using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace ExamTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            DateTime now = DateTime.Now;
            PerusalStart = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            _dispatcherTimer.Tick += TimerTick;
        }

        private string _perusalName = "Perusal";
        public string PerusalName
        {
            get => _perusalName;
            set
            {
                if (_perusalName != value)
                {
                    bool hadPerusal = HavePerusal;

                    _perusalName = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HavePerusal));

                    if (hadPerusal != HavePerusal)
                    {
                        if (HavePerusal)
                            ExamStart = PerusalStart.Add(PerusalTime);
                        else
                            ExamStart = PerusalStart;
                    }
                }
            }
        }


        //private bool _havePerusal;
        public bool HavePerusal
        {
            get => !string.IsNullOrEmpty(PerusalName);
        }


        /*
        private ICommand _setPerusal;
        public ICommand SetPerusal
        {
            get
            {
                if (_setPerusal == null)
                    _setPerusal = new RelayCommand(
                        param =>
                        {
                            if (param != null)
                            {
                                PerusalName = param.ToString();
                                HavePerusal = true;
                            }
                            else
                            {
                                HavePerusal = false;
                            }
                        }
                    );
                return _setPerusal;
            }
        }
        */



        private TimeSpan _perusalTime = new TimeSpan(0, 10, 0);
        public TimeSpan PerusalTime
        {
            get => _perusalTime;
            set
            {
                if (_perusalTime != value)
                {
                    _perusalTime = value;
                    NotifyPropertyChanged();

                    ExamStart = PerusalStart.Add(PerusalTime);
                }
            }
        }


        private TimeSpan _examTime = new TimeSpan(1, 0, 0);
        public TimeSpan ExamTime
        {
            get => _examTime;
            set
            {
                if (_examTime != value)
                {
                    _examTime = value;
                    NotifyPropertyChanged();

                    FinishTime = ExamStart.Add(ExamTime);
                }
            }
        }




        private DateTime _perusalStart;
        public DateTime PerusalStart
        {
            get => _perusalStart;
            set
            {
                if (_perusalStart != value)
                {
                    _perusalStart = value;
                    NotifyPropertyChanged();

                    ExamStart = PerusalStart.Add(PerusalTime);
                }
            }
        }


        private DateTime _examStart;
        public DateTime ExamStart
        {
            get => _examStart;
            set
            {
                if (_examStart != value)
                {
                    _examStart = value;
                    NotifyPropertyChanged();

                    FinishTime = ExamStart.Add(ExamTime);
                }
            }
        }


        private DateTime _finishTime;
        public DateTime FinishTime
        {
            get => _finishTime;
            set
            {
                if (_finishTime != value)
                {
                    _finishTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        
        public enum Section
        {
            Before,
            Perusal,
            Exam,
            After
        }

        public Section CurrentSection
        {
            get
            {
                DateTime now = DateTime.Now;
                if (HavePerusal)
                {
                    if (now.CompareTo(PerusalStart) < 0)
                        return Section.Before;
                    if (now.CompareTo(ExamStart) < 0)
                        return Section.Perusal;
                }
                else
                {
                    if (now.CompareTo(ExamStart) < 0)
                        return Section.Before;
                }
                if (now.CompareTo(FinishTime) < 0)
                    return Section.Exam;
                else
                    return Section.After;
            }
        }


        public string CurrentName
        {
            get
            {
                switch (CurrentSection)
                {
                    case Section.Perusal:
                        return PerusalName;
                    case Section.Exam:
                        return "Working";
                    case Section.After:
                        return "Finished";
                    default:
                        return string.Empty;
                }
            }
        }


        public Brush CurrentColour
        {
            get
            {
                switch (CurrentSection)
                {
                    case Section.Perusal:
                        return Brushes.LightBlue;
                    case Section.Exam:
                        return Brushes.LightGreen;
                    case Section.After:
                        return Brushes.Orange;
                    default:
                        return Brushes.Transparent;
                }
            }
        }


        public string TimeNow
        {
            get => DateTime.Now.ToString("T");
        }


        public string TimeRemaining
        {
            get
            {
                TimeSpan left = TimeSpan.Zero;
                switch (CurrentSection)
                {
                    case Section.Before:
                        if (HavePerusal)
                            left = PerusalTime;
                        else
                            left = ExamTime;
                        break;
                    case Section.Perusal:
                        left = PerusalTime.Subtract(DateTime.Now.Subtract(PerusalStart));
                        break;
                    case Section.Exam:
                        left = ExamTime.Subtract(DateTime.Now.Subtract(ExamStart));
                        break;
                }

                return left.ToString(@"hh\:mm\:ss");
            }
        }


        private void NotifyTimes()
        {
            NotifyPropertyChanged(nameof(CurrentSection));
            NotifyPropertyChanged(nameof(CurrentName));
            NotifyPropertyChanged(nameof(CurrentColour));
            NotifyPropertyChanged(nameof(TimeNow));
            NotifyPropertyChanged(nameof(TimeRemaining));
        }


        DispatcherTimer _dispatcherTimer;

        private void TimerTick(object sender, EventArgs e)
        {
            NotifyTimes();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
        }
    }
}
