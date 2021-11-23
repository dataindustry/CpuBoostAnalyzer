using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CpuBoostAnalyzer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private readonly List<PerformanceCounter> corePerformanceCounters = new();
        private readonly List<PerformanceCounter> powerPerformanceCounters = new();
        private readonly List<CpuDataSource> cpuDataSources = new();
        private readonly List<EnergyDataSource> energyDataSources = new();

        private readonly DispatcherQueueTimer timer;

        private readonly String licenseKey = "NTM4NDg5QDMxMzkyZTMzMmUzMFVVcmhxVWt0OHV2L1ZWZ1NEbE04RHV1YUVEZENlbTBTeHFySWMzTytlalk9";

        private ViewModel viewModel;

        public MainWindow()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

            this.InitializeComponent();

            this.InitializeCorePerformanceCounter();

            this.InitializePowerPerformanceCounter();

            viewModel = new();

            ccTest.DataContext = viewModel;

            foreach (var counter in corePerformanceCounters)
            {

                CpuDataSource dataSource = new();

                Rectangle rec = new();

                rec.Height = 10;
                rec.Fill = new SolidColorBrush(Colors.Black);

                rec.HorizontalAlignment = HorizontalAlignment.Left;
                rec.StrokeThickness = 1;
                rec.Stroke = new SolidColorBrush(Colors.White);
                rec.StrokeDashArray = new DoubleCollection() { 5 };
                rec.StrokeDashCap = PenLineCap.Flat;

                spCpu.Children.Add(rec);

                Binding bind = new();
                bind.Source = dataSource;
                bind.Path = new PropertyPath("CoreUsage");
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                rec.SetBinding(Rectangle.WidthProperty, bind);

                cpuDataSources.Add(dataSource);

            }

            foreach (var counter in powerPerformanceCounters)
            {

                EnergyDataSource dataSource = new();

                TextBlock tb = new();
                tb.HorizontalAlignment = HorizontalAlignment.Left;

                spEnergy.Children.Add(tb);

                Binding bind = new();
                bind.Source = dataSource;
                bind.Path = new PropertyPath("Power");
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tb.SetBinding(TextBlock.TextProperty, bind);

                energyDataSources.Add(dataSource);

            }

            this.timer = this.DispatcherQueue.CreateTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

        }

        private void Timer_Tick(DispatcherQueueTimer sender, object args)
        {
            for (int i = 0; i < this.corePerformanceCounters.Count; i++)
            {
                this.cpuDataSources[i].CoreUsage = (int)this.corePerformanceCounters[i].NextValue();
            }

            for (int i = 0; i < this.powerPerformanceCounters.Count; i++)
            {
                // this.energyDataSources[i].Power = (int)this.powerPerformanceCounters[i].NextValue();
            }

            this.viewModel.Data.RemoveAt(0);
            this.viewModel.Data.Add(new Person { Name = "T", Height = powerPerformanceCounters[1].NextValue() });
            
        }

        private void InitializeCorePerformanceCounter()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var counter = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
                this.corePerformanceCounters.Add(counter);
            }
        }

        private void InitializePowerPerformanceCounter()
        {
            powerPerformanceCounters.Add(new PerformanceCounter("Energy Meter", "Power", "RAPL_Package0_DRAM"));
            powerPerformanceCounters.Add(new PerformanceCounter("Energy Meter", "Power", "RAPL_Package0_PKG"));
            powerPerformanceCounters.Add(new PerformanceCounter("Energy Meter", "Power", "RAPL_Package0_PP0"));
            powerPerformanceCounters.Add(new PerformanceCounter("Energy Meter", "Power", "RAPL_Package0_PP1"));
        }

    }

    public class CpuDataSource : INotifyPropertyChanged
    {
        private int _CoreUsage;

        public int CoreUsage
        {
            get { return _CoreUsage; }
            set
            {
                _CoreUsage = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(CoreUsage)));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class EnergyDataSource : INotifyPropertyChanged
    {
        private int _Power;

        public int Power
        {
            get { return _Power; }
            set
            {
                _Power = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Power)));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Person
    {
        public string Name { get; set; }

        public double Height { get; set; }
    }

    public class ViewModel
    {
        public ObservableCollection<Person> Data { get; set; }

        public ViewModel()
        {
            Data = new ObservableCollection<Person>()
            {
                new Person { Name = "1", Height = 0 },
                new Person { Name = "2", Height = 0 },
                new Person { Name = "3", Height = 0 },
                new Person { Name = "4", Height = 0 },
                new Person { Name = "5", Height = 0 },
                new Person { Name = "6", Height = 0 },
                new Person { Name = "7", Height = 0 },
                new Person { Name = "8", Height = 0 },
                new Person { Name = "9", Height = 0 },
                new Person { Name = "10", Height = 0 }
            };
        }
    }
}
