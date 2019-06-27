using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Operation LastOperation = Operation.None;
        string DisplayString { get; set; }
        string LastValue { get; set; }
        string TempValue { get; set; }
        double Memory { get; set; } = 0.0;
        bool isCalculate = false;
        bool isClearString = false;

        enum Operation
        {
            None,
            Devide,
            Multiply,
            Minus,
            Plus,
            Percent,
            Sqrt,
            OneX
        }

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplayString();
        }

        // Обновление текстовой строки
        private void UpdateDisplayString()
        {
            if (String.IsNullOrEmpty(DisplayString))
                txBlock.Text = "0";
            else
                txBlock.Text = DisplayString;
        }

        // Нажатие цифр
        private void ButtonDg_Click(object sender, RoutedEventArgs e)
        {
            AddDigit((sender as Button).Content.ToString());
            UpdateDisplayString();
        }

        // Добавить цифру
        private void AddDigit(string digit)
        {
            // Если выбрана операция запомнить первый операнд
            if (isClearString)
            {
                isCalculate = true;
                isClearString = false;
                LastValue = DisplayString;
                DisplayString = "";
            }
            // Ввво цифр
            if (digit == ".")
            {
                if (String.IsNullOrEmpty(DisplayString))
                    DisplayString += "0,";
                else if (DisplayString.IndexOf(",") > 0)
                     return;
                else
                    DisplayString += ",";
            }
            else
                DisplayString += digit;
        }

        // Нажатие оперирующих клавиш
        private void ButtonOp_Click(object sender, RoutedEventArgs e)
        {
            switch((sender as Button).Name)
            {
                case "btBack":                                                      // "<--"
                    if (!String.IsNullOrEmpty(DisplayString))
                    {
                        DisplayString = DisplayString.Remove(DisplayString.Length - 1);
                        if (DisplayString == "-")
                            DisplayString = "";
                    }
                    UpdateDisplayString();
                    break;
                case "btC":                                                         // "C"
                    AllCleare();
                    break;
                case "btCE":                                                        // "CE"
                    LastOperation = Operation.None;
                    DisplayString = LastValue;
                    UpdateDisplayString();
                    break;
                case "btPM":                                                        // "+/-"
                    if (String.IsNullOrEmpty(DisplayString))
                        return;
                    if (DisplayString[0] == '-')
                        DisplayString = DisplayString.Replace("-", "");
                    else
                        DisplayString = DisplayString.Insert(0, "-");
                    UpdateDisplayString();
                    break;
                case "btSqrt":                                                      // "Sqrt"
                    LastOperation = Operation.Sqrt;
                    LastValue = DisplayString;
                    Calculate();
                    LastValue = DisplayString;
                    LastOperation = Operation.None;
                    isCalculate = false;
                    isClearString = true;
                    break;
                case "btPercent":                                                   // "%"
                    if (isCalculate)
                    {
                        Calculate();
                        LastOperation = Operation.Percent;
                        break;
                    }
                    LastOperation = Operation.Percent;
                    break;
                case "btOneOver":                                                   // "1/x"
                    LastOperation = Operation.OneX;
                    LastValue = DisplayString;
                    Calculate();
                    LastValue = DisplayString;
                    LastOperation = Operation.None;
                    isCalculate = false;
                    isClearString = true;
                    break;
                case "btDevide":                                                    // "/"
                    isClearString = true;
                    if (isCalculate)
                    {
                        Calculate();
                        LastOperation = Operation.Devide;
                        break;
                    }
                    LastOperation = Operation.Devide;
                    TempValue = "";
                    break;
                case "btMultiply":                                                  // ""*
                    isClearString = true;
                    if (isCalculate)
                    {
                        Calculate();
                        LastOperation = Operation.Multiply;
                        break;
                    }
                    LastOperation = Operation.Multiply;
                    TempValue = "";
                    break;
                case "btMinus":                                                     // "-"
                    isClearString = true;
                    if (isCalculate)
                    {
                        Calculate();
                        LastOperation = Operation.Minus;
                        break;
                    }
                    LastOperation = Operation.Minus;
                    TempValue = "";
                    break;
                case "btPlus":                                                      // "+"
                    isClearString = true;
                    if (isCalculate)
                    {
                        Calculate();
                        LastOperation = Operation.Plus;
                        break;
                    }
                    LastOperation = Operation.Plus;
                    TempValue = "";
                    break;
                case "btEqual":                                                     // "="
                    isClearString = true;
                    if (isCalculate)
                    {
                        TempValue = DisplayString;
                        Calculate();                      
                        AddDigit(DisplayString);
                        isCalculate = false;
                        isClearString = false;
                        DisplayString = "";
                        break;
                    }
                    else if(LastOperation != Operation.None)
                    {
                        if (!String.IsNullOrEmpty(DisplayString))
                        {
                            if (String.IsNullOrEmpty(TempValue))
                                TempValue = DisplayString;
                            AddDigit(DisplayString);
                        }
                        DisplayString = TempValue;
                        Calculate();
                        break;
                    }
                    break;                  
                case "btMemClear":                                                  // "MC"
                    Memory = 0.0;
                    break;
                case "btMemRead":                                                   // "MR"
                    DisplayString = Memory.ToString();
                    UpdateDisplayString();
                    break;
                case "btMemSave":                                                   // "MS"
                    if (ValidateString(DisplayString))
                    {
                        Memory = StringToDigit(DisplayString);
                        DisplayString = "";
                    }
                    break;
                case "btMemPlus":                                                    // "M+"
                    if(ValidateString(DisplayString))
                        Memory += StringToDigit(DisplayString);
                    break;
                case "btMemMinus":                                                  // "M-"
                    if (ValidateString(DisplayString))
                        Memory -= StringToDigit(DisplayString);
                    break;
            }
        }

        // Проверка строки
        private bool ValidateString(string str)
        {
            try
            {
                CheckDigit(StringToDigit(DisplayString));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        // Выполнение операции
        private void Calculate()
        {
            double num1, num2;
            try
            {
                num1 = StringToDigit(LastValue);
                num2 = StringToDigit(DisplayString);
                switch (LastOperation)
                {
                    case Operation.Devide:
                        Calc(num1 / num2);
                        break;
                    case Operation.Multiply:
                        Calc(num1 * num2);
                        break;
                    case Operation.Minus:
                        Calc(num1 - num2);
                        break;
                    case Operation.Plus:
                        Calc(num1 + num2);
                        break;
                    case Operation.Percent:
                        Calc((num1 * num2) / 100.0f);
                        break;
                    case Operation.Sqrt:
                        Calc(Math.Sqrt(num1));
                        break;
                    case Operation.OneX:
                        Calc(1.0f / num1);
                        break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AllCleare();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                AllCleare();
            }
            finally
            {

            }
        }
        private void Calc(double result)
        {
            CheckDigit(result);
            DisplayString = result.ToString();
            //LastValue = DisplayString;
            UpdateDisplayString();
            isCalculate = false;
            //isCalculate = false;
        }

        // Преобразование строки в число
        private double StringToDigit(string str)
        {
            double digit;
            if (!Double.TryParse(str, out digit))
                throw new Exception("Недопустимое значение: " + str);
            return digit;
        }

        // Проверка результата
        private void CheckDigit(double result)
        {
            if (Double.IsNegativeInfinity(result) || Double.IsPositiveInfinity(result) || Double.IsNaN(result))
                throw new ArgumentOutOfRangeException(result.ToString());
        }

        // Обнуление калькулятора
        private void AllCleare()
        {
            isCalculate = false;
            LastOperation = Operation.None;
            LastValue = "";
            DisplayString = "";
            UpdateDisplayString();
        }
    }
}