using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TTGenWPFEdition
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        string expressionValue;
        public string Expression
        {
            get => expressionValue;
            set
            {
                Dictionary<char, string> commands = new Dictionary<char, string> {
                    {'+', " ∨ "}, {'*', " ∧ "}, {'&', " ∧ "},
                    { '!', " ¬ "}, { '>', " ⇒ "}, {'^', " ⊕ " }, { '=', " ⇔ "},
                    { '1', " True "}, {'0', " False "}
                };


                expressionValue = string.Concat(value.Select(ch =>
                {

                    if (commands.TryGetValue(ch, out var res))
                    {
                        return res;
                    }
                    if (char.IsLetter(ch) || char.IsWhiteSpace(ch) || "()∨∧*¬⇒⊕⇔".Contains(ch))
                    {
                        return ch.ToString();
                    }
                    else return "";
                }
                ));
            }

        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Expression) || Expression.All(ch => char.IsWhiteSpace(ch)))
            {
                return;
            }
            GridWindow window = new GridWindow(Expression);
            window.Show();
            Input.Focus();

        }

        private void OpButton_Click(object sender, RoutedEventArgs e)
        {


            Input.SelectedText = ' ' + (sender as Button).Content.ToString() + ' ';
            Input.CaretIndex += Input.SelectedText.Length;
            Input.SelectionLength = 0;
            Input.Focus();

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
            => Input.Clear();
       
    }


}