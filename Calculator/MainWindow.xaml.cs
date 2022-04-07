using Calculator.Structures;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CalcLineEdit.Focus();
            cursor = CalcLineEdit.Text.Length;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private int cursor;

        private void btnNumClick(object sender, RoutedEventArgs e)
        {
            if (CalcLineEdit.Text.Length > 30) return;
            if (!CalcLineEdit.IsFocused) CalcLineEdit.Focus();
            String text = ((Button)sender).Tag.ToString();
            CalcLineEdit.Text += text;
            CalcLineEdit.SelectionStart = CalcLineEdit.Text.Length;

        }

        private void CalcLineEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpperLine.Text = CalcLineEdit.Text;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            CalcLineEdit.Text = "";
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(AppExit));
            thread.Start();
        }

        private void AppExit()
        {
            Thread.Sleep(400);
            Application.Current.Shutdown();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (CalcLineEdit.Text == "") return;
            CalcLineEdit.Text = CalcLineEdit.Text.Remove(CalcLineEdit.Text.Length - 1);
        }

        private bool bSaveFile = true;
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            List<string> RPNexpression = ExpressionCalculator.ParseExpression(CalcLineEdit.Text);
            if (RPNexpression == null)
            {
                CalcLineEdit.Text = "Ошибка!";
                UpperLine.Text = "Ошибка! Скобки не сбалансированны!";
                ExpressionCalculator.error = 0;
                return;
            }
            double result = ExpressionCalculator.Calculate(RPNexpression);
            if (ExpressionCalculator.error == -1)
            {
                CalcLineEdit.Text = "Ошибка!";
                UpperLine.Text = "Ошибка! Не удалось вычислить выражение!";
                ExpressionCalculator.error = 0;
                return;
            }
            SaveLine.Text = CalcLineEdit.Text + " = " + result.ToString();
            if(bSaveFile) if (!SaveToFile(SaveLine.Text)) bSaveFile = false;
            CalcLineEdit.Text = result.ToString();

        }

        private bool SaveToFile(string text)
        {
            string writepath = Directory.GetCurrentDirectory()+"\\CalculatorData.txt";
            try {
                StreamWriter sw = new StreamWriter(writepath, true);
                sw.WriteLine(text+" {"+DateTime.Now.ToShortDateString()+' '+DateTime.Now.ToShortTimeString()+'}');
                sw.Close();
            }
            catch (Exception)
            {
                CalcLineEdit.Text = "Ошибка!";
                UpperLine.Text = "Ошибка! Не удалось сохранить выражение в файл!";
                return false;
            }
            return true;
            
        }

        private void btnMenuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\CalculatorData.txt";
                System.Diagnostics.Process.Start(path);
            } catch (Exception)
            {
                MessageBox.Show("Не удалось открыть файл");
            }
            
        }

        private void btnMenuClearFile_Click(object sender, RoutedEventArgs e)
        {
            string writepath = Directory.GetCurrentDirectory() + "\\CalculatorData.txt";
            try
            {
                StreamWriter sw = new StreamWriter(writepath, false);
                sw.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось очистить файл");
            }
            MessageBox.Show("Файл очищен");
        }

        private void btnMenuExitProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Разработал: Бушуев Максим ВИП-308");
        }

        private void btnToogleTrig_Click(object sender, RoutedEventArgs e)
        {
            if (ExpressionCalculator.tSwitch) btnToogleTrig.Background = (Brush)(new BrushConverter().ConvertFrom("#FF278F09"));
            else btnToogleTrig.Background = (Brush)(new BrushConverter().ConvertFrom("#FF8F0909"));

            ExpressionCalculator.tSwitch = !ExpressionCalculator.tSwitch;
            
        }
    }



    static class ExpressionCalculator
    {
        private static string operators = "+-*/^%"; // операторы
        private static string[] functions = {"sqrt","sin","cos","tan","lg","ln"};//функции
        private static string delimiters = "() +-*/^%";//разделители
        public static int error = 0; //номер ошибки
        public static bool tSwitch = true;//Радианы или градусы

        private static bool isDelimiter(string token)//проверка на разделитель
        {
            if (token.Length != 1) return false;
            for (int i = 0; i < delimiters.Length; i++)
            {
                if (token[0] == delimiters[i]) return true;
            }
            return false;
        }

        private static bool IsOperator(string token)//проверка на оператор
        {
            if (token == "u-") return true;
            for (int i = 0; i < operators.Length; i++)
                if (token[0] == operators[i]) return true;
            return false;
        }

        private static bool IsFunction(string token)//проверка на функцию
        {
            for (int i = 0; i < functions.Length;i++)
                if (token == functions[i]) return true;
            return false;
        }

        private static int GetPriority(string token)//получить приоритет лексемы
        {
            if (token == "(") return 1;
            if (token == "+" || token == "-" || token == "%") return 2;
            if (token == "*" || token == "/") return 3;
            return 4;
        }

        public static List<string> ParseExpression(string expression) //функция перевода выражения в обратную польскую запись
        {
            List<string> RPNexpression = new List<string>();//обратная польская запись
            MyStack<string> stack = new MyStack<string>();
            string[] tokens = Regex.Split(expression, @"([*()\^\/\!\%]|(?<!E)[\+\-])");//делим выражение на лексемы
            string prev = "", current = "";
            foreach(string token in tokens)//перебираем лексемы
            {
                current = token;//текущая лексема
                if (current == " ") continue;
                if (IsFunction(current)) stack.Push(current);
                else if (isDelimiter(current))
                {
                    if (current == "(") stack.Push(current);
                    else if (current == ")")
                    {
                        if (stack.IsNull())
                        {
                            error = -2;
                            return null;
                        }
                        while (stack.Peek() != "(")
                        {
                            RPNexpression.Add(stack.Pop());
                            if (stack.IsNull())
                            {//ошибка
                                error = -2;
                                return null;
                            }
                        }
                        stack.Pop();
                        if (!stack.IsNull() && IsFunction(stack.Peek()))
                        {
                            RPNexpression.Add(stack.Pop());
                        }
                    }
                    else {
                        if (current == "-" && prev == "" || isDelimiter(prev) && prev != ")")
                        {
                            // унарный минус
                            current = "u-";
                        }
                        else {
                            while (!stack.IsNull() && (GetPriority(current) <= GetPriority(stack.Peek())))
                            {//кладем текущую лексему в стек в соответствии с приоритетом
                                RPNexpression.Add(stack.Pop());
                            }

                        }
                        stack.Push(current);
                    }

                }
                else {
                    RPNexpression.Add(current);
                }
                prev = current;//сохраняем предыдущую лексему для унарного мин4са
            }

            while (!stack.IsNull())
            {//перекладываем содержимое стека
                if (IsOperator(stack.Peek())) RPNexpression.Add(stack.Pop());
                else {//ошибка
                    error = -2;
                    return null;
                }
            }
            return RPNexpression;
        }

        public static double Calculate(List<string> RPNexpression)
        {
            MyStack<double> stack = new MyStack<double>();
            double a, b;
            foreach (string x in RPNexpression)
            {
                switch (x)
                {
                    case "!": stack.Push(Factorial((int)stack.Pop())); break;
                    case "e": stack.Push(Math.E); break;
                    case "π": stack.Push(Math.PI); break;
                    case "sqrt": stack.Push(Math.Sqrt(stack.Pop())); break;
                    case "sin": {a = stack.Pop(); if (tSwitch) stack.Push(Math.Sin(a * Math.PI / 180)); else stack.Push(Math.Sin(a)); break; }
                    case "cos": {a = stack.Pop();if (tSwitch) stack.Push(Math.Cos(a*Math.PI/180)); else stack.Push(Math.Cos(a)); break; }
                    case "tan": { a = stack.Pop();if (tSwitch) stack.Push(Math.Tan(a*Math.PI/180)); else stack.Push(Math.Tan(a)); break; }
                    case "ln": stack.Push(Math.Log(stack.Pop())); break;
                    case "lg": stack.Push(Math.Log10(stack.Pop())); break;
                    case "+": stack.Push(stack.Pop() + stack.Pop()); break;
                    case "-": {a=stack.Pop();b = stack.Pop();stack.Push(b - a); break; }
                    case "*": stack.Push(stack.Pop() * stack.Pop()); break;
                    case "/": { a = stack.Pop(); b = stack.Pop(); stack.Push(b / a); break; }
                    case "^": { a = stack.Pop(); b = stack.Pop(); stack.Push(Math.Pow(b, a)); break; }
                    case "%": { a = stack.Pop(); b = stack.Pop(); stack.Push(b%a); break; }
                    case "u-": stack.Push(-stack.Pop());break;
                    case "": break;
                    default: {
                            try {
                                stack.Push(Convert.ToDouble(x));
                            } catch (FormatException){
                                error = -1;
                            }
                            break;
                        }

                }
            }
            return stack.Pop();
        }
        public static int Factorial(int n)
        {
            int factorial = 1;

            for (int i = 2; i <= n; i++)
            {
                factorial = factorial * i;
            }

            return factorial;
        }
    }

    
}
