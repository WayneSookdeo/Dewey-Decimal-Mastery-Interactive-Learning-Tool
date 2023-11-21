using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace LibraryShelves
{
    public partial class Part3 : Window
    {

        //https://www.simplilearn.com/tutorials/c-sharp-tutorial/tree-in-c-sharp
        //the above link was used to gain knowlege on the use of trees in C#
        private DeweyDecimalNode root;
        private DeweyDecimalNode currentQuestion;
        private Random random;
        private int currentStreak = 0;
        public Part3()
        {
            InitializeComponent();
            root = ReadDeweyDecimalData("C:\\Users\\lab_services_student\\source\\repos\\LibraryShelves\\LibraryShelves\\Reformatted_Data.txt");
            random = new Random();
            DisplayNextQuestion();
        }

        private void DisplayNextQuestion()
        {
            var thirdLevelNodes = GetAllThirdLevelNodes(root);
            currentQuestion = thirdLevelNodes[random.Next(thirdLevelNodes.Count)];

            QuestionLabel.Content = currentQuestion.Description;
            var topOptions = GetTopLevelOptions(currentQuestion);

            OptionsListBox.ItemsSource = topOptions;
        }

        private List<DeweyDecimalNode> GetAllThirdLevelNodes(DeweyDecimalNode node)
        {
            var nodes = new List<DeweyDecimalNode>();
            if (node.Children.Count == 0)
            {
                nodes.Add(node);
            }
            else
            {
                foreach (var child in node.Children)
                {
                    nodes.AddRange(GetAllThirdLevelNodes(child));
                }
            }
            return nodes;
        }

        private List<string> GetTopLevelOptions(DeweyDecimalNode correctNode)
        {
            var topLevelCode = correctNode.Code / 100 * 100;
            var correctTopLevelNode = root.Children.FirstOrDefault(c => c.Code == topLevelCode);

            if (correctTopLevelNode == null)
            {
                // Handle the case where no matching top-level node is found
                return new List<string>();
            }

            var options = new List<DeweyDecimalNode> { correctTopLevelNode };
            var incorrectOptions = root.Children.Where(c => c.Code != topLevelCode).ToList();

            // Randomly add three incorrect options
            while (options.Count < 4)
            {
                var randomOption = incorrectOptions[random.Next(incorrectOptions.Count)];
                if (!options.Contains(randomOption))
                {
                    options.Add(randomOption);
                }
            }

            // Order the options numerically by their code
            options = options.OrderBy(o => o.Code).ToList();

            return options.Select(o => $"{o.Code} {o.Description}").ToList();
        }


        private DeweyDecimalNode ReadDeweyDecimalData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var root = new DeweyDecimalNode(0, "Root");
            var currentNodes = new Stack<DeweyDecimalNode>();
            currentNodes.Push(root);

            foreach (var line in lines)
            {
                var level = line.TakeWhile(char.IsWhiteSpace).Count() / 2; // Assuming 2 spaces per indent
                var content = line.Trim().Split(new[] { ' ' }, 2);
                var code = int.Parse(content[0]);
                var description = content[1];

                var newNode = new DeweyDecimalNode(code, description);

                while (currentNodes.Count > level + 1)
                    currentNodes.Pop();

                currentNodes.Peek().Children.Add(newNode);
                currentNodes.Push(newNode);
            }

            return root;
        }


        private void OptionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && currentQuestion != null)
            {
                var selectedOption = e.AddedItems[0] as string;
                var selectedCode = int.Parse(selectedOption.Split(' ')[0]);
                var currentQuestionTopLevelCode = currentQuestion.Code / 100 * 100;

                if (selectedCode == currentQuestionTopLevelCode)
                {
                    currentStreak++;
                    MessageBox.Show($"Correct!");
                    UpdateStreakDisplay();
                    DisplayNextQuestion();
                }
                else
                {
                    currentStreak = 0; // Reset streak on wrong answer
                    MessageBox.Show("Incorrect. Try again.");
                }
            }
        }
        private void UpdateStreakDisplay()
        {
            StreakTextBlock.Text = $"Streak: {currentStreak}";
        }



    }
    public class DeweyDecimalNode
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public List<DeweyDecimalNode> Children { get; set; }

        public DeweyDecimalNode(int code, string description)
        {
            Code = code;
            Description = description;
            Children = new List<DeweyDecimalNode>();
        }
    }
}
