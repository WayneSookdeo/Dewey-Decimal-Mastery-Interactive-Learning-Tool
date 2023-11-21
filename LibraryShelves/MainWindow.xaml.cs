using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace LibraryShelves
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<string> _callNumbers;
        public List<string> CallNumbers
        {
            get { return _callNumbers; }
            set
            {
                _callNumbers = value;
                OnPropertyChanged("CallNumbers");
            }
        }

        private List<string> _userInputOrder; // Added to track user input order
        private List<string> _sortedCallNumbers; // Added to store the sorted call numbers
        private int _userScore; // Added to track the user's score

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _userInputOrder = new List<string>();
            _sortedCallNumbers = new List<string>();
            _userScore = 0; // Initialize the user's score
        }

        private void generateCallNumbersButton_Click(object sender, RoutedEventArgs e)
        {
            CallNumbers = new List<string>();
            _userInputOrder.Clear(); // Clear user input order when regenerating numbers
            _sortedCallNumbers.Clear(); // Clear the sorted call numbers
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                int topic = random.Next(1000);
                string author = new string(Enumerable.Range(0, 3).Select(x => (char)random.Next('A', 'Z' + 1)).ToArray());
                string callNumber = $"{topic:D3}.{random.Next(100):D2} {author}";
                CallNumbers.Add(callNumber);
            }

            // Sort and store the call numbers
            _sortedCallNumbers = BubbleSort(CallNumbers.ToList());

            checkOrderButton.IsEnabled = true;
        }

        private void callNumbersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Capture the user's input order as they click the call numbers
            List<string> selectedItems = new List<string>();
            foreach (var item in callNumbersListBox.SelectedItems)
            {
                selectedItems.Add(item.ToString());
            }
            _userInputOrder = selectedItems.ToList();
        }

        private void checkOrderButton_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrect = _sortedCallNumbers.SequenceEqual(_userInputOrder); // Compare with user input order

            // Update the user's score based on their performance
            if (isCorrect)
            {
                _userScore += 10; // Award 10 points for correct sorting
            }
            else
            {
                _userScore -= 5; // Deduct 5 points for incorrect sorting
            }

            // Display the user's score
            resultLabel.Content = isCorrect ? "Correct order! Score: " + _userScore : "Incorrect order. Score: " + _userScore;
        }

        private List<string> BubbleSort(List<string> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (string.Compare(list[j], list[j + 1]) > 0)
                    {
                        // Swap list[j] and list[j+1]
                        string temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void identifyAreasRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IdentifyingAreas IA = new IdentifyingAreas();
            IA.Show();
        }

        private void findCallNumbersRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Part3 p3 = new Part3();
            p3.Show();
        }
    }
}
