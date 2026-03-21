using ParticleEmitter.PropEditor;
using SolidShineUi;
using SolidShineUi.KeyboardShortcuts;
using System.Drawing.Drawing2D;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ParticleEmitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        KeyboardShortcutHandler ksh;

        public MainWindow()
        {
            InitializeComponent();

            ksh = new KeyboardShortcutHandler(this);
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F5,
                new ActionKeyAction(Start, "Start", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.Ctrl, Key.F6,
                new ActionKeyAction(Pause, "Pause", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.Delete,
                new ActionKeyAction(Clear, "Clear", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F8,
                new ActionKeyAction(ShowHideSettings, "Settings", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F9,
                new ActionKeyAction(ChangeBackground, "Background", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F10,
                new ActionKeyAction(ShowHideTheme, "Theme", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F11,
                new ActionKeyAction(Fullscreen, "Fullscreen", this));
            ksh.KeyRegistry.RegisterKeyShortcut(KeyboardCombination.None, Key.F12,
                new ActionKeyAction(ShowHideUi, "ShowHideUi", this));

            SsuiTheme = new SsuiAppTheme(SystemColors.AccentColorLight1);
            selAccent.IsSelected = true;

            pl.UnregisterEditor(typeof(uint));
            pl.RegisterEditor(typeof(uint), typeof(LongEditor));
            pl.ShowInheritedProperties = false;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pl.LoadObject(emitter);
            pl.ShowInheritedProperties = false;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            emitter.Shutdown();
        }

        bool hasStarted = false;

        void Start()
        {
            if (!hasStarted)
            {
                emitter.Start();
                hasStarted = true;
            }
            else
            {
                emitter.Resume();
            }
        }

        void Pause()
        {
            emitter.Pause();
        }

        void Clear()
        {
            emitter.Clear();
        }

        void ShowHideSettings()
        {
            if (brdrSettings.Visibility == Visibility.Visible)
            {
                brdrSettings.Visibility = Visibility.Collapsed;
                btnSettings.IsSelected = false;
            }
            else
            {
                brdrSettings.Visibility = Visibility.Visible;
                pl.ReloadObject();
                pl.ShowInheritedProperties = false;
                btnSettings.IsSelected = true;
            }
        }

        void ShowHideTheme()
        {
            if (selTheme.Visibility == Visibility.Visible)
            {
                selTheme.Visibility = Visibility.Collapsed;
                btnTheme.IsSelected = false;
            }
            else
            {
                selTheme.Visibility = Visibility.Visible;
                btnTheme.IsSelected = true;
            }
        }

        void ChangeBackground()
        {
            Color windowCol = Colors.White;
            if (Background is SolidColorBrush scb)
            {
                windowCol = scb.Color;
            }
            ColorPickerDialog cpd = new ColorPickerDialog(windowCol)
            {
                SsuiTheme = SsuiTheme,
                Owner = this
            };

            cpd.ShowDialog();
            if (cpd.DialogResult)
            {
                Background = new SolidColorBrush(cpd.SelectedColor);
            }
        }

        void Fullscreen()
        {
            if (WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
        }

        void ShowHideUi()
        {
            if (toolbar.Visibility == Visibility.Visible)
            {
                toolbar.Visibility = Visibility.Collapsed;
            }
            else
            {
                toolbar.Visibility = Visibility.Visible;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowHideSettings();
        }

        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
        }

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            ShowHideTheme();
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            Fullscreen();
        }

        private void btnShowHideUi_Click(object sender, RoutedEventArgs e)
        {
            ShowHideUi();
        }

        private void selTheme_SelectionChanged(object sender, RoutedSelectionChangedEventArgs<IClickSelectableControl> e)
        {
            IClickSelectableControl? ics = selTheme.Items.SelectedItems.FirstOrDefault();

            if (ics != null)
            {
                switch (ics.Name)
                {
                    case nameof(selAccent):
                        SsuiTheme = new SsuiAppTheme(SystemColors.AccentColorLight1);
                        break;
                    case nameof(selDark):
                        SsuiTheme = SsuiThemes.CreateDarkTheme(SystemColors.AccentColorLight1);
                        break;
                    case nameof(selLight):
                        SsuiTheme = SsuiThemes.CreateLightTheme(SystemColors.AccentColorLight1);
                        break;
                    case nameof(selHc1):
                        SsuiTheme = SsuiThemes.HighContrastWhiteOnBlack;
                        break;
                    case nameof(selHc2):
                        SsuiTheme = SsuiThemes.HighContrastGreenOnBlack;
                        break;
                    case nameof(selHc3):
                        SsuiTheme = SsuiThemes.HighContrastBlackOnWhite;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}