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
using System.Windows.Shapes;

namespace LibraryShelves
{
    /// <summary>
    /// Interaction logic for IdentifyingAreas.xaml
    /// </summary>
    public partial class IdentifyingAreas : Window
    {
        private Dictionary<int, string> deweyCategories;
        private int score = 0;
        private List<DeweyCategory> currentQuestions;
        private int level = 1;
        private const int POINTS_PER_CORRECT = 20;
        private const int POINTS_PER_LEVEL = 100;

        public IdentifyingAreas()
        {
            InitializeComponent();
            InitializeCategories();
            GenerateQuestions();
        }

        private void InitializeCategories()
        {
            deweyCategories = new Dictionary<int, string>()
            {
                {000, "General Knowledge"},
                {100, "Philosophy and psychology"},
                {200, "Religion"},
                 {300, "Social Sciences"},
                  {400, "Languages"},
                   {500, "Science"},
                    {600, "Technology"},
                  {700, "Arts & Recreation"},
                   {800, "Literature"},
                    {900, "History & Geography"},

            };
        }

        private void GenerateQuestions()
        {
            currentQuestions = deweyCategories.OrderBy(x => Guid.NewGuid()).Take(4).Select(kvp => new DeweyCategory { CallNumber = kvp.Key, Description = kvp.Value }).ToList();

            lstCallNumbers.ItemsSource = currentQuestions.Select(c => c.CallNumber).ToList();

            var descriptions = currentQuestions.Select(c => c.Description).ToList();

            // Add 3 random incorrect ones
            while (descriptions.Count < 7)
            {
                var randomDescription = deweyCategories.Values.ElementAt(new Random().Next(deweyCategories.Count));
                if (!descriptions.Contains(randomDescription))
                {
                    descriptions.Add(randomDescription);
                }
            }

            lstDescriptions.ItemsSource = descriptions.OrderBy(d => Guid.NewGuid()).ToList();
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            int correctCount = 0;

            // Check if matching is correct
            for (int i = 0; i < lstCallNumbers.SelectedItems.Count; i++)
            {
                var selectedCallNumber = (int)lstCallNumbers.SelectedItems[i];
                var correspondingDescription = currentQuestions.First(q => q.CallNumber == selectedCallNumber).Description;

                if (lstDescriptions.SelectedItems.Contains(correspondingDescription))
                {
                    correctCount++;
                }
            }

            score += correctCount * POINTS_PER_CORRECT;
            if (score >= level * POINTS_PER_LEVEL)
            {
                level++;
                txtLevel.Text = "Level: " + level;
                score = score - (level - 1) * POINTS_PER_LEVEL; // Reset score for next level
                progressLevel.Value = 0;
            }
            else
            {
                progressLevel.Value = (double)score / POINTS_PER_LEVEL * 100; // Percentage of progress to next level
            }

            txtScore.Text = "Score: " + score;
        }

        private void OnNext(object sender, RoutedEventArgs e)
        {
            GenerateQuestions();
        }
        private void OnRestart(object sender, RoutedEventArgs e)
        {
            // Reset score, level, and progress bar
            score = 0;
            level = 1;
            progressLevel.Value = 0;

            // Update UI elements
            txtScore.Text = "Score: 0";
            txtLevel.Text = "Level: 1";

            // Generate new questions
            GenerateQuestions();
        }

    }
}
