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

        private void StartComputer()
        {
            string fileName = @"D:\Dropbox\Work\AdventOfCode\2019\Day25\Full.txt";
            Computer = new Computer(ReadProgram(fileName));

            Computer.Input += (c) => Writer.Take();
            Computer.Output += (c, value) => CharacterReceived((char)value);

            Task.Factory.StartNew(() => Computer.Run(), TaskCreationOptions.LongRunning);
        }

        private void CharacterReceived(char ch)
        {
            string text = (ch == 10 ? Environment.NewLine : "" + ch);
            BeginInvoke(() => textBoxOuput.AppendText(text));
        }

        private void ExecuteCommand(string command)
        {
            foreach (var ch in command)
            {
                Writer.Add(ch);
            }

            Writer.Add(10);
        }

        static long[] ReadProgram(string fileName)
        {
            return File.ReadAllText(fileName).Split(',').Select(long.Parse).ToArray();
        }

        private void textBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ExecuteCommand(textBoxInput.Text);
                textBoxInput.Clear();
                e.Handled = true;
                e.SuppressKeyPress = true;  
            }

        }
    }
}
