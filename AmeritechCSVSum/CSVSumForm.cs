using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmeritechCSVSum
{
    public partial class CSVSumForm : Form
    {
        public CSVSumForm()
        {
            InitializeComponent();
        }

        //Opens a file browser on button click, then sends the FileName to readCSV
        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileBrowser = new OpenFileDialog();
            fileBrowser.Filter = "CSV File (*.csv)|*.csv";

            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                //A double check on the selected file type
                if (Path.GetExtension(fileBrowser.SafeFileName) == ".csv")
                {
                    readCSV(fileBrowser.FileName);
                }
                else
                {
                    MessageBox.Show("Incorrect file type of: "+ Path.GetExtension(fileBrowser.SafeFileName));
                }
            }
            fileBrowser.Dispose();
        }

        //Referenced in browseButton_Click. Opens the csv with StreamReader. Reading one line and sending it to parseLine, then updateOutputBox
        private void readCSV(string filePath)
        {
            //lineCounter is for an output error message in parseLine
            int lineCounter = 1;
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        string currLine = sr.ReadLine();
                        //Removes anything not in the first column
                        currLine = currLine.Split(',')[0];
                        ulong lineLong = parseLine(currLine, lineCounter);
                        updateOutputBox(lineLong);
                        lineCounter++;
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Process failed: " + e.ToString());
            }
        }

        //Referenced in readCSV. Takes in the current line of the csv and removes non-digits then all but the last 10 digits. Then converts to ulong and returns.
        private ulong parseLine(string line, int lineCounter)
        {
            //Remove non-digit characters from line
            string currLine = Regex.Replace(line, "[^0-9]", "");
            //Remove all but last 10 characters from string, unless string is less than 10 characters
            currLine = currLine.Substring(Math.Max(0, currLine.Length - 10));

            ulong currLong;
            bool success = UInt64.TryParse(currLine, out currLong);
            if (success)
            {
                return currLong;
            }
            else
            {
                MessageBox.Show("Conversion of string to long failed. Line number "+ lineCounter +" from file.");
                return 0;
            }
        }

        //Referenced in readCSV. Converts the TextBox value to ulong then adds the ulong from the current line of the csv. Converts the sum to string and updates the TextBox
        private void updateOutputBox(ulong currLong)
        {
            ulong sum = 0;
            //Converts text in outputBox to long so it can be added with the current line from the csv
            if (!UInt64.TryParse(outputBox.Text, out sum))
            {
                MessageBox.Show("Conversion of string to long failed. outputBox.");
            }
            sum += currLong;
            outputBox.Text = sum.ToString();
        }
    }
}
