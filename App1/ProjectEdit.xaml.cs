using System;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.UI.Controls;
using YetAnotherScriptingLanguage;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectEdit : Page
    {
        public Action<string, int> removeTab;
        static public Interpreter _interpreter = new Interpreter();
        int currentTranslationUnit = -1;
        public int CurrentTranslationUnit => currentTranslationUnit;
        public ConsoleBuild InnerConsole => this.ConsoleInterface;
        public ProjectEdit()
        {
            this.InitializeComponent();
            ConsoleInterface.HookEvents();
            this.Tabs.Items.VectorChanged += (IObservableVector<object> sender, IVectorChangedEventArgs @event) =>
            {
                bool SplitterEnabler = sender.Count > 0;
                Splitter.IsEnabled = SplitterEnabler;
                this.row1.Height = SplitterEnabler ? new GridLength(200, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
            };
            this.Tabs.TabClosing += (object sender, TabClosingEventArgs e) =>
            {
                var Path = (e.Tab.Content as TranslationUnitEdit).Path;
                var index = this.Tabs.Items.IndexOf(e.Item);
                removeTab(Path, index);
            };
        }

        public void SelectTab(int index)
        {
            this.Tabs.SelectedIndex = index;
        }

        public void AddTab(TabViewItem t)
        {
            this.Tabs.Items.Add(t);
        }

        public void reset()
        {
            _interpreter.Reset();
            ConsoleInterface.HookEvents();
            ConsoleInterface.clear();
        }

        public void Run()
        {
            try
            {
                TranslationUnit tes = new TranslationUnit(((Tabs.SelectedItem as TabViewItem).Content as TranslationUnitEdit).Code);
                var res = Parser.Process(tes.Tokens);
                reset();
            }
            catch (Exception _e)
            {
                this.ConsoleInterface.addText(_e.Message);
                this.ConsoleInterface.addText(_e.StackTrace);
                this.ConsoleInterface.addText(Environment.NewLine);
                reset();
            }
        }

        private void RunRequest_Click(object sender, RoutedEventArgs e)
        {
            if (currentTranslationUnit != -1)
            {
                Run();
            }
            else
            {
                this.ConsoleInterface.addText("Please Open a AYSL File");
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentTranslationUnit = (sender as Microsoft.Toolkit.Uwp.UI.Controls.TabView).SelectedIndex;
        }

        private async void Tabs_TabClosing(object sender, TabClosingEventArgs e)
        {
            var UIEditCode = e.Tab.Content as TranslationUnitEdit;
            await PathIO.WriteTextAsync(UIEditCode.Path, UIEditCode.Code);
        }
    }
}
