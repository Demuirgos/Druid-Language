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
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Popups;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class EditPage : Page
    {

        Dictionary<string,TranslationUnitEdit> list = new Dictionary<string, TranslationUnitEdit>();
        StorageFolder current;
        public EditPage()
        {
            this.InitializeComponent();
            this.EditCodePallet.removeTab += (string path, int idx) =>
            {
                this.list.Remove(path);
                this.UsedFiles.Items.RemoveAt(idx);
            };
            this.UsedFiles.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                this.EditCodePallet.SelectTab((sender as ListView).SelectedIndex);
            };
        }

        private void LightModeRequest_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Light;
        }

        private void DarkModeRequest_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
        }

        private void FullScreenRequest_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }

        private async void NewProjectRequest_Click(object sender, RoutedEventArgs e)
        {
            App1.Prompts.NewProjectPrompt prompt = new App1.Prompts.NewProjectPrompt();
            await prompt.ShowAsync();
            ImportFolderProject(prompt.Folder);
        }

        private void Saverequested_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitRequested_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void NewFItemRequest_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".aysl");
            var f = await picker.PickSingleFileAsync();
            if (f is null) return;
            ImportFileCode(f);
        }

        private async void NewPItemRequest_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".aysl");
            picker.FileTypeFilter.Add(".txt");
            var f = await picker.PickSingleFolderAsync();
            ImportFolderProject(f);
        }

        private async void ImportFileCode(StorageFile file)
        {
            if (!list.ContainsKey(file.Path))
            {
                this.UsedFiles.Items.Add(new TextBlock { Text = "🗒️ : " + file.Name });
                var translation = new TranslationUnitEdit(file);
                list.Add(file.Path, translation);
                TabViewItem tab = new TabViewItem();
                tab.Header = file.Name;
                tab.Content = translation;
                this.EditCodePallet.AddTab(tab);
            }
        }

        private async void ImportFolderProject(StorageFolder f)
        {
            ClearTreeNodes();
            if (f is null)
                return;
            current = f;
            TreeViewNode root = new TreeViewNode();
            this.ProjectTree.RootNodes.Add(await InitFolderTreesync(f));
        }

        void ClearTreeNodes()
        {
            this.ProjectTree.RootNodes.Clear();
        }

        private async Task<TreeViewNode> InitFolderTreesync(StorageFolder f)
        {
            var itemsList =  await f.GetItemsAsync();
            TreeViewNode node = new TreeViewNode();
            node.Content = "📁 : " + f.Name;
            foreach (var item in itemsList)
            {
                if (item is StorageFolder)
                {
                    try
                    {
                        var SubFolder = await StorageFolder.GetFolderFromPathAsync(item.Path);
                        node.Children.Add(await InitFolderTreesync(SubFolder));

                    }
                    catch (Exception _e)
                    {
                        MessageDialog requestPermissionDialog =
                                  new MessageDialog($"The app needs to access the {f.Path}. " +
                                  "Press OK to open system settings and give this app permission. " +
                                  "If the app closes, reopen it afterwards. " +
                                  "If you Cancel, the app will have limited functionality only.");
                        var okCommand = new UICommand("OK");
                        requestPermissionDialog.Commands.Add(okCommand);
                        var cancelCommand = new UICommand("Cancel");
                        requestPermissionDialog.Commands.Add(cancelCommand);
                        requestPermissionDialog.DefaultCommandIndex = 0;
                        requestPermissionDialog.CancelCommandIndex = 1;
                        var requestPermissionResult = await requestPermissionDialog.ShowAsync();
                        if (requestPermissionResult == okCommand)
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
                            var confirmationDialog = new MessageDialog($"Please give this app the {_e.Message} permission " +
                                                                        "in the Settings app which has now opened.");
                            confirmationDialog.Commands.Add(okCommand);
                            await confirmationDialog.ShowAsync();
                        }
                    }
                }
                else
                {
                    if (item.Name.EndsWith(".aysl"))
                    {
                        TreeViewNode fnode = new TreeViewNode();
                        fnode.Content =  "📄 : " + item.Name;
                        node.Children.Add(fnode);
                    }
                }
            }
            return node;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ProjectTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            TreeViewNode InvokedItem = args.InvokedItem as TreeViewNode;
            try
            {
                Func<TreeViewNode, string, string> getPath = null;
                getPath=(TreeViewNode t,string accumulated) =>
                 {
                     if (t.Depth == 0)
                         return accumulated;
                     else {
                        string folderName = (t.Content as string).Remove(0, "📁 : ".Length);
                         return getPath(t.Parent, folderName + "\\" + accumulated);
                     }

                 };
                string fileName = (InvokedItem.Content as string).Remove(0, "📄 : ".Length);
                var Path = current.Path + "\\" +  getPath(InvokedItem.Parent, fileName);
                var file = await StorageFile.GetFileFromPathAsync(Path);
                ImportFileCode(file);
            }
            catch (Exception)
            {
                InvokedItem.IsExpanded = !InvokedItem.IsExpanded;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (EditCodePallet.CurrentTranslationUnit != -1)
            {
                EditCodePallet.Run();
            }
            else
            {
                EditCodePallet.InnerConsole.addText("Please Open a AYSL File" + Environment.NewLine);
            }
        }
    }
}
