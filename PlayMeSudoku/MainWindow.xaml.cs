using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudoKu
{
    public partial class MainWindow : Window
    {
        private const int SUDOKU_SIZE = 9;
        private bool isSolved;
        TextBox[,] TextBoxMatrix = new TextBox[SUDOKU_SIZE, SUDOKU_SIZE];
        string[,] SymbolMatrix = new string[SUDOKU_SIZE, SUDOKU_SIZE];
        Random randomGenerator = new Random();
        private int nthSolution = 0;

        private void ClearBoard()
        {
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    SymbolMatrix[row, col] = " ";
                    TextBoxMatrix[row, col].Dispatcher.Invoke(
                        new Action(
                            () => { TextBoxMatrix[row, col].Text = " "; }
                        )
                    );
                    System.Threading.Thread.Sleep(5);
                }
            }
            this.Dispatcher.Invoke(
                new Action(SwitchButtonsState)
            ); 
            System.Threading.Thread T = new System.Threading.Thread(IsCorrect);
            T.Start();
        }

        private bool CheckBox(int rowEnd, int colEnd, List<string> container)
        {
            for (int row = rowEnd; row < rowEnd + 3; row++)
            {
                for (int col = colEnd; col < colEnd + 3; col++)
                {
                    container.Remove(SymbolMatrix[row, col]);
                }
            }
            bool result = (container.Count == 0);
            return result;
        }

        private bool CheckRow(int row, List<string> container)
        {
            for (int col = 0; col < SUDOKU_SIZE; col++)
            {
                container.Remove(SymbolMatrix[row, col]);
            }
            bool result = (container.Count == 0);
            return result;
        }

        private bool CheckCol(int col, List<string> container)
        {
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                container.Remove(SymbolMatrix[row, col]);
            }
            bool result = (container.Count == 0);
            return result;
        }

        private List<string> PossibleNumbers(int row, int col)
        {
            List<string> possibleNumbers = new List<string>();
            for (int i = 1; i < SUDOKU_SIZE + 1; i++)
            {
                possibleNumbers.Add(i.ToString());
            }
            CheckBox((row / 3) * 3, (col / 3) * 3, possibleNumbers);
            CheckRow(row, possibleNumbers);
            CheckCol(col, possibleNumbers);
            return possibleNumbers;            
        }

        private bool NextBox(Box box)
        {
            int min = SUDOKU_SIZE + 1;
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    if (SymbolMatrix[row, col] == " ")
                    {
                        List<string> possibleNumbers = PossibleNumbers(row, col);
                        if (possibleNumbers.Count < min)
                        {
                            min = possibleNumbers.Count;
                            box.Row = row;
                            box.Col = col;
                            box.PossibleNumbers = possibleNumbers;
                        }
                    }
                    if (min <= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsEmpty()
        {
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    if (SymbolMatrix[row, col] == " ")
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void SubmitBox(int row, int col, string number, bool display = true)
        {
            if (isSolved == false) 
            {
                SymbolMatrix[row, col] = number;
                if (display == true)
                {
                    TextBoxMatrix[row, col].Dispatcher.Invoke(
                        new Action(
                            () => { TextBoxMatrix[row, col].Text = number.ToString(); }
                        )
                    );
                    System.Threading.Thread.Sleep(SUDOKU_SIZE + 1);
                }
                Box next_point = new Box();
                if (NextBox(next_point))
                {
                    int numbers_size = next_point.PossibleNumbers.Count;
                    for (int numb = 0; numb < numbers_size; numb++)
                    {
                        SubmitBox(next_point.Row, next_point.Col, next_point.PossibleNumbers[numb], display);
                    }
                }
                if (IsEmpty())
                {
                    if (nthSolution == 0)
                    {
                        isSolved = true;
                        return;
                    }
                    else
                    {
                        --nthSolution;
                    }
                }
                SymbolMatrix[row, col] = " ";
                if (display == true)
                {
                    TextBoxMatrix[row, col].Dispatcher.Invoke(
                        new Action(
                            () => { TextBoxMatrix[row, col].Text = SymbolMatrix[row, col].ToString(); }
                        )
                    );
                }
            }
        }

        private void Solve()
        {
            if (isSolved == false)
            {
                Box next_point = new Box();
                if (NextBox(next_point))
                {
                    int numbers_size = next_point.PossibleNumbers.Count;
                    for (int i = 0; i < numbers_size; i++)
                    {
                        SubmitBox(next_point.Row, next_point.Col, next_point.PossibleNumbers[i]);
                    }
                }
                System.Threading.Thread T = new System.Threading.Thread(IsCorrect);
                T.Start();
            }
            this.Dispatcher.Invoke(new Action(SwitchButtonsState));
        }

        private void Generate()
        {
            Box startPoint = new Box();
            NextBox(startPoint);
            int startDigit = randomGenerator.Next(startPoint.PossibleNumbers.Count);
            SubmitBox(startPoint.Row, startPoint.Col, startPoint.PossibleNumbers[startDigit], false);
            int num = 17 + randomGenerator.Next(15);
            isSolved = false;
            for (int i = 0; i < num; i++)
            {
                int row, col;
                do
                {
                    row = randomGenerator.Next(SUDOKU_SIZE);
                    col = randomGenerator.Next(SUDOKU_SIZE);
                }
                while (SymbolMatrix[row, col] == " ");
                TextBoxMatrix[row, col].Dispatcher.Invoke(
                    new Action(
                        () => { TextBoxMatrix[row, col].Text = SymbolMatrix[row, col].ToString(); }
                    )
                );
                SymbolMatrix[row, col] = " ";
                System.Threading.Thread.Sleep(SUDOKU_SIZE + 1);
            }
            this.Dispatcher.Invoke(new Action(SwitchButtonsState));
            System.Threading.Thread T = new System.Threading.Thread(IsCorrect);
            T.Start();
        }

        private void UpdateMatrix()
        {
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    SymbolMatrix[row, col] = " ";
                    this.Dispatcher.Invoke(new Action(
                        () =>
                        {
                            if (TextBoxMatrix[row, col].Text.Length > 0)
                            {
                                SymbolMatrix[row, col] = TextBoxMatrix[row, col].Text;
                            }
                        }
                    ));
                }
            }
            IsCorrect();
        }

        private void ValidateTextBoxInput(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            bool isDigit = e.Key >= Key.D1 && e.Key <= Key.D9;
            if (textBox.Text.Length > 0)
            {
                textBox.Text = "";
            }
            if (isDigit == true)
            {
                textBox.Text = e.Key.ToString().Replace("D", "");
            }
            e.Handled = true;
            System.Threading.Thread T = new System.Threading.Thread(UpdateMatrix);
            T.Start();
        }

        private void SwitchButtonsState()
        {
            this.buttonSolve.IsEnabled = !this.buttonSolve.IsEnabled;
            this.buttonGenerate.IsEnabled = !this.buttonGenerate.IsEnabled;
            this.buttonClear.IsEnabled = !this.buttonClear.IsEnabled;
        }

        private void InitTextBox(int row, int col)
        {
            TextBoxMatrix[row, col] = new TextBox();
            TextBoxMatrix[row, col].KeyDown +=
                new KeyEventHandler(ValidateTextBoxInput);
            if (((row / 3) * 3) % 2 == ((col / 3) * 3) % 2)
            {
                TextBoxMatrix[row, col].Background = Brushes.WhiteSmoke;
            }
            TextBoxMatrix[row, col].FontSize = 18;
            TextBoxMatrix[row, col].TextAlignment = TextAlignment.Center;
            TextBoxMatrix[row, col].Width = 30;
            TextBoxMatrix[row, col].Height = 30;
            TextBoxMatrix[row, col].Text = " ";
            TextBoxMatrix[row, col].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            TextBoxMatrix[row, col].VerticalAlignment = System.Windows.VerticalAlignment.Top;
            TextBoxMatrix[row, col].Margin = new Thickness(col * 32, row * 32, 0, 0);
            Inside.Children.Add(TextBoxMatrix[row, col]);    
        }

        public MainWindow()
        {
            InitializeComponent();
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    InitTextBox(row, col);
                }
            }
        }

        private void buttonSolve_Click(object sender, RoutedEventArgs e)
        {
            SwitchButtonsState();
            UpdateMatrix();
            System.Threading.Thread T = new System.Threading.Thread(Solve);
            T.Start();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            SwitchButtonsState();
            System.Threading.Thread T = new System.Threading.Thread(ClearBoard);
            T.Start();
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            SwitchButtonsState();
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                for (int col = 0; col < SUDOKU_SIZE; col++)
                {
                    SymbolMatrix[row, col] = " ";
                    TextBoxMatrix[row, col].Dispatcher.Invoke(new Action(() => { TextBoxMatrix[row, col].Text = " "; }));
                }
            }
            isSolved = false;
            nthSolution = randomGenerator.Next(3);
            System.Threading.Thread T = new System.Threading.Thread(Generate);
            T.Start();
        }

        private void IsCorrect()
        {
            isSolved = CheckBoard();
            string state = "Incorrect";
            SolidColorBrush textColor = Brushes.IndianRed;
            if (isSolved)
            {
                state = "Correct";
                textColor = Brushes.Green;
            }
            this.Dispatcher.Invoke(
                new Action(
                    () =>
                    {
                        this.textBox1.Foreground = textColor;
                        this.textBox1.Text = state;
                    }
                )
            );
        }

        private List<string> GenerateFullDigitList()
        {
            List<string> elements = new List<string>();
            for (int i = 1; i < SUDOKU_SIZE + 1; i++)
            {
                elements.Add(i.ToString());
            }
            return elements;
        }

        private bool CheckBoard()
        {
            for (int row = 0; row < SUDOKU_SIZE; row++)
            {
                if (CheckRow(row, GenerateFullDigitList()) == false)
                {
                    return false;
                }
            }
            for (int col = 0; col < SUDOKU_SIZE; col++)
            {
                if (CheckCol(col, GenerateFullDigitList()) == false)
                {
                    return false;
                }
            }
            for (int row = 0; row < SUDOKU_SIZE; row+=3)
            {
                for (int col = 0; col < SUDOKU_SIZE; col+=3)
                {
                    if (CheckBox(row, col, GenerateFullDigitList()) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
