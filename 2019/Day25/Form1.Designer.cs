namespace Day25
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxInput = new TextBox();
            textBoxOuput = new TextBox();
            buttonAutomate = new Button();
            SuspendLayout();
            // 
            // textBoxInput
            // 
            textBoxInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxInput.Location = new Point(12, 23);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(836, 23);
            textBoxInput.TabIndex = 0;
            textBoxInput.KeyDown += textBoxInput_KeyDown;
            // 
            // textBoxOuput
            // 
            textBoxOuput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxOuput.Location = new Point(12, 52);
            textBoxOuput.Multiline = true;
            textBoxOuput.Name = "textBoxOuput";
            textBoxOuput.ReadOnly = true;
            textBoxOuput.ScrollBars = ScrollBars.Vertical;
            textBoxOuput.Size = new Size(836, 447);
            textBoxOuput.TabIndex = 1;
            // 
            // buttonAutomate
            // 
            buttonAutomate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonAutomate.Location = new Point(12, 523);
            buttonAutomate.Name = "buttonAutomate";
            buttonAutomate.Size = new Size(112, 23);
            buttonAutomate.TabIndex = 2;
            buttonAutomate.Text = "Automate";
            buttonAutomate.UseVisualStyleBackColor = true;
            buttonAutomate.Click += buttonAutomate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 558);
            Controls.Add(buttonAutomate);
            Controls.Add(textBoxOuput);
            Controls.Add(textBoxInput);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "AOC 2019 Day 25";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxInput;
        private TextBox textBoxOuput;
        private Button buttonAutomate;
    }
}
