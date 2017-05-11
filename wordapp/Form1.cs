using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wordapp
{
    public partial class Form1 : Form
    {
        String text = String.Empty;
        String result = String.Empty;
        int maxWordLength = 1;

        string[] safeSigns = { "!", "–", "«", "»", "_", "?", "(", ")", ";", "*", "/" };
        string[] victims = { ".", ":", ",", "\"", "'"};

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {                        
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                fileLabel.Text = "No file chosen";
                return;
            }
            var fileLength = new FileInfo(openFileDialog1.FileName).Length / 1024 / 1024;
            if (fileLength > 5)
            {
                MessageBox.Show("HTTP ERROR 500", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (StreamReader sr = new StreamReader(openFileDialog1.FileName, Encoding.Default))
            {
                text = sr.ReadToEnd();
            }
                
            fileLabel.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var path = saveFileDialog1.FileName;
            processText();
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
                {
                    sw.Write(result);
                }
            } catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            return;            
        }

        private void processText()
        {
            if (text == String.Empty)
                return;

            text = text.Replace(Environment.NewLine, "");
            if (KillVictimsCB.Checked)
            {
                foreach (var symbol in victims)
                {
                    text = text.Replace(symbol, "");
                }
                
            }

            var split = text.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0; i<split.Length; ++i)
            {
                if (getTrueWordLength(split[i]) <= maxWordLength)
                {
                    split[i] = "";
                }
            }

            result = String.Join(" ", split.Where(x => !x.Equals("")));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string newText = ((TextBox)sender).Text;
            int newSize = 0;
            if (int.TryParse(newText, out newSize))
            {
                maxWordLength = newSize < 0 ? 0 : newSize;
            }
            else maxWordLength = 0;
        }

        private int getTrueWordLength(string word)
        {
            int initialLength = word.Length;
            foreach(var item in victims)
            {
                word = word.Replace(item, "");
            }
            int newLength = word.Length;
            return newLength == 0 ? 1 : newLength;
        }
    }
}
