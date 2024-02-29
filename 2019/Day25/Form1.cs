using System.Collections.Concurrent;

namespace Day25
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartComputer();
        }

        private Computer? Computer;
        private BlockingCollection<long> Writer = [];
        private BlockingCollection<long> Reader = [];
        private CancellationTokenSource CancellationTokenSource = new();
        private CancellationToken CancellationToken;

        private void StartComputer()
        {
            CancellationToken = CancellationTokenSource.Token;

            string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day25\Full.txt";
            Computer = new Computer(ReadProgram(fileName));

            Computer.Input += (c) => Writer.Take();
            Computer.Output += (c, value) => Reader.Add(value);

            Task.Factory.StartNew(() => 
            {
                Computer.Run();
                CancellationTokenSource.Cancel();
            }, TaskCreationOptions.LongRunning);
            ReadAnswer(true);
        }

        private void Output(string text)
        {
            textBoxOuput.AppendText(Environment.NewLine + text);
        }

        private string ExecuteCommand(string command, bool visibleAnswer)
        {
            foreach (var ch in command)
            {
                Writer.Add(ch);
            }

            Writer.Add(10);

            return ReadAnswer(visibleAnswer);
        }
        private string ExecuteCommandAutomated(string command)
        {
            return ExecuteCommand(command, false);
        }


        private string ReadAnswer(bool visible)
        {
            string result = "";

            try
            {

                char ch;
                do
                {
                    ch = (char)Reader.Take(CancellationToken);
                    string text = (ch == 10 ? Environment.NewLine : "" + ch);
                    result += text;
                }
                while (!CancellationToken.IsCancellationRequested && !result.EndsWith("Command?"));
            }
            catch(OperationCanceledException)
            {
                // Is normal if compter has finished
            }

            if (visible)
            {
                Output(result);
            }

            return result;   
        }

        static long[] ReadProgram(string fileName)
        {
            return File.ReadAllText(fileName).Split(',').Select(long.Parse).ToArray();
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ExecuteCommand(textBoxInput.Text, true);
                textBoxInput.Clear();
                e.Handled = true;
                e.SuppressKeyPress = true;
                buttonAutomate.Enabled = false;
            }
        }

        private void buttonAutomate_Click(object sender, EventArgs e)
        {
            ExecuteCommandAutomated("east");
            ExecuteCommandAutomated("take klein bottle");
            ExecuteCommandAutomated("east");
            ExecuteCommandAutomated("take semiconductor");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("take dehydrated water");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("take sand");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("take astrolabe");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("take mutex");
            ExecuteCommandAutomated("east");
            ExecuteCommandAutomated("east");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("north");
            ExecuteCommandAutomated("take shell");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("south");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("take ornament");
            ExecuteCommandAutomated("west");
            ExecuteCommandAutomated("south");


            Output("Arrived at Security Checkpoint. Trying combinations of items...");

            TryCombinations();

        }

        private void TryCombinations()
        {
            string[] items = ["shell", "mutex", "ornament", "astrolabe", "sand", "semiconductor", "dehydrated water", "klein bottle"];

            int itemCountToTest = 1;

            while (itemCountToTest <= items.Length)
            {
                List<string[]> combinations = GetCombinations(items.ToHashSet(), itemCountToTest);

                foreach(var combination in combinations)
                {
                    DropAllItems(items);
                    TakeItems(combination);
                    string result = ExecuteCommandAutomated("south");

                    if (!result.Contains("Alert"))
                    {
                        Output("Correct combination found: " + string.Join(",", combination));
                        Output(result);
                        return;
                    }
                }

                itemCountToTest++;
            }

            Output("No valid combinations found!");
        }

        private List<string[]> GetCombinationsRecursive(string[] soFar, HashSet<string> remainingItems, int itemCount)
        {
            if (soFar.Length == itemCount) return [soFar];

            List<string[]> result = [];

            foreach (string remainingItem in remainingItems)
            {
                result.AddRange(GetCombinationsRecursive([.. soFar, remainingItem], remainingItems.Except([remainingItem]).ToHashSet(), itemCount));
            }

            return result;
        }

        private List<string[]> GetCombinations(HashSet<string> items, int itemCount)
        {
            List<string[]> result = [];

            result.AddRange(GetCombinationsRecursive([], items, itemCount));

            return result;
        }


        private void DropAllItems(string[] items)
        {
            foreach (var item in items)
            {
                ExecuteCommandAutomated("drop " + item);
            }
        }
        private void TakeItems(string[] items)
        {
            foreach (var item in items)
            {
                ExecuteCommandAutomated("take " + item);
            }
        }
    }
}
