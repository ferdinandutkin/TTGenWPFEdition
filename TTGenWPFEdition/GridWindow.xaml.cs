
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TTGenWPFEdition
{
    /// <summary>
    /// Логика взаимодействия для GridWindow.xaml
    /// </summary>
    public partial class GridWindow : Window
    {
        public GridWindow(string expression)
        {
            InitializeComponent();

            Title = expression;

            TruthTableGen gen = new TruthTableGen(expression);
            var res = gen.Generate();
           
            var columnNames = res[0];
            res.RemoveAt(0);

            foreach (string name in columnNames)
            {
                DataGridTextColumn column = new DataGridTextColumn() 
                {
                    
                    Header = name,
                    Binding = new Binding(name.Replace(' ', '_').Replace('(', '_').Replace(')', '_')) //какой же бред АХАХАХХАХАХАХАХАХАХА
                };
        
        
                Table.Columns.Add(column);
            }



            foreach (var resRow in res)
            {

                dynamic row = new ExpandoObject();

                for (int i = 0; i < columnNames.Count; i++)
                    ((IDictionary<string, object>)row)[columnNames[i].Replace(' ', '_').Replace('(', '_').Replace(')', '_')] = resRow[i];

                Table.Items.Add(row);

            }
        }
    }
}
