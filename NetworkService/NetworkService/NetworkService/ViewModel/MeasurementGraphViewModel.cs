﻿using NetworkService.Model;
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
        #region Initialize
        int keyCount = -1;
        private List<string> comboBoxItems;
        private string selectedEntity;
        private string selectedEntityToShow;
        public Dictionary<string, List<Measurement>> MeasurementDict { get; set; }
        public ObservableCollection<CircleMarker> CircleMarkers { get; set; }
        public BindingList<Entity> EntitiesInList { get; set; }

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
        public string SelectedEntityToShow
        {
            get { return selectedEntityToShow; }
            set
            {
                selectedEntityToShow = value;

                OnPropertyChanged("SelectedEntityToShow");
            }
        }

        public MeasurementGraphViewModel()
        {
            EntitiesInList = new BindingList<Entity>();
            
            EntitiesInList.ListChanged += OnEntitiesInListChanged;

            MeasurementDict = new Dictionary<string, List<Measurement>>();
            CircleMarkers = new ObservableCollection<CircleMarker>();
            for (int i = 0; i <5; i++)
            {
                CircleMarker marker = new CircleMarker();
                CircleMarkers.Add(marker);
            }

            UpdateComboBoxItems();

            ShowCommand = new ClassICommand(OnShow, CanShow);
        }
        #endregion

        #region ShowBTN
        public void OnShow()
        {
            SelectedEntityToShow = SelectedEntity;
            keyCount = 0;
            foreach (var entity in EntitiesInList)
            {
                if (entity.Name == SelectedEntityToShow)
                {
                    break;
                }
                keyCount++;
            }
            foreach (CircleMarker marker in CircleMarkers)
            {
                marker.CmTime = "";
                marker.CmValue = 1;
                marker.CmDate = "";
            }
            UpdateValue();           
        }

        private bool CanShow()
        {
            return SelectedEntity != null;
        }
        #endregion

        #region AutoUpdate
        public void UpdateValue()
        {
            
            foreach (var item in MeasurementDict.Keys)
            {                
                string customKey = $"Entity_{keyCount}";

                if (item == customKey)
                {                   
                    List<Measurement> list = new List<Measurement>();
                    list = MeasurementDict[item];
                    int cnt = 0;
                    foreach (Measurement measurement in list)
                    {
                        CircleMarker cm = new CircleMarker(measurement.Value, measurement.Date, measurement.Time);

                        CircleMarkers[cnt++] = cm;
                        if (cnt == 5)
                        {
                            cnt = 0;
                        }
                    }
                }
            }
        }

        public void AutoShow()
        {
            string filePath = "Log.txt";
            string poslednjaLinija = ReadLastLine(filePath);
            if (poslednjaLinija != null)
            {
                (string date, string time, string entity, int value) = ParseLine(poslednjaLinija);
                string[] entityParts = entity.Split('_');
                int entityNumber = int.Parse(entityParts[1]);
               
                Measurement measurement = new Measurement(date, time, value);
                string key = $"Entity_{entityNumber}";

                if (MeasurementDict.ContainsKey(key))
                {
                    MeasurementDict[key].Add(measurement);
                    if (MeasurementDict[key].Count > 5)
                    {
                        MeasurementDict[key].RemoveAt(0);
                    }
                }
                else
                {
                    MeasurementDict[key] = new List<Measurement> { measurement };
                }

                UpdateValue();
            }

        }
        #endregion

        #region LogParser
        static string ReadLastLine(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                string lastLine = null;

                while ((line = reader.ReadLine()) != null)
                {
                    lastLine = line;
                }

                return lastLine;
            }
        }
        static (string date, string time, string entity, int value) ParseLine(string line)
        {
            // Podelite liniju koristeći tačku-zarez (`;`) kao separator
            string[] parts = line.Split(';');

            // Prvi deo (datum i vreme) je prvi element niza `delovi`
            string dateTimePart = parts[0].Trim();

            // Podelite datum i vreme koristeći razmak (` `) kao separator
            string[] dateTimePart_s = dateTimePart.Split(' ');
            string date = dateTimePart_s[0];
            string time = dateTimePart_s[1];

            // Drugi deo (entitet i vrednost) je drugi element niza `delovi`
            string leftover = parts[1].Trim();

            // Podelite ostatak koristeći zarez (`,`) kao separator
            string[] leftoverParts = leftover.Split(',');

            // Entitet je prvi deo ostatka
            string entity = leftoverParts[0].Trim();

            // Vrednost je drugi deo ostatka
            int value = int.Parse(leftoverParts[1].Trim());

            // Vraća tuple sa razdvojenim datumom, vremenom, entitetom i vrednošću
            return (date, time, entity, value);
        }
        #endregion       

        #region ComboBoxItems
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
        #endregion
    }
}
