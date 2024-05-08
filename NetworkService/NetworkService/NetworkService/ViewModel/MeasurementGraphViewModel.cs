using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        private string selectedEntityToShow;
        public Dictionary<string, List<Measurement>> MesuresDict { get; set; }
        public ObservableCollection<CircleMarker> CircleMarkers { get; set; }

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

            MesuresDict = new Dictionary<string, List<Measurement>>();
            CircleMarkers = new ObservableCollection<CircleMarker>();
            for (int i = 0; i <5; i++)
            {
                CircleMarker marker = new CircleMarker();
                CircleMarkers.Add(marker);
            }

            UpdateComboBoxItems();

            ShowCommand = new ClassICommand(OnShow, CanShow);
        }

        public string SelectedEntityToShow
        {
            get { return selectedEntityToShow; }
            set
            {
                selectedEntityToShow = value;

                OnPropertyChanged("SelectedEntityToShow");
            }
        }

        public void OnShow()
        {
            SelectedEntityToShow = SelectedEntity;
            UpdateValue();
        }

        public void UpdateValue()
        {
            foreach (var item in MesuresDict.Keys)
            {
                if (item == SelectedEntityToShow)
                {
                    List<Measurement> list = new List<Measurement>();
                    list = MesuresDict[item];
                    int br = 0;
                    foreach (Measurement measurement in list)
                    {
                        CircleMarker cm = new CircleMarker(measurement.Value, measurement.Time, "bbb");

                        CircleMarkers[br++] = cm;
                        if (br == 5)
                        {
                            br = 0;
                        }
                    }
                }
            }
        }

        public void AutoShow()
        {
            string filePath = "Log.txt";
            string poslednjaLinija = ProcitajPoslednjuLiniju(filePath);
            if (poslednjaLinija != null)
            {
                // Parsirajte poslednju liniju
                (string datum, string entitet, int vrednost) = ParsirajLiniju(poslednjaLinija);
                string[] delovi = entitet.Split('_');
                int brojEntiteta = int.Parse(delovi[1]);
                // Ispišite rezultate
                //MessageBox.Show($"{datum} {entitet} {vrednost} {brojEntiteta}");
                bool validno = false;
                if (vrednost > 250 || vrednost < 350)
                {
                    validno = true;
                }

                Measurement measurement = new Measurement(datum, vrednost, validno);
                string key = $"Entity_{brojEntiteta}";

                if (MesuresDict.ContainsKey(key))
                {
                    MesuresDict[key].Add(measurement);
                    if (MesuresDict[key].Count > 5)
                    {
                        MesuresDict[key].RemoveAt(0);
                    }
                }
                else
                {
                    MesuresDict[key] = new List<Measurement> { measurement };
                }

                UpdateValue();
            }

        }

        static string ProcitajPoslednjuLiniju(string filePath)
        {
            // Koristite StreamReader za čitanje fajla
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Kreirajte promenljivu za čitanje linije
                string linija;
                string poslednjaLinija = null;

                // Čitajte fajl dok ne dođete do kraja
                while ((linija = reader.ReadLine()) != null)
                {
                    // Zadržite poslednju liniju
                    poslednjaLinija = linija;
                }

                return poslednjaLinija;
            }
        }
        static (string, string, int) ParsirajLiniju(string linija)
        {
            string[] delovi = linija.Split(new[] { ':' }, 2);

            // Prvi deo je datum i vreme (kao deo `delovi[0]`)
            string datumVremeDeo = delovi[0];

            // Podelite datum i vreme koristeći razmak kao separator
            string[] datumVremeDelovi = datumVremeDeo.Split(' ');
            string datum = datumVremeDelovi[0];
            string vreme = datumVremeDelovi[1];

            // Kombinujte datum i vreme u željeni format
            string datumVreme = $"{datum} {vreme}";

            // Drugi deo je ostatak linije, razdvajamo ga po zarezu (,)
            string[] ostatakDelovi = delovi[1].Split(',');

            // Entitet je prvi deo ostatka
            string entitet = ostatakDelovi[0].Trim();

            // Vrednost je drugi deo ostatka
            int vrednost = int.Parse(ostatakDelovi[1].Trim());

            return (datumVreme, entitet, vrednost);
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
