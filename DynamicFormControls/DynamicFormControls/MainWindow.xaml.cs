using FormModeller;
using MySql.Data.MySqlClient;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicFormControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button SubmitButton;
        private Button DeleteButton;
        private Button CreateButton;
        private Button InsertButton;
        private bool submitted = false;
        private DynamicForm myForm;
        private Window window;
        private StackPanel stackPanel;
        private SquirrelDB sDB;
        private DataTable dt;
        private FormModel myFormModel;
        private FormField myFormField;

        public MainWindow()
        {
            InitializeComponent();
            sDB = new SquirrelDB();

            sDB.loadFactories();
            sDB.connectFactories();
            sDB.Basepath = "C:\\SquirrelDB\\Databases\\";
            sDB.Database = "hrDB";
            sDB.Username = "dev";
            sDB.Password = "dev";
            sDB.Host = "localhost";
            sDB.Extension = "sq3";
            sDB.setMySqlConnectionString();
            sDB.setSqliteConnectionString();
            sDB.setSelectStatement();
        }

        private void populateListbox()
        {
        }

        private void configForm()
        {
            string[] tables = { "employees" };//, "ac_ok_test", "address", "allergen_severity", "allergies", "assignedhours", "assignedstaff", "children", "clientaddress", "clientphone", "clientschedule", "contact", "custody", "diagnosis", "emergencycontact", "familyhistory", "formsblank", "formscomplete", "formsneeded", "gender", "guardian", "healthcareproviders", "housingconcerns", "income", "initials", "insurance", "insurer", "livingarrangements", "locustest", "marital", "medicalconcerns", "medicalsupports", "med_management", "name", "orgaddress", "organization", "orgphones", "phone", "possible_assistance_sources", "presentingproblem", "providertypes", "referral", "releases", "ssn", "statefederalentitlements", "vocational", "vocationalfinancialneeds", "zipcodes", };
            foreach (string table in tables)
            {
                sDB.Table = table;
                sDB.getColumnsFromLocalTable();
                dt = sDB.getLocalTable();
                sDB.setVartypes();
            }
        }

        private void createEmptyControls()
        {
            string[] cols = sDB.Columns.ToArray();
            foreach (string s in cols)
            {
                CrudControl c = new CrudControl();

                c.Database = "hrDB";
                c.Tablename = sDB.Table;
                c.WpfControl = new TextBox();
                c.BackgroundColor = new SolidColorBrush(Colors.White);
                c.BorderColor = new SolidColorBrush(Colors.Black);
                c.ForegroundColor = new SolidColorBrush(Colors.Black);
                c.ErrorColor = new SolidColorBrush(Colors.Red);
                c.Width = 128;
                c.Height = 28;
                c.Columnname = s;
                c.LabelText = s;
                c.WpfControlLabel.FontSize = 12;
                c.WpfControlLabel.FontWeight = FontWeights.Bold;
                c.MysqlDataType = MySqlDbType.VarChar;
                c.applyStyle();

                c.Value = "";

                myForm.Controls.Add(c);
            }
        }

        /// <summary>
        /// Loads form from current file, and displays it.
        /// </summary>
        private void getControlsFromFormModel()
        {
        }

        private void createControls()
        {
            string[] cols = sDB.Columns.ToArray();

            foreach (string s in cols)
            {
                CrudControl c = new CrudControl();

                c.Database = "clientDB";
                c.Tablename = sDB.Table;
                c.WpfControl = new TextBox();
                c.BackgroundColor = new SolidColorBrush(Colors.White);
                c.BorderColor = new SolidColorBrush(Colors.Black);
                c.ForegroundColor = new SolidColorBrush(Colors.Black);
                c.ErrorColor = new SolidColorBrush(Colors.Red);
                c.Width = 128;
                c.Height = 28;
                c.Columnname = s;
                c.LabelText = s;
                c.WpfControlLabel.FontSize = 12;
                c.WpfControlLabel.FontWeight = FontWeights.Bold;
                c.MysqlDataType = MySqlDbType.VarChar;
                c.applyStyle();

                c.Value = dt.Rows[customersListBox.SelectedIndex].ItemArray[dt.Columns[s].Ordinal].ToString();
                myForm.Controls.Add(c);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (customersListBox.SelectedIndex > -1)
            {
                myForm = new DynamicForm();
                configForm();
                createControls();
                submitted = false;
                window = new Window() { MaxWidth = 400, MaxHeight = 1080, Width = 400, Height = 600, MinWidth = 400, MinHeight = 600, WindowStyle = WindowStyle.ThreeDBorderWindow, SizeToContent = SizeToContent.WidthAndHeight };
                myForm.Panel = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, VerticalAlignment = System.Windows.VerticalAlignment.Stretch };
                myForm.Panel.Children.Add(new Label { Content = "Label", Name = "formLabel" });
                myForm.Panel.Children.Add(new CheckBox { Name = "ValidCheckBox", Width = 18, Height = 18 });
                SubmitButton = new Button() { Name = "UpdateButton", Content = "Update", Width = 64, Height = 28 };
                SubmitButton.Click += new RoutedEventHandler(SubmitButton_Click);
                DeleteButton = new Button() { Name = "DeleteButton", Content = "Delete", Width = 64, Height = 28 };
                DeleteButton.Click += new RoutedEventHandler(DeleteButton_Click);
                foreach (CrudControl c in myForm.Controls)
                {
                    myForm.Panel.Children.Add(c.WpfControlLabel);

                    myForm.Panel.Children.Add(c.WpfControl);
                }
                myForm.Panel.Children.Add(SubmitButton);
                myForm.Panel.Children.Add(DeleteButton);
                myForm.Window = window;
                ScrollViewer scr = new ScrollViewer();
                scr.Content = myForm.Panel;
                myForm.Window.Content = scr;
                myForm.Window.Show();
            }
            else
            {
                MessageBox.Show("Select an item.");
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            myForm = new DynamicForm();
            configForm();
            createEmptyControls();
            submitted = false;
            window = new Window() { MaxWidth = 400, MaxHeight = 1080, Width = 400, Height = 600, MinWidth = 400, MinHeight = 600, WindowStyle = WindowStyle.ThreeDBorderWindow, SizeToContent = SizeToContent.WidthAndHeight };
            myForm.Panel = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, VerticalAlignment = System.Windows.VerticalAlignment.Stretch };
            myForm.Panel.Children.Add(new Label { Content = "Label", Name = "formLabel" });
            myForm.Panel.Children.Add(new CheckBox { Name = "ValidCheckBox", Width = 18, Height = 18 });
            InsertButton = new Button() { Name = "InsertButton", Content = "Insert Fresh Data", Width = 164, Height = 28 };
            InsertButton.Click += new RoutedEventHandler(InsertButton_Click);
            foreach (CrudControl c in myForm.Controls)
            {
                myForm.Panel.Children.Add(c.WpfControlLabel);
                myForm.Panel.Children.Add(c.WpfControl);
            }
            myForm.Panel.Children.Add(InsertButton);
            //myForm.Panel.Children.Add(DeleteButton);
            myForm.Window = window;
            ScrollViewer scr = new ScrollViewer();
            scr.Content = myForm.Panel;
            myForm.Window.Content = scr;
            myForm.Window.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            sDB.setupMysqlTable();

            if (!submitted)
            {
                sDB.myConnection.Open();
                string priKey;
                sDB.myAdapter = sDB.myFactory.CreateDataAdapter();
                foreach (CrudControl c in myForm.Controls)
                {
                    c.Value = null;
                    c.setTextValue(null);
                }
                //sDB.myAdapter.UpdateCommand;
                for (int i = 0; i < myForm.Controls.Count; i++)
                {
                    if (!dt.Rows[customersListBox.SelectedIndex].Table.Constraints.Contains(dt.Columns[i].ColumnName))
                    {
                        try
                        {
                            dt.Rows[customersListBox.SelectedIndex][i] = "";
                        }
                        catch (Exception)
                        {
                            try
                            {
                                dt.Rows[customersListBox.SelectedIndex][i] = new MySql.Data.Types.MySqlDecimal();
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    dt.Rows[customersListBox.SelectedIndex][i] = new MySql.Data.Types.MySqlDateTime();
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                            sDB.myAdapter.UpdateCommand.Parameters[i].Value = (((TextBox)myForm.Controls[i].WpfControl).Text);
                            sDB.myAdapter.Update(dt);
                            sDB.myConnection.Close();
                            myForm.Panel.Children.Add(new Label() { Content = "form submitted" });
                            submitted = true;
                        }
                    }
                }
                sDB.myConnection.Close();
            }

            myForm.updateMySql();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!submitted)
            {
                if (sDB.liteConnection == null)
                {
                    sDB.liteConnection = new SQLiteConnection(sDB.SQLiteConnectionString);
                    sDB.liteCommand = sDB.liteFac.CreateCommand();
                }

                //6sDB.liteConnection.Open();
                DateTime date;
                sDB.liteAdapter.UpdateCommand.Prepare();
                for (int i = 0; i < myForm.Controls.Count; i++)
                {
                    sDB.liteAdapter.UpdateCommand.Parameters[i].Value = (((TextBox)myForm.Controls[i].WpfControl).Text);
                    if (dt.Rows[customersListBox.SelectedIndex][i].GetType().ToString() == "MySql.Data.Types.MySqlDateTime")
                    {
                        date = DateTime.Parse((((TextBox)myForm.Controls[i].WpfControl).Text));
                        MySql.Data.Types.MySqlDateTime myDate = new MySql.Data.Types.MySqlDateTime(date);
                        dt.Rows[customersListBox.SelectedIndex][i] = myDate;
                    }
                    else
                    {
                        try
                        {
                            dt.Rows[customersListBox.SelectedIndex][i] = (((TextBox)myForm.Controls[i].WpfControl).Text);
                        }
                        catch
                        {
                            dt.Rows[customersListBox.SelectedIndex][i] = (dt.Rows[customersListBox.SelectedIndex][i]);
                        }
                    }
                }
                sDB.liteAdapter.Update(dt);
                sDB.liteConnection.Close();
                myForm.Panel.Children.Add(new Label() { Content = "form submitted" });
                submitted = true;
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!submitted)
            {
                DataRow newrow = dt.NewRow();

                int NewRowIndex = dt.Rows.Count;

                sDB.myConnection.Open();
                DateTime date;
                sDB.myAdapter.InsertCommand.Prepare();
                for (int i = 0; i < myForm.Controls.Count; i++)
                {
                    sDB.myAdapter.InsertCommand.Parameters[i].Value = (((TextBox)myForm.Controls[i].WpfControl).Text);
                    if (newrow[i].GetType().ToString() == "MySql.Data.Types.MySqlDateTime")
                    {
                        date = DateTime.Parse((((TextBox)myForm.Controls[i].WpfControl).Text));
                        MySql.Data.Types.MySqlDateTime myDate = new MySql.Data.Types.MySqlDateTime(date);
                        newrow[i] = myDate;
                    }
                    else
                    {
                        try
                        {
                            newrow[i] = (((TextBox)myForm.Controls[i].WpfControl).Text);
                        }
                        catch
                        {
                            newrow[i] = 0;

                            dt.Rows.Add(newrow);
                        }
                        sDB.myAdapter.Update(dt);
                        sDB.myConnection.Close();
                        myForm.Panel.Children.Add(new Label() { Content = "form submitted" });
                        submitted = true;
                    }
                }
            }
        }

        private void customersButton_Click(object sender, RoutedEventArgs e)
        {
            customersListBox.Items.Clear();
            configForm();

            dt = sDB.getLocalTable();
            foreach (DataRow row in dt.Rows)
            {
                customersListBox.Items.Add(row[1].ToString());
            }
        }
    }
}

/// <summary>
/// DynamicForm - A form with crud function, wpf, and hopefully html export;
/// </summary>
internal class DynamicForm
{
    private Window _window;
    private StackPanel _panel;
    private string _user = "dev";
    private string _pass = "dev";
    private string _server = "localhost";
    private string _connectString = "Host = {0}, Database = {1}, Uid = {2}, Pwd = {3}";
    private string _insertCommand = "INSERT INTO {0}({1}) VALUES (@{2})";
    private string _updateCommand = "UPDATE {0} SET {1}=@{2} WHERE {3}=@{4}";
    private string _sql;
    private string _table;
    private List<string> _columns;
    private SolidColorBrush _backgroundColor;
    private List<CrudControl> _controls;
    private int _formHeight;
    private int _formWidth;
    private int _formHorizontalMargin;
    private int _formVerticalMargin;
    private int index;
    private DataSet _tables;
    public DbDataAdapter liteAdapter;
    public DbCommandBuilder liteBuilder;
    public DbCommand liteCommand;
    public DbConnection liteConnection;

    private DbProviderFactory liteFac;

    public DynamicForm()
    {
        this._window = new Window();
        this._panel = new StackPanel();
        this.Controls = new List<CrudControl>();
        this._controls = new List<CrudControl>();
        this._window.Content = this._panel;
    }

    public DataSet Tables { get { return _tables; } set { _tables = value; } }

    public List<string> Columns { get { return _columns; } set { _columns = value; } }

    public int FormHeight { get { return _formHeight; } set { _formHeight = value; } }

    public int FormWidth { get { return _formWidth; } set { _formWidth = value; } }

    public int FormHorizontalMargin { get { return _formHorizontalMargin; } set { _formHorizontalMargin = value; } }

    public int FormVerticalMargin { get { return _formVerticalMargin; } set { _formVerticalMargin = value; } }

    public Window Window { get { return _window; } set { _window = value; } }

    public StackPanel Panel { get { return _panel; } set { _panel = value; } }

    public List<CrudControl> Controls { get; set; }

    public SolidColorBrush BackgroundColor { get; set; }

    public int Index { get; set; }

    public string Table { get { return _table; } set { _table = value; } }

    public void setSelectStatement()
    {
        _sql = "SELECT";

        _sql += " ";
        foreach (string column in _columns)
        {
            _sql += "`" + column + "`" + ", ";
        }
        _sql = _sql.Substring(0, _sql.Length - 2);
        _sql += " FROM " + _table + ";";
    }

    /// <summary>
    /// updates a MySql database table from a form control;
    /// </summary>
    /// <param name="control"></param>
    public string updateControlValueMySql(CrudControl control)
    {
        MySqlConnection mycon = new MySqlConnection();

        MySqlCommand myCmd = new MySqlCommand();
        myCmd.Parameters.Add(control.Columnname, control.MysqlDataType).Value = control.Value;
        //mycon.Open();
        //myCmd.ExecuteNonQuery();
        return myCmd.CommandText;
    }

    private void createInsert(CrudControl control)
    {
        liteConnection = new SQLiteConnection(_connectString);
        setSelectStatement();
        DbCommand liteCommand = liteFac.CreateCommand();
        liteCommand.CommandText = _sql;
        liteCommand.Connection = liteConnection;
        liteCommand.Connection.Open();
        liteAdapter = liteFac.CreateDataAdapter();

        liteAdapter.SelectCommand = liteCommand;
        liteBuilder = liteFac.CreateCommandBuilder();
        liteBuilder.DataAdapter = liteAdapter;
        liteAdapter.InsertCommand = liteBuilder.GetInsertCommand();
        liteAdapter.UpdateCommand = liteBuilder.GetUpdateCommand();
        liteAdapter.DeleteCommand = liteBuilder.GetDeleteCommand();

        try
        {
            liteConnection.Open();
        }
        catch (Exception ex)
        {
        }
        try
        {
            int count = 0;

            using (IDbTransaction tran = liteConnection.BeginTransaction())
            {
                int rowCount = Tables.Tables[index].Rows.Count;
                int columnCount = Tables.Tables[index].Columns.Count;

                liteCommand.Connection = liteConnection;

                for (int row = 0; row < rowCount; row++)
                {
                    for (int column = 0; column < columnCount; column++)
                    {
                        liteAdapter.InsertCommand.Parameters[column].Value = Tables.Tables[index].Rows[row][column];
                    }
                    //liteCommand.ExecuteNonQuery();
                    liteAdapter.InsertCommand.ExecuteNonQuery();
                }
                tran.Commit();
                count = 0;
            }
        }
        catch (Exception ex)
        {
        }
        liteConnection.Close();
    }

    public void updateMySql()
    {
        foreach (CrudControl control in Controls)
        {
            updateControlValueMySql(control);
        }
    }

    /// <summary>
    /// retireves data from a crud control
    ///
    /// </summary>
    /// <param name="control"></param>
    public string provideControlValue(CrudControl control)
    {
        return control.Value;
    }

    public void insertMysql()
    {
        foreach (CrudControl c in Controls)
        {
            provideControlValue(c);
        }
    }
}

/// <summary>
///
/// </summary>
internal class CrudControl
{
    public CrudControl()
    {
        _borderColor = new SolidColorBrush();
        _backgroundColor = new SolidColorBrush();
        _foregroundColor = new SolidColorBrush();
        _wpfControl = new Control();
        _wpfControlLabel = new Label();
    }

    private string _keyColumn;
    private string _keyValue;
    private string _database;
    private string _tablename;
    private string _columnname;
    private MySqlDbType _mysqlDataType;
    private string _value;
    private string _sqliteDataType;
    private Control _wpfControl;
    private Label _wpfControlLabel;

    private string _htmlFormControl;
    private string _htmlControlType;
    private SolidColorBrush _borderColor;
    private SolidColorBrush _backgroundColor;
    private SolidColorBrush _foregroundColor;
    private SolidColorBrush _errorColor;
    private int _width;
    private int _height;

    public int Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
            _wpfControl.MaxWidth = value;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
        set
        {
            _height = value;
            _wpfControl.MaxHeight = value;
        }
    }

    private string _content;

    public string Database { get { return _database; } set { _database = value; } }

    public string Tablename { get { return _tablename; } set { _tablename = value; } }

    public string Columnname { get { return _columnname; } set { _columnname = value; } }

    public string RenderHtml { get { return _htmlFormControl; } }

    public string LabelText { get { return (string)this._wpfControlLabel.Content; } set { this._wpfControlLabel.Content = value; } }

    public Label WpfControlLabel { get { return _wpfControlLabel; } set { _wpfControlLabel = value; } }

    public Control WpfControl
    {
        get
        {
            return _wpfControl;
        }
        set
        {
            _wpfControl = value;
        }
    }

    public MySqlDbType MysqlDataType { get { return _mysqlDataType; } set { _mysqlDataType = value; } }

    public string SqliteDataType { get { return _sqliteDataType; } set { _sqliteDataType = value; } }

    public SolidColorBrush BorderColor { get { return _borderColor; } set { _borderColor = value; } }

    public SolidColorBrush BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }

    public SolidColorBrush ForegroundColor { get { return _foregroundColor; } set { _foregroundColor = value; } }

    public SolidColorBrush ErrorColor { get { return _errorColor; } set { _errorColor = value; } }

    public string Content { get { return _content; } set { _content = value; } }

    public string Value
    {
        get
        {
            return _value;
        }
        set
        {
            setTextValue(value);
            _value = value;
        }
    }

    public string KeyColumn { get { return _keyColumn; } set { _keyColumn = value; } }

    public string KeyValue { get { return _keyValue; } set { _keyValue = value; } }

    public string getTextValue()
    {
        TextBox txt = (TextBox)this._wpfControl;
        return txt.Text;
    }

    public string setTextValue(string value)
    {
        TextBox txt = (TextBox)this._wpfControl;
        txt.Text = value;
        try
        {
            this._wpfControl = txt;
            return txt.Text;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private int getIntValue()
    {
        return 0;
    }

    public void applyStyle()
    {
        if (this._height <= 0)
        {
            this._height = 18;
        }

        if (this._width <= 0)
        {
            this._width = 64;
        }

        if (this.BackgroundColor.Color == null)
        {
            this.BackgroundColor.Color = Colors.White;
        }

        if (this.BorderColor.Color == null)
        {
            this.BorderColor.Color = Colors.Black;
        }

        if (this.ForegroundColor.Color == null)
        {
            this.ForegroundColor.Color = Colors.Black;
        }

        this._wpfControl.Height = this._height;

        this._wpfControl.Width = this._width;

        this._wpfControl.Background = this._backgroundColor;

        this._wpfControl.BorderBrush = this._borderColor;

        this._wpfControl.Foreground = this._foregroundColor;
    }
}