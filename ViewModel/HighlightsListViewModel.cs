using GalaSoft.MvvmLight.Command;
using MFormat.Model;
using MFormat.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Action = MFormat.Model.Action;

namespace MFormat.ViewModel
{
    //ViewModel for HighlightsList
    public class HighlightsListViewModel : INotifyPropertyChanged
    {
        private bool enableGuiElements = false;
        public bool EnableGuiElements 
        { 
            get { return enableGuiElements; } 
            set 
            {
                this.enableGuiElements = value;
                OnPropertyChanged("EnableGuiElements");
            } 
        }
        private bool previewFlag = false;
        //Reference to view
        public HiglightsList view;
        //Reference to main broadcasting player
        public BroadcastManager broadcastPlayer;
        //Player for editing
        private BroadcastManager editPlayer;
        //PreviewSource for editPlayer
        private D3DImage previewSource;
        public D3DImage PreviewSource { get { return previewSource; } set { previewSource = value; } }
        //Reference for list of actions
        public ObservableCollection<Action> ActionsList { get; set; }
        //Reference to selectedAction
        private Action selectedAction;
        public Action SelectedAction
        {
            get { return selectedAction; }
            set
            {
                this.selectedAction = value;
                HighlightSelected();
                OnPropertyChanged("SelectedAction");
            }
        }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand<Action> DeleteHighlightCommand { get; set; }
        public RelayCommand<Action> BroadcastHighlightCommand { get; set; }
        public RelayCommand PlayHighlightPreviewCommand { get; set; }
        public RelayCommand StopHiglightPreviewCommand { get; set; }
        public RelayCommand BeforeChangedCommand { get; set; }
        public RelayCommand AfterChangedCommand { get; set; }
        public RelayCommand<MouseWheelEventArgs> BeforeChangedByWheelCommand { get; set; }
        public RelayCommand<MouseWheelEventArgs> AfterChangedByWheelCommand { get; set; }
        public RelayCommand NameChangedCommand { get; set; }

        public HighlightsListViewModel()
        {
            ViewLoadedCommand = new RelayCommand(() => PageLoaded());
            DeleteHighlightCommand = new RelayCommand<Action>((action) => DeleteHighlight(action));
            BroadcastHighlightCommand = new RelayCommand<Action>((action) => BroadcastHighlight(action));
            //SelectedHighlightCommand = new RelayCommand(() => SelectedHighlight());
            PlayHighlightPreviewCommand = new RelayCommand(() => PlayHighlightPreview());
            StopHiglightPreviewCommand = new RelayCommand(() => StopHighlightPreview());
            BeforeChangedCommand = new RelayCommand(() => BeforeChanged());
            AfterChangedCommand = new RelayCommand(() => AfterChanged());
            NameChangedCommand = new RelayCommand(() => NameChanged());
            
            Actions.Instance.PropertyChanged += ActionAddedEventHandler;
            this.ActionsList = Actions.Instance.GetActions();
        }
        //After View was loaded initialize ListBox with actions and create instance for EditVideo player
        private void PageLoaded()
        {
            this.editPlayer = new BroadcastManager(out this.previewSource, 0);
            this.editPlayer.PropertyChanged += EditPlayerFrameRendered;
        }


        //When Highlight was selected, load that highlight into editPlayer and set start time and end time of that highlight into number pickers
        private void HighlightSelected()
        {
            if (this.SelectedAction != null)
            {
                previewFlag = false;
                this.editPlayer.ChangeMedia(this.selectedAction.filename);
                this.EnableGuiElements = true;
                previewFlag = true;
            }
            else
            {
                this.EnableGuiElements = false;
            }
        }

        //Plays preview of the highlight
        private void PlayHighlightPreview()
        {
            if (this.selectedAction != null)
            {
                this.editPlayer.StartMediaSourcePlayback(this.selectedAction.startTime, this.selectedAction.endTime);
            }
        }
        //Stops preview of the highlight
        private void StopHighlightPreview()
        {
            this.editPlayer.StopPlayback();
        }

        private void DeleteHighlight(Action action)
        {
            Actions.Instance.DeleteAction(action);
        }
        //After doubleclick send the highlight into broadcasting
        private void BroadcastHighlight(Action action)
        {
            this.broadcastPlayer.ChangeMedia(action.filename);
            this.broadcastPlayer.StartMediaSourcePlayback(action.startTime, action.endTime);
            this.view.highlightsList.UnselectAll();
        }
        //When StartTime of highlight was changed, play the preview again
        private void BeforeChanged()
        {
            if (SelectedAction != null)
            {
                PlayHighlightPreview();
            }
        }
        //When EndTime of highlight was changed, play the preview again
        private void AfterChanged()
        {
            if (SelectedAction != null)
            {
                PlayHighlightPreview();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; 

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //When action was added into list, re-initialize the list
        private void ActionAddedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ActionsList");
        }


        private void EditPlayerFrameRendered(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("PreviewSource");
        }

        //Event handling name change
        private void NameChanged()
        {
            if (SelectedAction != null) {
                OnPropertyChanged("ActionsList");
            }
        }

    }
}
