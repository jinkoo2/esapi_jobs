using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;

namespace esapi.UI
{
    public partial class CourseListComboBox : UserControl
    {
        private IEnumerable<Course> _courseList;

        public CourseListComboBox()
        {
            InitializeComponent();
            CourseComboBox.SelectionChanged += CourseComboBox_SelectionChanged;
        }

        public void SetCourses(IEnumerable<Course> courses)
        {
            _courseList = courses;
            CourseComboBox.ItemsSource = _courseList;
        }

        public Course GetSelectedCourse()
        {
            return CourseComboBox.SelectedItem as Course;
        }

        public string GetSelectedCourseId()
        {
            return CourseComboBox.SelectedValue as string;
        }

        // Event declaration
        public event EventHandler<Course> SelectedItemChanged;

        private void CourseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = GetSelectedCourse();
            SelectedItemChanged?.Invoke(this, selectedCourse);
        }
    }
}
