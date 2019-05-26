using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Data;
using System.Windows.Threading;
using System.Windows.Input;
using System.ComponentModel;

namespace GameOfLife
{
    class MainViewModel : DependencyObject
    {
        public string StartStopTite
        {
            get { return (string)GetValue(StartStopTiteProperty); }
            set { SetValue(StartStopTiteProperty, value); }
        }
        public static readonly DependencyProperty StartStopTiteProperty =
            DependencyProperty.Register("StartStopTite", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));

        //поле отображения текущего поколения
        public string TextGeneration
        {
            get { return (string)GetValue(TextGenerationProperty); }
            set { SetValue(TextGenerationProperty, value); }
        }
        public static readonly DependencyProperty TextGenerationProperty =
            DependencyProperty.Register("TextGeneration", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));

        //пдоложка клеток
        public Canvas CanvasPanel
        {
            get { return (Canvas)GetValue(canvasPanelProperty); }
            set { SetValue(canvasPanelProperty, value); }
        }
        public static readonly DependencyProperty canvasPanelProperty =
           DependencyProperty.Register("contentControl", typeof(Canvas), typeof(MainViewModel), new PropertyMetadata(null));

        //radiobutton выбора типа поля
        public Boolean ClosedWord
        {
            get { return (Boolean)GetValue(closedWordProperty); }
            set { SetValue(closedWordProperty, value); }
        }
        public static readonly DependencyProperty closedWordProperty =
            DependencyProperty.Register("closedWord", typeof(Boolean), typeof(MainViewModel), new PropertyMetadata(true));


        DispatcherTimer Timer = new DispatcherTimer(); //таймер для обработки генерации новых поколений
        Rectangle[,] felder = new Rectangle[height, width]; //объект отрисовки клетки
        DbControl DbControl = new DbControl();
        DataTable DataTable = new DataTable();

        public int[,] ArrayFelder = new int[height, width];//размеры игрового поля. Хотел вынести на изменение
        const int width = 100;
        const int height = 70;
        public string query;
        public int Generation = 0; //отображает текуще поколение
        int SelectedId; //указывает на id выбранной строки в datagrid

        public MainViewModel()
        {
            StartStopTite = "Старт";
            Timer.Interval = TimeSpan.FromSeconds(0.1); // установка скорости обновления таймера
            Timer.Tick += Timer_tick;//назначение события на обновление таймера
            CanvasPanel = new Canvas();

            //MainView view = (MainView)App.Current.MainWindow;
            NewEmptyFieldCommand = new DelegateCommand(NewEmptyField);
            OpenFieldCommand = new DelegateCommand(OpenField);
            SaveFieldCommand = new DelegateCommand(SaveField);
            DelFieldCommand = new DelegateCommand(DelField);
            LoadRandFieldCommand = new DelegateCommand(LoadRandField);
            FieldRandCommand = new DelegateCommand(FieldRand);
            ExitCommand = new DelegateCommand(Exit);
            StartStopCommand = new DelegateCommand(StartStop);
            LogCommand = new DelegateCommand(OpenLog);
            OpenSaveCommand = new DelegateCommand(OpenSave);
            SaveSaveCommand = new DelegateCommand(SaveSave);
            DelSaveCommand = new DelegateCommand(DelSave);
            object startObject = new object();
            NewEmptyField(startObject);
        }

        #region DelegateCommand
        public DelegateCommand NewEmptyFieldCommand { get; private set; }
        public DelegateCommand OpenFieldCommand { get; set; }
        public DelegateCommand SaveFieldCommand { get; set; }
        public DelegateCommand DelFieldCommand { get; set; }
        public DelegateCommand LoadRandFieldCommand { get; set; }
        public DelegateCommand FieldRandCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand StartStopCommand { get; set; }
        public DelegateCommand LogCommand { get; set; }

        public DelegateCommand OpenSaveCommand { get; set; }
        public DelegateCommand SaveSaveCommand { get; set; }
        public DelegateCommand DelSaveCommand { get; set; }
        #endregion

        private void NewEmptyField(object obj)
        {
            StartStopTite = "Старт";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    ArrayFelder[i, j] = 0;//обнуляем все элементы массива и переаем на печать
                }
            }
            Generation = 0;
            Print(ArrayFelder);
        }
        private void OpenField(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            query = "SELECT [id], [name], [dateTime]  FROM [GameOfLifeDb].[dbo].[field] order by [id] ";
            DataTable = DbControl.Query(query);

            OpenViewMode openViewMode = new OpenViewMode(DataTable, "Открыть");
            SelectedId = openViewMode.ShowDialog();
            if (SelectedId > 0)
            {
                loadField(SelectedId, false);
            }
        }
        private void SaveField(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            string arrayFelderString = "";
            Boolean saveThis = false; //указывает требуется ли сохраить

            //считывание игрового поля в масив
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (felder[i, j].Fill == Brushes.DarkSlateGray)
                    {
                        ArrayFelder[i, j] = 1;
                    }
                    else
                    {
                        ArrayFelder[i, j] = 0;
                    }
                    arrayFelderString += ArrayFelder[i, j];
                }
                arrayFelderString += '!';
            }

            query = "SELECT data  FROM [GameOfLifeDb].[dbo].[field] order by [id] desc";
            DataTable = DbControl.Query(query);

            //проверка на наличие подобных в базе и запись
            if (DataTable.Rows.Count == 0)//проверка на пустую базу
            {
                InputBox inputBox = new InputBox("Введите название для растановки");
                string nameField = inputBox.ShowDialog();

                if ( nameField != null) //при нажатии кнопки отмена возвращается null
                {
                    query = "INSERT INTO [dbo].[field] ([name],[data],[dateTime]) VALUES ('" + nameField + "' ,'" + arrayFelderString + "','" + DateTime.Now + "')";
                    DbControl.Query(query);
                }
            }
            else
            {
                for (int index = 0; index < DataTable.Rows.Count; index++)
                {
                    if (DataTable.Rows[index]["data"].ToString() == arrayFelderString)
                    {
                        MessageBox.Show("Ошибка! Данная растановка уже записанна в базу");
                        break;
                    }
                    else
                    {
                        saveThis = true; //используется для сохранения растановки только один раза
                    }
                }

                if (saveThis)
                {
                    InputBox inputBox = new InputBox("Введите название для растановки");
                    string nameField = inputBox.ShowDialog();

                    if (nameField != null) //при нажатии кнопки отмена возвращается null
                    {
                        query = "INSERT INTO [dbo].[field] ([name],[data],[dateTime]) VALUES ('" + nameField + "' ,'" + arrayFelderString + "','" + DateTime.Now + "')";
                        DbControl.Query(query);
                        MessageBox.Show("Растановка успешно сохранена");
                    }
                }

            }
        }
        private void DelField(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            query = "SELECT [id], [name], [dateTime]  FROM [GameOfLifeDb].[dbo].[field] order by [id] ";
            DataTable = DbControl.Query(query);

            OpenViewMode openViewMode = new OpenViewMode(DataTable, "Уалить");
            SelectedId = openViewMode.ShowDialog();

            query = "DELETE FROM [GameOfLifeDb].[dbo].[field] WHERE ID = '"+ SelectedId+"'";
            DbControl.Query(query);
        }
        private void LoadRandField(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            Random rand = new Random();
            Boolean reset = false; //отображает нужно ли повторно вызвать мето
            Generation = 0;
            TextGeneration = "Поколение: " + Generation.ToString();
            query = "SELECT *  FROM [GameOfLifeDb].[dbo].[field] order by [id] desc";
            DataTable = DbControl.Query(query);
            
            int randomId = rand.Next(1, Convert.ToInt16( DataTable.Rows[0]["Id"] )); //генерируем randomId в диапозоне от 1 о посленего значения id

            if (DataTable.Rows.Count != 0)//проверка на пустую базу
            {
                for (int index = 0; index < DataTable.Rows.Count; index++) //проверка на наличие в базе растановки с id = randomId (при удалении в базе могут быть пропуски)
                {
                    if (Convert.ToInt16(DataTable.Rows[index]["Id"]) == randomId)
                    {
                        loadField(randomId, false); //передача false параметром меняет sql запрос в loadField для загрузки сохранения
                        break;
                    }
                    else
                    {
                        reset = true;
                    }
                }
                if (reset)
                {
                    LoadRandField(obj);
                }
            }
            else
            {
                MessageBox.Show("Ошибка! В базе данных нет растановок.");
            }
        }
        private void FieldRand(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            Random rand = new Random();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    ArrayFelder[i, j] = rand.Next(0, 2);
                }
            }
            Print(ArrayFelder);
        }
        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }
        private void StartStop(object obj)
        {
            LogDB(StartStopTite);
            if (Timer.IsEnabled)
            {
                Timer.Stop();
                StartStopTite = "Старт";
            }
            else
            {
                Timer.Start();
                StartStopTite = "Стоп";
            }
        }
        public void Print(int[,] arrayFelder)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            CanvasPanel.Background = Brushes.LightGray;
            CanvasPanel.Height = height * 17 - 2; // размеры пдоложки клеток. 17 - размер клетки с учетом границб "-2" - убирает полоску пдоложки внизу и справа
            CanvasPanel.Width = width * 17 - 2;
            CanvasPanel.HorizontalAlignment = HorizontalAlignment.Center;
            CanvasPanel.VerticalAlignment = VerticalAlignment.Center;
            CanvasPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            CanvasPanel.Arrange(new Rect(0.0, 0.0, CanvasPanel.Width, CanvasPanel.DesiredSize.Height));

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = 15; //размеры клетки
                    rectangle.Height = 15;

                    if (arrayFelder[i, j] == 1)
                        rectangle.Fill = Brushes.DarkSlateGray; //живаая клетка
                    else
                        rectangle.Fill = Brushes.DarkGray; //мертвая клетка

                    CanvasPanel.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, j * 17);
                    Canvas.SetTop(rectangle, i * 17);
                    rectangle.MouseDown += R_MouseDown;
                    felder[i, j] = rectangle;
                }
            }
        }
        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle)sender).Fill = (((Rectangle)sender).Fill == Brushes.DarkGray) ? Brushes.DarkSlateGray : Brushes.DarkGray;
        }
        private void Timer_tick(object sender, EventArgs e)
        {
            //       j_left  0 j_right
            //    i_up [ ]  [ ]  [ ]
            //       0 [ ]  [X]  [ ]
            //  i_down [ ]  [ ]  [ ]

            int[,] countLifeNeighbors = new int[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int neighbors = 0;

                    int i_up = 0;
                    int i_down = 0;
                    int j_right = 0;
                    int j_left = 0;

                    #region проверка для замкнутого поля
                    if (ClosedWord)
                    {
                        i_up = i - 1;
                        if (i_up < 0)
                            i_up = height - 1;
                        i_down = i + 1;
                        if (i_down >= height)
                            i_down = 0;

                        j_right = j - 1;
                        if (j_right < 0)
                            j_right = width - 1;
                        j_left = j + 1;
                        if (j_left >= width)
                            j_left = 0;
                    }
                    #endregion

                    #region проверка для органиченного поля
                    else
                    {
                        i_up = i - 1;
                        i_down = i + 1;
                        j_right = j + 1;
                        j_left = j - 1;
                        //если сосед проверяемого элемента находится за границам масива,
                        // то проверяем текущий элемент. если  этот элемент живой, то отнимает из учета живых соседей, для компенсации

                        if (i - 1 < 0)
                        {           
                            i_up = i;
                            if (felder[i, j].Fill == Brushes.DarkSlateGray) neighbors--;
                        }
                        if (i + 1 >= height)
                        {
                            i_down = i;
                            if (felder[i, j].Fill == Brushes.DarkSlateGray) neighbors--;
                        }

                        if (j - 1 < 0)
                        {
                            j_left = j;
                            if (felder[i, j].Fill == Brushes.DarkSlateGray) neighbors--;
                        }
                        if (j + 1 >= width)
                        {
                            j_right = j;
                            if (felder[i, j].Fill == Brushes.DarkSlateGray) neighbors--;
                        }
                    }
                    #endregion

                    //считаем живых соседей
                    if (felder[i_up, j_right].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i_up, j].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i_up, j_left].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i, j_right].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i, j_left].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i_down, j_right].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i_down, j].Fill == Brushes.DarkSlateGray)
                        neighbors++;
                    if (felder[i_down, j_left].Fill == Brushes.DarkSlateGray)
                        neighbors++;

                    countLifeNeighbors[i, j] = neighbors;
                }
            }

            Generation++;
            TextGeneration = "Поколение: " + Generation.ToString();
            
            //раскрашиваем поле в соответствии с соседями
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (countLifeNeighbors[i, j] < 2 || countLifeNeighbors[i, j] > 3)
                        felder[i, j].Fill = Brushes.DarkGray;
                    else if (countLifeNeighbors[i, j] == 3)
                        felder[i, j].Fill = Brushes.DarkSlateGray;
                }
            }
        }
        public void loadField(int id, Boolean thisSave)
        {
            if (thisSave)
            {
                 query = "Select * from [GameOfLifeDb].[dbo].[save] where id = '" + id + "'";
            }
            else
            {
                 query = "Select * from [GameOfLifeDb].[dbo].[field] where id = '" + id + "'";
            }
            DataTable = DbControl.Query(query);
            if (thisSave)
            {
                Generation = Convert.ToInt16( DataTable.Rows[0]["gen"]);
                TextGeneration = "Поколение: " + Generation;
            }

            char[] arrayFelderString = (DataTable.Rows[0]["data"].ToString()).ToCharArray();

            for (int i = 0, j = 0, index = 0; index < arrayFelderString.Length; index++)
            {
                if (arrayFelderString[index].ToString() == "!")
                {
                    j++;
                    i = 0;
                }
                else
                {
                    ArrayFelder[j, i] = Convert.ToInt16(arrayFelderString[index].ToString());
                    i++;
                }
            }
            Print(ArrayFelder);
        }

        private void LogDB(string startStop)
        {
            string arrayFelderString = "";

            //считывание игрового поля в масив
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (felder[i, j].Fill == Brushes.DarkSlateGray)
                    { ArrayFelder[i, j] = 1; }
                    else
                    { ArrayFelder[i, j] = 0; }
                    arrayFelderString += ArrayFelder[i, j];
                }
                arrayFelderString += '!';
            }

            query = "INSERT INTO [dbo].[log] ([type],[dateTime],[field]) VALUES ('" + startStop + "' ,'" + DateTime.Now + "','" + arrayFelderString + "')";
            DbControl.Query(query);
        }
        private void OpenLog(object obj)
        {
            query = "SELECT * FROM [GameOfLifeDb].[dbo].[log] order by [id] ";
            DataTable = DbControl.Query(query);

            OpenViewMode openViewMode = new OpenViewMode(DataTable, "ok");
            SelectedId = openViewMode.ShowDialog();
        }

        private void OpenSave(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            query = "SELECT [id], [name], [gen], [dateTime]  FROM [GameOfLifeDb].[dbo].[save] order by [id] ";
            DataTable = DbControl.Query(query);

            OpenViewMode openViewMode = new OpenViewMode(DataTable, "Открыть");
            SelectedId = openViewMode.ShowDialog();
            loadField(SelectedId, true);
        }
        private void SaveSave(object obj)
        {
            //останавливаем генерцию новых поколений
            Timer.Stop();
            StartStopTite = "Старт";

            string arrayFelderString = "";
            //считывание игрового поля в масив
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (felder[i, j].Fill == Brushes.DarkSlateGray)
                        ArrayFelder[i, j] = 1; 
                    else
                        ArrayFelder[i, j] = 0;

                    arrayFelderString += ArrayFelder[i, j];
                }
                arrayFelderString += '!';
            }

            query = "SELECT data  FROM [GameOfLifeDb].[dbo].[save] order by [id] desc";
            DataTable = DbControl.Query(query);

            //проверка на наличие подобных в базе и запись
            if (DataTable.Rows.Count == 0)//проверка на пустую базу
            {
                InputBox inputBox = new InputBox("Enter a name for the field");
                string nameField = inputBox.ShowDialog();

                query = "INSERT INTO [dbo].[save] ([name],[data],[gen],[dateTime]) VALUES ('" + nameField + "' ,'" + arrayFelderString + "','"+ Generation + "','" + DateTime.Now + "')";
                DbControl.Query(query);
            }
            else
            {
                for (int index = 0; index < DataTable.Rows.Count; index++)
                {
                    if (DataTable.Rows[index]["data"].ToString() == arrayFelderString)
                    {
                        MessageBox.Show("Ошибка! Данная растановка уже записанна в базу");
                    }
                    else
                    {
                        InputBox inputBox = new InputBox("Введите название сохранения");
                        string nameField = inputBox.ShowDialog();

                        query = "INSERT INTO [dbo].[field] ([name],[data],[dateTime]) VALUES ('" + nameField + "' ,'" + arrayFelderString + "','" + DateTime.Now + "')";
                        DbControl.Query(query);
                        MessageBox.Show("Сохранение успешно записанно");
                    }
                }
            }
        }
        private void DelSave(object obj)
        {
            Timer.Stop();
            StartStopTite = "Старт";
            query = "SELECT [id], [name], [gen], [dateTime]  FROM [GameOfLifeDb].[dbo].[save] order by [id] ";
            DataTable = DbControl.Query(query);

            OpenViewMode openViewMode = new OpenViewMode(DataTable, "Открыть");
            SelectedId = openViewMode.ShowDialog();
            query = "DELETE FROM [GameOfLifeDb].[dbo].[save] WHERE ID = '" + SelectedId+"'";
            DbControl.Query(query);
        }
    }
}
