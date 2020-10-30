using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1.Prompts
{
    public sealed partial class NewProjectPrompt : ContentDialog
    {
        Windows.Storage.StorageFolder folder;
        public Windows.Storage.StorageFolder Folder => folder;
        string projectName;
        string initFileName;
        bool isInit= false;
        public NewProjectPrompt()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if(folder is null || String.IsNullOrWhiteSpace(projectName) || (isInit && String.IsNullOrWhiteSpace(initFileName)))
            {
                MessageDialog FillDataPrompt = new MessageDialog($"Please fill all the required Data");
                var okCommand = new UICommand("OK");
                this.Hide();
                var FillDataPromptResult = await FillDataPrompt.ShowAsync();
            }
            var f = await folder.CreateFolderAsync(projectName.Replace(" ", ""));
            folder = f;
            if (isInit)
                await f.CreateFileAsync(initFileName + ".aysl");
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            var f = await picker.PickSingleFolderAsync();
            if(!(f is null))
            {
                folder = f;
                this.ProjectPath.Text = f.Path;
            }
        }

        private void ProjectNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            projectName = (sender as TextBox).Text;
        }

        private void InitFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            initFileName = (sender as TextBox).Text;
        }

        private void IsInitiRequest_Toggled(object sender, RoutedEventArgs e)
        {
            isInit = (sender as ToggleSwitch).IsOn;
        }
    }
}
