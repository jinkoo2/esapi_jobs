using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;

namespace esapi.UI
{
    public partial class DataObjectListComboBox : UserControl
    {
        private IEnumerable<ApiDataObject> _list;

        public DataObjectListComboBox()
        {
            InitializeComponent();
            ComboBox1.SelectionChanged += CourseComboBox_SelectionChanged;
        }

        public void SetList(IEnumerable<ApiDataObject> list)
        {
            _list = list;
            ComboBox1.ItemsSource = _list;
        }

        public ApiDataObject GetSelectedItem()
        {
            return ComboBox1.SelectedItem as ApiDataObject;
        }

        public string GetSelectedItemId()
        {
            return ComboBox1.SelectedValue as string;
        }

        // Event declaration
        public event EventHandler<ApiDataObject> SelectedItemChanged;

        private void CourseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = GetSelectedItem();
            SelectedItemChanged?.Invoke(this, selectedItem);
        }
    }




}
