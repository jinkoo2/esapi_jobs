using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VMS.TPS.Common.Model.API;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PluginScriptExample
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        private ViewModel _viewModel;

        //Public properties used to receive data from main program
        private StructureSet _structureSet;
        public StructureSet StructureSet
        {
            get { return _structureSet; }

            set
            {
                _structureSet = value;
            }
        }

        private PlanningItem _pItem;
        public PlanningItem pItem
        {
            get { return _pItem; }
            set
            {
                _pItem = value;
                lblPlan.Content = _pItem.Id;
            }
        }

        private List<PlanningItem> _pItemsInScope;
        public List<PlanningItem> PItemsInScope
        {
            get { return _pItemsInScope; }
            set
            {
                _pItemsInScope = value;
                foreach (var pitem in _pItemsInScope)
                    _viewModel.PlanningItemIds.Add(pitem.Id);
            }
        }

        private Patient _patient;
        public Patient patient
        {
            get { return _patient; }
            set
            {
                _patient = value;
                lblPatient.Content = _patient.Id;
            }
        }

        private User _user;
        public User user
        {
            get { return _user; }
            set
            {
                _user = value;
                lblUser.Content = _user.Id;
            }
        }

        public class ViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public ObservableCollection<string> _planningItemIds;
            public ObservableCollection<string> PlanningItemIds
            {
                get { return _planningItemIds; }
                set { _planningItemIds = value; }
            }

            public ViewModel()
            {
                _planningItemIds = new ObservableCollection<string>();
            }

            private void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public MainControl()
        {
            InitializeComponent();

            _viewModel = new ViewModel();
            this.DataContext = _viewModel;
        }
    }
}
