using Gat.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WindowsPathEditor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class MainViewModel
    {
        public MainViewModel()
        {
            LoadItems();

            SaveCommand = new RelayCommand(Save);
            DiscardCommand = new RelayCommand(Discard);

            PickPathCommand = new RelayCommand<PathItem>(PickPath);
            DeleteCommand = new RelayCommand<PathItem>(Delete);
        }

        public ObservableCollection<PathItem> Items { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand DiscardCommand { get; set; }

        public ICommand PickPathCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private void LoadItems()
        {
            Items = new ObservableCollection<PathItem>(
                Environment.GetEnvironmentVariable("PATH")
                    .Split(';')
                    .Distinct()
                    .OrderBy(o => o)
                    .Select(o => new PathItem(o))
            );
        }

        private void Save(object obj)
        {
            try
            {
                File.WriteAllText("path.bak", Environment.GetEnvironmentVariable("PATH"));
            }
            catch
            {
                MessageBox.Show("Could not create path.bak file. Aborting!", "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Environment.SetEnvironmentVariable("PATH",
                    String.Join(";", Items.Where(o => !String.IsNullOrWhiteSpace(o.Path)).Select(o => o.Path).Distinct().OrderBy(o => o)),
                    EnvironmentVariableTarget.Machine);
            }
            catch
            {
                MessageBox.Show("Could not change PATH! Try running as admin.", "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Your path has been updated.", "Saved successfully!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Discard(object obj)
        {
            if (MessageBox.Show("Are you sure?", "Confirm Discard", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                LoadItems();
        }

        private void PickPath(PathItem obj)
        {
            if (obj == null)
                return;

            var dialog = new OpenDialogView();
            var vm = (OpenDialogViewModel)dialog.DataContext;

            vm.IsDirectoryChooser = true;
            if (vm.Show() == true)
            {
                obj.Path = vm.SelectedFilePath;
            }
        }

        private void Delete(PathItem obj)
        {
            if (MessageBox.Show("Are you sure?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Items.Remove(obj);
        }
    }

    [PropertyChanged.ImplementPropertyChanged]
    public class PathItem : PropertyValidateModel, IEditableObject
    {
        private string currentPath;

        public PathItem()
            : this(null)
        {
        }

        public PathItem(string path)
        {
            Path = path;
        }

        [Required]
        [FolderPathExists]
        public string Path { get; set; }

        public void BeginEdit()
        {
            currentPath = Path;
        }

        public void CancelEdit()
        {
            Path = currentPath;
        }

        public void EndEdit()
        {
            
        }
    }
}
