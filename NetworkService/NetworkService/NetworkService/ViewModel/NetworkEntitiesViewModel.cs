using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : ClassINotifyPropertyChanged
    {
        private int _id = 0;
        private string errorMSg = "";
        public List<string> ComboBoxItems { get; set; } = new List<string>()
        {
            "RTD",
            "TermoSprega",
        };
    public ObservableCollection<Entity> EntitiesToShow { get; set; }
        public ObservableCollection<Entity> Entities { get; set; }
        public ObservableCollection<Entity> EntitiesSearched { get; set; }
        public ClassICommand AddEntityCommand { get; set; }
        public ClassICommand DeleteEntityCommand { get; set; }
        public ClassICommand SearchEntityCommand { get; set; }


        // Unos novog entiteta
        private Entity currentEntity = new Entity();
        private EntityType currentEntityType = new EntityType();
        // Entitet selektovan u datagridu
        private Entity selectedEntity;
        //Search
        private string searchBox;
        private bool isTypeRBSelected;
        private bool isNameRBSelected = true;
        private List<string> searchedHistory = new List<string>();

        public NetworkEntitiesViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            EntitiesSearched = new ObservableCollection<Entity>();
            EntitiesToShow = Entities;

            AddEntityCommand = new ClassICommand(OnAdd);
            DeleteEntityCommand = new ClassICommand(OnDelete, CanDelete);
            SearchEntityCommand = new ClassICommand(onSearch);
        }
        #region SearchBTN

        public List<string> SearchedHistory
        {
            get { return searchedHistory; }
            set
            {
                searchedHistory = value;
                OnPropertyChanged("SearchedHistory");
            }
        }

        public string SearchBox
        {
            get { return searchBox; }
            set
            {
                searchBox = value;
                OnPropertyChanged("SearchBox");
            }
        }

        public bool IsTypeRBSelected
        {
            get { return isTypeRBSelected; }
            set
            {
                isTypeRBSelected = value;
                OnPropertyChanged("IsTypeRBSelected");
            }
        }

        public bool IsNameRBSelected
        {
            get { return isNameRBSelected; }
            set
            {
                isNameRBSelected = value;
                OnPropertyChanged("IsNameRBSelected");
            }
        }

        private void onSearch()
        {
            EntitiesSearched.Clear();

            if (IsTypeRBSelected)
            {
                if(SearchBox.Trim() == "")
                {
                    EntitiesToShow = Entities;
                    OnPropertyChanged("EntitiesToShow");
                }
                else
                {
                    foreach (var entity in Entities)
                    {
                        if (entity.Type.Type.Contains(SearchBox))
                        {
                            EntitiesSearched.Add(entity);
                        }
                    }
                    EntitiesToShow = EntitiesSearched;
                    OnPropertyChanged("EntitiesToShow");
                    if (!SearchedHistory.Contains(SearchBox))
                    {
                        SearchedHistory.Add(SearchBox);
                        OnPropertyChanged("SearchedHistory");
                    }
                }      
            }
            else
            {
                if (SearchBox.Trim() == "")
                {
                    EntitiesToShow = Entities;
                    OnPropertyChanged("EntitiesToShow");
                }
                else
                {
                    foreach (var entity in Entities)
                    {
                        if (entity.Name.Contains(SearchBox))
                        {
                            EntitiesSearched.Add(entity);
                        }
                    }
                    EntitiesToShow = EntitiesSearched;
                    OnPropertyChanged("EntitiesToShow");
                    if (!SearchedHistory.Contains(SearchBox))
                    {
                        SearchedHistory.Add(SearchBox);
                        OnPropertyChanged("SearchedHistory");
                    }
                }
            }
        }


        #endregion

        #region DeletEntityBTN
        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                DeleteEntityCommand.RaiseCanExecuteChanged();
            }
        }
        private void OnDelete()
        {
            Entities.Remove(SelectedEntity);
            if (EntitiesSearched.Contains(SelectedEntity))
            {
                EntitiesSearched.Remove(SelectedEntity);
            }
        }

        private bool CanDelete()
        {
            return SelectedEntity != null;
        }
        #endregion

        #region AddEntityBTN
        public string ErrorMSg
        {
            get { return errorMSg; }
            set
            {
                errorMSg = value;
                OnPropertyChanged("ErrorMSg");
            }
        }

        public Entity CurrentEntity
        {
            get { return currentEntity; }
            set
            {
                currentEntity = value;
                OnPropertyChanged("CurrentEntity");
            }
        }

        public EntityType CurrentEntityType
        {
            get { return currentEntityType; }
            set
            {
                currentEntityType = value;
                OnPropertyChanged("CurrentEntityType");
            }
        }

        public void OnAdd()
        {
            string imgPath = "";
            if(CurrentEntityType.Type == null)
            {
                ErrorMSg = "Need To Choose Type!";
                return;
            }
            else if(CurrentEntityType.Type == "RTD")
            {
                imgPath = "pack://application:,,,/NetworkService;component/Images/Screenshot_4.png";
            }
            else
            {
                imgPath = "pack://application:,,,/NetworkService;component/Images/Screenshot_6.png";
            }
            ErrorMSg = "";

            try
            {
                Entities.Add(new Entity() 
                            { Id = _id,
                              Name = $"Entity_{_id++}",
                              Value = 0,
                              Type = new EntityType() { Type = CurrentEntityType.Type, ImgSrc = imgPath }
                            });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - {ex.Message}");
            }
        }
        #endregion
    }
}
