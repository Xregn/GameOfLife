using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameOfLife
{
    class InputBox
    {
        Window Box = new Window();//window for the inputbox 
        Button ButOk = new Button();
        Button ButCancel = new Button();
        TextBox TBoxContent = new TextBox();
        Label Label = new Label();
        Canvas CanvasContentPanel = new Canvas();// items container

        int FontSize = 14;//fontsize for the input
        Brush BoxBackgroundColor = Brushes.WhiteSmoke;// Window Background 
         
       
        string title = "InputBox";//title as heading 
        string LabelContent;//title 
        string TBoxContentDefaultText = "Введите...";//default textbox content 
        string errormessage = "Данные не ввеены. Ввеите данные";//error messagebox content 
        string errortitle = "Error";//error messagebox heading title 
        string ButOkText = "OK";//Ok button content 
        string ButCancelText = "Cancel";//Cancel button content 

        bool clicked = false;
        bool inputreset = false;
        public InputBox(string content)
        {
            try
            {
                LabelContent = content;
            }
            catch { LabelContent = "Error!"; }
            InitializeComponent();
        }
        public InputBox(string content, string Htitle, string DefaultText)
        {
            try
            {
                LabelContent = content;
            }
            catch { LabelContent = "Error!"; }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            try
            {
                TBoxContentDefaultText = DefaultText;
            }
            catch
            {
                DefaultText = "Error!";
            }
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            #region Initialize Box
            Box.Height = 125; 
            Box.Width = 289; 
            Box.Background = BoxBackgroundColor;
            Box.Title = title;
            Box.Content =  CanvasContentPanel;
            Box.Closing += Box_Closing;
            #endregion

            #region Initialize Box
            Label.Margin = new Thickness(12,4,0,0); 
            Label.Content = LabelContent;
            Label.FontSize = FontSize;
            CanvasContentPanel.Children.Add(Label);
            #endregion

            #region Initialize TBoxContent
            TBoxContent.Height = 20;
            TBoxContent.Width = 249;
            TBoxContent.Margin = new Thickness(12, 27, 0, 0);
            TBoxContent.FontSize = FontSize;
            TBoxContent.Text = TBoxContentDefaultText;
            TBoxContent.MouseEnter += input_MouseDown;
            CanvasContentPanel.Children.Add(TBoxContent);
            #endregion

            #region Initialize ButCancel
            ButCancel.Height = 23;
            ButCancel.Width = 75; 
            ButCancel.Margin = new Thickness(186, 53, 0, 0); 
            ButCancel.Content = ButCancelText;
            ButCancel.Click += cancel_Click;
            CanvasContentPanel.Children.Add(ButCancel);
            #endregion

            #region Initialize ButOk
            ButOk.Height = 23;
            ButOk.Width = 75;
            ButOk.Margin = new Thickness(105, 53, 0, 0); 
            ButOk.Content = ButOkText;
            ButOk.Click += ok_Click;
            CanvasContentPanel.Children.Add(ButOk);
            #endregion
        }

        void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!clicked)
                e.Cancel = true;
        }

        private void input_MouseDown(object sender, MouseEventArgs e)
        {
            if ((sender as TextBox).Text == TBoxContentDefaultText && inputreset == false)
            {
                (sender as TextBox).Text = null;
                inputreset = true;
            }
        }

        void cancel_Click(object sender, RoutedEventArgs e)
        {
            clicked = true;
            Box.Close();
            TBoxContent.Text = null;
            clicked = false;
        }

        void ok_Click(object sender, RoutedEventArgs e)
        {
            clicked = true;
            if (TBoxContent.Text == TBoxContentDefaultText || TBoxContent.Text == "")
                MessageBox.Show(errormessage, errortitle);
            else
            {
                Box.Close();
            }
            clicked = false;
        }
        public string ShowDialog()
        {
            Box.ShowDialog();
            return TBoxContent.Text;
        }

    }
}
