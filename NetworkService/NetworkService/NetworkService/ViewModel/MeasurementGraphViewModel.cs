using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : ClassINotifyPropertyChanged
    {
        private List<string> comboBoxItems;
        private string selectedEntity;

        public ClassICommand ShowCommand { get; set; }
        public List<string> ComboBoxItems
        {
            get { return comboBoxItems; }
            set
            {
                comboBoxItems = value;
                OnPropertyChanged("ComboBoxItems");
            }
        }
        public string SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;

                if (SelectedEntity != null && !ComboBoxItems.Contains(SelectedEntity))
                {
                    SelectedEntity = null;  
                }
                ShowCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedEntity");
            }
        }
        public BindingList<Entity> EntitiesInList { get; set; }

        public MeasurementGraphViewModel()
        {
            EntitiesInList = new BindingList<Entity>();
            
            EntitiesInList.ListChanged += OnEntitiesInListChanged;

            
            
            UpdateComboBoxItems();

            ShowCommand = new ClassICommand(OnShow, CanShow);
        }

        private void OnShow()
        {
            
        }
        private bool CanShow()
        {
            return SelectedEntity != null;
        }

        private void OnEntitiesInListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateComboBoxItems();

            if (SelectedEntity != null && !ComboBoxItems.Contains(SelectedEntity))
            {
                SelectedEntity = null;
            }

            
        }

        private void UpdateComboBoxItems()
        {
            ComboBoxItems = EntitiesInList
                .Select(entity => entity.Name)
                .ToList();
            OnPropertyChanged(nameof(ComboBoxItems));
        }

    }
}
