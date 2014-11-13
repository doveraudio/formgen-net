using FormModeller;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FormDesigner
{
    /// <summary>
    /// Interaction logic for FormToolBox.xaml
    /// </summary>
    public partial class FormToolBox : Window
    {
        private SolidColorBrush BrushFormBackground;
        private SolidColorBrush BrushFormForeground;
        private SolidColorBrush BrushFormBorder;
        private SolidColorBrush BrushFieldBackground;
        private SolidColorBrush BrushFieldForeground;
        private SolidColorBrush BrushFieldBorder;
        private SolidColorBrush BrushFieldError;
        private FormField _field;
        private FormModel _form;
        private List<string> _SelectedDBType;
        private ObservableCollection<string> FieldTypes;
        private Dictionary<String, KeyValuePair<long, long>> DBVarTypes;

        public FormField Field { get; set; }

        public FormModel Form { get; set; }

        /// <summary>
        /// Initialize Color Brushes for Color Boxes,
        /// placeholders for the color sliders.
        /// </summary>
        private void initColorBrushes()
        {
            BrushFormBackground = new SolidColorBrush();
            BrushFormForeground = new SolidColorBrush();
            BrushFormBorder = new SolidColorBrush();
            BrushFieldBackground = new SolidColorBrush();
            BrushFieldBorder = new SolidColorBrush();
            BrushFieldForeground = new SolidColorBrush();
            BrushFieldError = new SolidColorBrush();
        }

        /// <summary>
        /// Initialize the Database Variable Types used in the ORM Model
        /// </summary>
        private void initDBVarTypes()
        {
            DBVarTypes = new Dictionary<string, KeyValuePair<Int64, Int64>>();
            DBVarTypes.Clear();
            DBVarTypes.Add("CHAR", new KeyValuePair<Int64, Int64>(0, 255));
            DBVarTypes.Add("VARCHAR", new KeyValuePair<Int64, Int64>(0, 255));
            DBVarTypes.Add("TEXT", new KeyValuePair<Int64, Int64>(0, 65535));
            DBVarTypes.Add("MEDIUMTEXT", new KeyValuePair<Int64, Int64>(0, 16777215));
            DBVarTypes.Add("LONGTEXT", new KeyValuePair<Int64, Int64>(0, 4294967295));
            DBVarTypes.Add("INT", new KeyValuePair<Int64, Int64>(0, 0));
            DBVarTypes.Add("DOUBLE", new KeyValuePair<Int64, Int64>(10, 10));
            DBVarTypes.Add("DECIMAL", new KeyValuePair<Int64, Int64>(10, 10));
            DBVarTypes.Add("DATE", new KeyValuePair<Int64, Int64>(0, 0));
            DBVarTypes.Add("DATETIME", new KeyValuePair<Int64, Int64>(0, 0));
            DBVarTypes.Add("TIME", new KeyValuePair<Int64, Int64>(0, 0));
        }

        /// <summary>
        /// Add the field types to the FieldType ComboBox.
        /// </summary>
        public void addFieldTypes()
        {
            FieldTypes = new ObservableCollection<string>();
            FieldTypes.Add("Textbox");
            FieldTypes.Add("Checkbox");
            FieldTypes.Add("Radio");
            FieldTypes.Add("Date");
            FieldTypes.Add("Combobox");
            this.ComboBoxFieldType.ItemsSource = FieldTypes;
        }

        /// <summary>
        /// FormToolBox Constructor. Call any Init Functions here.
        /// </summary>
        public FormToolBox()
        {
            initColorBrushes();
            InitializeComponent();
            initDBVarTypes();
            addFieldTypes();
            SliderFormWidth.Maximum = 1600;
            SliderFormHeight.Maximum = 900;
            ComboBoxFieldType.SelectedIndex = 0;
            ListBoxDatabaseType.SelectedIndex = 0;
            _field = new FormField();
            _form = new FormModel();
            Field = _field;
            Form = _form;
        }

        private void SliderMValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxMValue.Text = Math.Round(SliderMValue.Value).ToString();
        }

        private void SliderDValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxDValue.Text = Math.Round(SliderDValue.Value).ToString();
        }

        /// <summary>
        /// Checks a string for the presence of numbers vs alphanumeric characters
        /// </summary>
        /// <param name="text">the string to check</param>
        /// <returns>true if a number, false for an alphanumeric character</returns>
        private bool checkNumber(string text)
        {
            Regex exp = new Regex(@"^\d+$");
            return exp.IsMatch(text);
        }

        /// <summary>
        /// removes letters from an alphanumeric string
        /// </summary>
        /// <param name="txtbox"></param>
        /// <returns>the text that has been cleaned</returns>
        private string eraseLetters(TextBox txtbox)
        {
            string current = txtbox.Text;
            if (txtbox.Text.Length > 0)
            {
                foreach (char letter in current)
                {
                    if (!checkNumber(letter.ToString()))
                    {
                        current = current.Replace(letter.ToString(), "");
                    };
                }
            }
            return current;
        }

        /// <summary>
        /// Cleans the letters from a textbox, using eraseLetters and checkNumber
        /// </summary>
        /// <param name="sender">The textbox to clean</param>
        /// <returns>The cleaned textbox</returns>
        private TextBox CleanLetters(object sender)
        {
            TextBox txtbx = (TextBox)sender;
            int oldSize = txtbx.Text.Length;
            int oldPos = txtbx.SelectionStart;
            int difSize = 0;

            txtbx.Text = eraseLetters((TextBox)sender);
            difSize = oldSize - txtbx.Text.Length;
            txtbx.SelectionStart = oldPos - difSize;
            return txtbx;
        }

        /// <summary>
        /// Checks if a string is empty, in which case replaces with zero;
        /// </summary>
        /// <param name="text">the string to check and or replace</param>
        /// <returns></returns>
        private string checkFieldEmptyToZero(string text)
        {
            if (text == "")
            {
                return "0";
            }
            else
            {
                return text;
            }
        }

        private void TextBoxMValue_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxMValue = CleanLetters(sender);
        }

        private void TextBoxDValue_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxDValue = CleanLetters(sender);
        }

        /// <summary>
        /// checks the state of a listboxitem, adjusts the range sliders according to DBVarType selected.
        /// </summary>
        /// <param name="sender">the ListBoxItem currently calling.</param>
        /// <param name="e">The event arguiments.</param>
        private void setVarType(object sender, RoutedEventArgs e)
        {
            ListBoxItem temp = (ListBoxItem)sender;
            switch (temp.Name)
            {
                case "DBchar":
                    SliderMValue.Maximum = DBVarTypes["CHAR"].Key;
                    SliderDValue.Maximum = DBVarTypes["CHAR"].Value;
                    TextBlockDecimal.Text = "Text Length";
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Collapsed;
                    SliderMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBoxMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockInteger.Text = "N/A";
                    GridSize.Visibility = System.Windows.Visibility.Visible;

                    break;

                case "DBvarchar":
                    SliderMValue.Maximum = DBVarTypes["VARCHAR"].Key;
                    SliderDValue.Maximum = DBVarTypes["VARCHAR"].Value;
                    TextBlockDecimal.Text = "Text Length";
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Collapsed;
                    SliderMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBoxMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockInteger.Text = "N/A";
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBtext":
                    SliderMValue.Maximum = DBVarTypes["TEXT"].Key;
                    SliderDValue.Maximum = DBVarTypes["TEXT"].Value;
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;

                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;
                    TextBlockInteger.Visibility = System.Windows.Visibility.Collapsed;

                    SliderMValue.Visibility = System.Windows.Visibility.Collapsed;

                    TextBoxMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockInteger.Text = "N/A";
                    TextBlockDecimal.Text = "Text Length";
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBmediumtext":
                    SliderMValue.Maximum = DBVarTypes["MEDIUMTEXT"].Key;
                    SliderDValue.Maximum = DBVarTypes["MEDIUMTEXT"].Value;
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;

                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Collapsed;

                    SliderMValue.Visibility = System.Windows.Visibility.Collapsed;

                    TextBoxMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBlockInteger.Text = "N/A";
                    TextBlockDecimal.Text = "Text Length";
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBlongtext":
                    SliderMValue.Maximum = DBVarTypes["LONGTEXT"].Key;
                    SliderDValue.Maximum = DBVarTypes["LONGTEXT"].Value;

                    SliderDValue.Visibility = System.Windows.Visibility.Visible;

                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Collapsed;
                    SliderMValue.Visibility = System.Windows.Visibility.Collapsed;
                    TextBoxMValue.Visibility = System.Windows.Visibility.Collapsed;

                    TextBlockInteger.Text = "N/A";
                    TextBlockDecimal.Text = "Text Length";
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBint":
                    SliderMValue.Maximum = 0;
                    SliderMValue.Minimum = DBVarTypes["INT"].Key;
                    SliderDValue.Maximum = DBVarTypes["INT"].Value;
                    GridSize.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case "DBdouble":

                    SliderMValue.Maximum = DBVarTypes["DOUBLE"].Key;
                    SliderDValue.Maximum = DBVarTypes["DOUBLE"].Value;

                    TextBlockInteger.Text = "Numerical Places";
                    TextBlockDecimal.Text = "Decimal Places";

                    SliderMValue.Visibility = System.Windows.Visibility.Visible;
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;

                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBoxMValue.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBdecimal":
                    SliderMValue.Maximum = DBVarTypes["DECIMAL"].Key;
                    SliderDValue.Maximum = DBVarTypes["DECIMAL"].Value;

                    SliderMValue.Visibility = System.Windows.Visibility.Visible;
                    SliderDValue.Visibility = System.Windows.Visibility.Visible;

                    TextBoxDValue.Visibility = System.Windows.Visibility.Visible;
                    TextBoxMValue.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Visibility = System.Windows.Visibility.Visible;
                    TextBlockDecimal.Visibility = System.Windows.Visibility.Visible;

                    TextBlockInteger.Text = "Numerical Places";
                    TextBlockDecimal.Text = "Decimal Places";
                    GridSize.Visibility = System.Windows.Visibility.Visible;
                    break;

                case "DBdate":
                    SliderMValue.Maximum = DBVarTypes["DATE"].Key;
                    SliderDValue.Maximum = DBVarTypes["DATE"].Value;
                    GridSize.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case "DBdatetime":
                    SliderMValue.Maximum = DBVarTypes["DATETIME"].Key;
                    SliderDValue.Maximum = DBVarTypes["DATETIME"].Value;
                    GridSize.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case "DBtime":
                    GridSize.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        private void CheckDBType(ListBoxItem selected)
        {
            if (TextBoxMValue.Text == "") { TextBoxMValue.Text = "0"; }
            if (TextBoxDValue.Text == "") { TextBoxDValue.Text = "0"; }

            switch (selected.Name)
            {
                case "DBchar":

                    _SelectedDBType = new List<string>(new string[] { "CHAR", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBvarchar":
                    _SelectedDBType = new List<string>(new string[] { "VARCHAR", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBtext":
                    _SelectedDBType = new List<string>(new string[] { "TEXT", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBmediumtext":
                    _SelectedDBType = new List<string>(new string[] { "MEDIUMTEXT", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBlongtext":
                    _SelectedDBType = new List<string>(new string[] { "LONGTEXT", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBint":
                    _SelectedDBType = new List<string>(new string[] { "INT", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBdouble":

                    _SelectedDBType = new List<string>(new string[] { "DOUBLE", TextBoxMValue.Text, TextBoxDValue.Text });
                    break;

                case "DBdecimal":
                    _SelectedDBType = new List<string>(new string[] { "DECIMAL", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBdate":
                    _SelectedDBType = new List<string>(new string[] { "DATE", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBdatetime":
                    _SelectedDBType = new List<string>(new string[] { "DATETIME", TextBoxMValue.Text, TextBoxDValue.Text });

                    break;

                case "DBtime":
                    _SelectedDBType = new List<string>(new string[] { "TIME", TextBoxMValue.Text, TextBoxDValue.Text });
                    break;
            }
        }

        private void SliderXPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxXPosition.Text = Math.Round(SliderXPosition.Value).ToString();
        }

        private void SliderYPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxYPosition.Text = Math.Round(SliderYPosition.Value).ToString();
        }

        private void TextBoxXPosition_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxXPosition = CleanLetters(sender);
        }

        private void TextBoxYPosition_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxXPosition = CleanLetters(sender);
        }

        private void SliderFieldWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxFieldWidth.Text = Math.Round(SliderFieldWidth.Value).ToString();
        }

        private void SliderFieldHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxFieldHeight.Text = Math.Round(SliderFieldHeight.Value).ToString();
        }

        private void SliderFormWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxFormWidth.Text = Math.Round(SliderFormWidth.Value).ToString();
            SliderFieldWidth.Maximum = Math.Round(SliderFormWidth.Value);
        }

        private void SliderFormHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBoxFormHeight.Text = Math.Round(SliderFormHeight.Value).ToString();
            SliderFieldHeight.Maximum = Math.Round(SliderFormHeight.Value);
        }

        private void TextBoxWidth_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxFieldWidth = CleanLetters(sender);
        }

        private void TextBoxHeight_KeyUp(object sender, KeyEventArgs e)
        {
            TextBoxFieldHeight = CleanLetters(sender);
        }

        private void SliderFormBackgroundRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBackgroundColorAdjusted();
        }

        private void SliderFormBackgroundGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBackgroundColorAdjusted();
        }

        private void SliderFormBackgroundBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBackgroundColorAdjusted();
        }

        private void FormBackgroundColorAdjusted()
        {
            BrushFormBackground.Color = Color.FromRgb((Byte)SliderFormBackgroundRed.Value, (Byte)SliderFormBackgroundGreen.Value, (Byte)SliderFormBackgroundBlue.Value);
            RectangleFormBackgroundColor.Fill = BrushFormBackground;
        }

        private void FormForegroundColorAdjusted()
        {
            BrushFormForeground.Color = Color.FromRgb((Byte)SliderFormForegroundRed.Value, (Byte)SliderFormForegroundGreen.Value, (Byte)SliderFormForegroundBlue.Value);
            RectangleFormForegroundColor.Fill = BrushFormForeground;
        }

        private void FormBorderColorAdjusted()
        {
            BrushFormBorder.Color = Color.FromRgb((Byte)SliderFormBorderRed.Value, (Byte)SliderFormBorderGreen.Value, (Byte)SliderFormBorderBlue.Value);
            RectangleFormBorderColor.Fill = BrushFormBorder;
        }

        private void SliderFormBorderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBorderColorAdjusted();
        }

        private void SliderFormBorderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBorderColorAdjusted();
        }

        private void SliderFormBorderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormBorderColorAdjusted();
        }

        private void SliderFormForegroundRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormForegroundColorAdjusted();
        }

        private void SliderFormForegroundGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormForegroundColorAdjusted();
        }

        private void SliderFormForegroundBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FormForegroundColorAdjusted();
        }

        private void SliderFieldBackgroundRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBackgroundColorAdjusted();
        }

        private void SliderFieldBackgroundGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBackgroundColorAdjusted();
        }

        private void SliderFieldBackgroundBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBackgroundColorAdjusted();
        }

        private void SliderFieldBorderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBorderColorAdjusted();
        }

        private void SliderFieldBorderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBorderColorAdjusted();
        }

        private void SliderFieldBorderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldBorderColorAdjusted();
        }

        private void SliderFieldForegroundRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldForegroundColorAdjusted();
        }

        private void SliderFieldForegroundGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldForegroundColorAdjusted();
        }

        private void SliderFieldForegroundBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldForegroundColorAdjusted();
        }

        private void FieldBackgroundColorAdjusted()
        {
            textBoxBackgroundRedValue.Text = Math.Round(SliderFieldBackgroundRed.Value).ToString();
            textBoxBackgroundGreenValue.Text = Math.Round(SliderFieldBackgroundGreen.Value).ToString();
            textBoxBackgroundBlueValue.Text = Math.Round(SliderFieldBackgroundBlue.Value).ToString();
            BrushFieldBackground.Color = Color.FromRgb((Byte)SliderFieldBackgroundRed.Value, (Byte)SliderFieldBackgroundGreen.Value, (Byte)SliderFieldBackgroundBlue.Value);
            RectangleFieldBackgroundColor.Fill = BrushFieldBackground;
        }

        private void FieldForegroundColorAdjusted()
        {
            textBoxForegroundRedValue.Text = Math.Round(SliderFieldForegroundRed.Value).ToString();
            textBoxForegroundGreenValue.Text = Math.Round(SliderFieldForegroundGreen.Value).ToString();
            textBoxForegroundBlueValue.Text = Math.Round(SliderFieldForegroundBlue.Value).ToString();
            BrushFieldForeground.Color = Color.FromRgb((Byte)SliderFieldForegroundRed.Value, (Byte)SliderFieldForegroundGreen.Value, (Byte)SliderFieldForegroundBlue.Value);
            RectangleFieldForegroundColor.Fill = BrushFieldForeground;
        }

        private void FieldBorderColorAdjusted()
        {
            textBoxBorderRedValue.Text = Math.Round(SliderFieldBorderRed.Value).ToString();
            textBoxBorderGreenValue.Text = Math.Round(SliderFieldBorderGreen.Value).ToString();
            textBoxBorderBlueValue.Text = Math.Round(SliderFieldBorderBlue.Value).ToString();
            BrushFieldBorder.Color = Color.FromRgb((Byte)SliderFieldBorderRed.Value, (Byte)SliderFieldBorderGreen.Value, (Byte)SliderFieldBorderBlue.Value);
            RectangleFieldBorderColor.Fill = BrushFieldBorder;
        }

        private void FieldErrorColorAdjusted()
        {
            textBoxErrorRedValue.Text = Math.Round(SliderFieldErrorRed.Value).ToString();
            textBoxErrorGreenValue.Text = Math.Round(SliderFieldErrorGreen.Value).ToString();
            textBoxErrorBlueValue.Text = Math.Round(SliderFieldErrorBlue.Value).ToString();

            BrushFieldError.Color = Color.FromRgb((Byte)SliderFieldErrorRed.Value, (Byte)SliderFieldErrorGreen.Value, (Byte)SliderFieldErrorBlue.Value);
            RectangleFieldErrorColor.Fill = BrushFieldError;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            CheckDBType((ListBoxItem)ListBoxDatabaseType.SelectedItem);

            TextBoxFieldHeight.Text = checkFieldEmptyToZero(TextBoxFieldHeight.Text);
            TextBoxFieldWidth.Text = checkFieldEmptyToZero(TextBoxFieldWidth.Text);
            Field.Name = TextBoxName.Text;
            Field.Height = Convert.ToInt32(TextBoxFieldHeight.Text);
            Field.Width = Convert.ToInt32(TextBoxFieldWidth.Text);
            Field.X = Convert.ToInt32(SliderXPosition.Value);
            Field.Y = Convert.ToInt32(SliderYPosition.Value);
            Field.Label = TextBoxLabel.Text;
            Field.Table = TextBoxTableName.Text;
            Field.FieldType = ComboBoxFieldType.SelectedItem.ToString();
            Field.Column = TextBoxColumn.Text;
            Field.KeyName = TextBoxKeyName.Text;
            Field.Key = TextBoxKeyValue.Text;
            Field.KeyTable = TextBoxKeyTableName.Text;
            Field.DBType = new List<string>(_SelectedDBType);
            Field.ForegroundColor = String.Format("{0},{1},{2}", BrushFieldForeground.Color.R.ToString(), BrushFieldForeground.Color.B.ToString(), BrushFieldForeground.Color.G.ToString());
            Field.BorderColor = String.Format("{0},{1},{2}", BrushFieldBorder.Color.R.ToString(), BrushFieldBorder.Color.B.ToString(), BrushFieldBorder.Color.G.ToString());
            Field.BackgroundColor = String.Format("{0},{1},{2}", BrushFieldBackground.Color.R.ToString(), BrushFieldBackground.Color.B.ToString(), BrushFieldBackground.Color.G.ToString());
            Form.Title = TextBoxFormTitle.Text;
            Form.Height = (int)SliderFormHeight.Value;
            Form.Width = (int)SliderFormWidth.Value;
            Form.ForegroundColor = String.Format("{0},{1},{2}", BrushFormForeground.Color.R.ToString(), BrushFormForeground.Color.B.ToString(), BrushFormForeground.Color.G.ToString());
            Form.BorderColor = String.Format("{0},{1},{2}", BrushFormBorder.Color.R.ToString(), BrushFormBorder.Color.B.ToString(), BrushFormBorder.Color.G.ToString());
            Form.BackgroundColor = String.Format("{0},{1},{2}", BrushFormBackground.Color.R.ToString(), BrushFormBackground.Color.B.ToString(), BrushFormBackground.Color.G.ToString());
            Form.Title = Form.Title;
        }

        private void SliderFieldErrorRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldErrorColorAdjusted();
        }

        private void SliderFieldErrorGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldErrorColorAdjusted();
        }

        private void SliderFieldErrorBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FieldErrorColorAdjusted();
        }
    }
}