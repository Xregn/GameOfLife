using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace GameOfLife
{
    class OpenViewMode
    {
        Window Box = new Window();//window for the inputbox 
        DataGrid DataGridView = new DataGrid();
        Button ButCancel = new Button();
        Button ButFunction = new Button();
        Canvas CanvasContentPanel = new Canvas();// items container
        DataTable DataTableStorage = new DataTable();
         

        Brush BoxBackgroundColor = Brushes.WhiteSmoke;// Window Background 

        string title = "Просмотр списка";//title as heading 
        string errormessage = "Данные не выбраны. Выберите данные";//error messagebox content 
        string errortitle = "Ошибка";//error messagebox heading title 
        string ButCancelText = "Отмена";//текст кнопки отмена 
        string ButFunctionText = "FunctionText";//текст функциональной кнопки 
        int SelectedId = -1;//id выленной строки. значение минус единица (-1), означает пустое выеление

        public OpenViewMode(DataTable dataTable, string FunctionText)
        {
            ButFunctionText = FunctionText;
            this.DataTableStorage = dataTable;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            #region Initialize Box
            Box.Height = 355;
            Box.Width = 300;
            Box.Background = BoxBackgroundColor;
            Box.Title = title;
            Box.Content = CanvasContentPanel;
            #endregion

            #region Initialize DataGridView
            DataGridView.Height = 278;
            DataGridView.Width = 270;
            DataGridView.VerticalAlignment = VerticalAlignment.Top;
            DataGridView.Margin = new Thickness(10,10,0,0);
            DataGridView.IsReadOnly = true;
            DataGridView.ItemsSource = DataTableStorage.AsDataView();
            DataGridView.SelectedItem = DataGridView.Items[0];
            DataGridView.ScrollIntoView(DataGridView.Items[0]);
            CanvasContentPanel.Children.Add(DataGridView);
            #endregion

            #region Initialize ButCancel
            ButFunction.Height = 20;
            ButFunction.Width = 75;
            ButFunction.Margin = new Thickness(10, 293, 0, 0);
            ButFunction.Content = ButFunctionText;
            ButFunction.Click += function_Click;
            CanvasContentPanel.Children.Add(ButFunction);
            #endregion

            #region Initialize ButOk
            ButCancel.Height = 20;
            ButCancel.Width = 75;
            ButCancel.Margin = new Thickness(90, 293, 0, 0);
            ButCancel.Content = ButCancelText;
            ButCancel.Click += cancel_Click;
            CanvasContentPanel.Children.Add(ButCancel);
            #endregion
        }

        void function_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow = (DataRowView)DataGridView.SelectedItem;

            if (dataRow["id"] != null)
            {
                SelectedId = Convert.ToInt16( dataRow["id"]);
                Box.Close();
            }
            else
            {
                MessageBox.Show(errormessage, errortitle);
            }
        }
        void cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedId = -1;
            Box.Close();
        }
        public int ShowDialog()
        {
            Box.ShowDialog();
            return SelectedId;
        }

    }
}
