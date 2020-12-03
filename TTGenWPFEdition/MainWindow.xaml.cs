using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Dynamic;

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
            if (Expression.All(ch => char.IsWhiteSpace(ch)))
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

        }
    }


}