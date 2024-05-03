using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : ClassINotifyPropertyChanged
    {
        public BindingList<Entity> EntitiesInList { get; set; }
        public ObservableCollection<Brush> BorderBrushCollection { get; set; }
        public ObservableCollection<Canvas> CanvasCollection { get; set; }

        private Entity selectedEntity;

        private Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        public ClassICommand<object> DropEntityOnCanvas { get; set; }
        public ClassICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public ClassICommand MouseLeftButtonUp { get; set; }
        public ClassICommand<object> SelectionChanged { get; set; }
        public ClassICommand<object> FreeCanvas { get; set; }
        public ClassICommand<object> RightMouseButtonDownOnCanvas { get; set; }
        public ClassICommand OrganizeAllCommand { get; set; }

        public NetworkDisplayViewModel()
        {
            EntitiesInList = new BindingList<Entity>();

            BorderBrushCollection = new ObservableCollection<Brush>();
            for (int i = 0; i < 12; i++)
            {
                BorderBrushCollection.Add(Brushes.DarkGray);
            }

            CanvasCollection = new ObservableCollection<Canvas>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas()
                {
                    Background = Brushes.LightGray,
                    AllowDrop = true
                });
            }

            DropEntityOnCanvas = new ClassICommand<object>(OnDrop);
            LeftMouseButtonDownOnCanvas = new ClassICommand<object>(OnLeftMouseButtonDown);
            MouseLeftButtonUp = new ClassICommand(OnMouseLeftButtonUp);
            SelectionChanged = new ClassICommand<object>(OnSelectionChanged);
            FreeCanvas = new ClassICommand<object>(OnFreeCanvas);
            RightMouseButtonDownOnCanvas = new ClassICommand<object>(OnRightMouseButtonDown);
            OrganizeAllCommand = new ClassICommand(onOrganize);

        }

        private void onOrganize()
        {
            List<Entity> addedEntities = new List<Entity>();
            try
            {
                int index = 0;
                foreach (var item in EntitiesInList)
                {
                    while (index < CanvasCollection.Count)
                    {
                        if (CanvasCollection[index].Resources != null && CanvasCollection[index].Resources["taken"] == null)
                        {
                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri(item.Type.ImgSrc, UriKind.RelativeOrAbsolute);
                            logo.EndInit();

                            CanvasCollection[index].Background = new ImageBrush(logo);
                            CanvasCollection[index].Resources.Add("taken", true);
                            CanvasCollection[index].Resources.Add("data", item);
                            BorderBrushCollection[index] = (item.IsValueValidForType()) ? Brushes.Green : Brushes.Red;

                            addedEntities.Add(item);

                            break;
                        }
                        index++;
                    }
                    index = 0; 
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            foreach (var entity in addedEntities)
            {
                EntitiesInList.Remove(entity);
            }

        }
        private void OnDrop(object parameter)
        {
            if (draggedItem != null)
            {
                int index =  Convert.ToInt32(parameter);

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri(draggedItem.Type.ImgSrc, UriKind.RelativeOrAbsolute);
                    logo.EndInit();

                    CanvasCollection[index].Background = new ImageBrush(logo);
                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", draggedItem);
                    BorderBrushCollection[index] = (draggedItem.IsValueValidForType()) ? Brushes.Green : Brushes.Red;

                    // PREVLACENJE IZ DRUGOG CANVASA
                    if (draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = Brushes.LightGray;
                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");
                        BorderBrushCollection[draggingSourceIndex] = Brushes.DarkGray;

                        //UpdateLinesForCanvas(draggingSourceIndex, index);

                        // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet pomeren na drugo polje
                        /*if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            currentLine = new MyLine();
                        }*/

                        draggingSourceIndex = -1;
                    }

                    // PREVLACENJE IZ LISTE
                    if (EntitiesInList.Contains(draggedItem))
                    {
                        EntitiesInList.Remove(draggedItem);
                    }
                }
            }
        }
        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                Entity entity = (CanvasCollection[i].Resources["data"]) as Entity;

                if ((entity != null) && (entity.Id == entityId))
                {
                    return i;
                }
            }
            return -1;
        }
        public void UpdateEntityOnCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                if (entity.IsValueValidForType())
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Green;
                }
                else
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Red;
                }
            }
        }
        public void DeleteEntityFromCanvas(Entity entity)
        {
            int canvasIndex = GetCanvasIndexForEntityId(entity.Id);

            if (canvasIndex != -1)
            {
                CanvasCollection[canvasIndex].Background = Brushes.LightGray;
                CanvasCollection[canvasIndex].Resources.Remove("taken");
                CanvasCollection[canvasIndex].Resources.Remove("data");
                BorderBrushCollection[canvasIndex] = Brushes.DarkGray;

                //DeleteLinesForCanvas(canvasIndex);
            }
        }
        private void OnLeftMouseButtonDown(object parameter)
        {
            if (!dragging)
            {
                int index = Convert.ToInt32(parameter);

                if (CanvasCollection[index].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (Entity)(CanvasCollection[index].Resources["data"]);
                    draggingSourceIndex = index;
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
                }
            }
        }
        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }
        private void OnSelectionChanged(object parameter)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = SelectedEntity;
                DragDrop.DoDragDrop((ListView)parameter, draggedItem, DragDropEffects.Move);
            }
        }
        private void OnFreeCanvas(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                // Crtanje linije se prekida ako je, izmedju postavljanja tacaka, entitet uklonjen sa canvas-a
                /*if (sourceCanvasIndex != -1)
                {
                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    currentLine = new MyLine();
                }

                DeleteLinesForCanvas(index);*/

                EntitiesInList.Add((Entity)CanvasCollection[index].Resources["data"]);
                CanvasCollection[index].Background = Brushes.LightGray;
                CanvasCollection[index].Resources.Remove("taken");
                CanvasCollection[index].Resources.Remove("data");
                BorderBrushCollection[index] = Brushes.DarkGray;
            }
        }
        private void OnRightMouseButtonDown(object parameter)
        {

        }

        public Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged("SelectedEntity");
            }
        }
    }
}
