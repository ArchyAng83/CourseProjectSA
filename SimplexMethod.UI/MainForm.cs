using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SimplexMethod.Logic;

namespace SimplexMethod.UI
{
    public partial class MainForm : Form
    {
        private const int LIMITS_COUNT = 5;// кол-во ограничений
        private const int UNKNOWS_COUNT = 6;//кол-во неизвестных
        private readonly double[,] data;//матрица для симплекс-таблицы

        public MainForm()
        {
            InitializeComponent();

            data = new double[LIMITS_COUNT + 1, UNKNOWS_COUNT + 1];
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                double[] results = new double[data.GetLength(1) - 1];
                Gomory gomori = new Gomory(data);
                double[,] total = gomori.GetResult(results, out bool output);

                if (output)
                {
                    resultProduct1SemiAutoLabel.Text = $"{Math.Round(results[0])} шт.";
                    resultProduct2SemiAutoLabel.Text = $"{Math.Round(results[1])} шт.";
                    resultProduct3SemiAutoLabel.Text = $"{Math.Round(results[2])} шт.";
                    resultProduct1AutoLabel.Text = $"{Math.Round(results[3])} шт.";
                    resultProduct2AutoLabel.Text = $"{Math.Round(results[4])} шт.";
                    resultProduct3AutoLabel.Text = $"{Math.Round(results[5])} шт.";
                    resultMinCostLabel.Text = $"{Math.Round(Math.Abs(total[total.GetLength(0) - 1, 0]))} ден.ед.";
                }
                else
                {
                    NonSolveResult();
                    MessageBox.Show("Нет решений!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Неверный ввод данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearAllDataFields();
        }

        // Очистка полей ввода-вывода
        private void ClearAllDataFields()
        {
            timeProduct1SemiAutoTextBox.Text = "0";
            timeProduct2SemiAutoTextBox.Text = "0";
            timeProduct3SemiAutoTextBox.Text = "0";
            costProduct1SemiAutoTextBox.Text = "0";
            costProduct2SemiAutoTextBox.Text = "0";
            costProduct3SemiAutoTextBox.Text = "0";
            timeLimitSemiAutoTextBox.Text = "0";

            timeProduct1AutoTextBox.Text = "0";
            timeProduct2AutoTextBox.Text = "0";
            timeProduct3AutoTextBox.Text = "0";
            costProduct1AutoTextBox.Text = "0";
            costProduct2AutoTextBox.Text = "0";
            costProduct3AutoTextBox.Text = "0";
            timeLimitAutoTextBox.Text = "0";

            orderProduct1TextBox.Text = "0";
            orderProduct2TextBox.Text = "0";
            orderProduct3TextBox.Text = "0";

            resultProduct1SemiAutoLabel.Text = "<none>";
            resultProduct2SemiAutoLabel.Text = "<none>";
            resultProduct3SemiAutoLabel.Text = "<none>";
            resultProduct1AutoLabel.Text = "<none>";
            resultProduct2AutoLabel.Text = "<none>";
            resultProduct3AutoLabel.Text = "<none>";
            resultMinCostLabel.Text = "<none>";
        }

        // Проверка валидации данных
        private bool ValidateForm()
        {
            bool output = true;
            try
            {
                data[0, 0] = Convert.ToDouble(timeLimitSemiAutoTextBox.Text);
                data[0, 1] = Convert.ToDouble(timeProduct1SemiAutoTextBox.Text);
                data[0, 2] = Convert.ToDouble(timeProduct2SemiAutoTextBox.Text);
                data[0, 3] = Convert.ToDouble(timeProduct3SemiAutoTextBox.Text);
                data[1, 0] = Convert.ToDouble(timeLimitAutoTextBox.Text);
                data[1, 4] = Convert.ToDouble(timeProduct1AutoTextBox.Text);
                data[1, 5] = Convert.ToDouble(timeProduct2AutoTextBox.Text);
                data[1, 6] = Convert.ToDouble(timeProduct3AutoTextBox.Text);
                data[2, 0] = -Convert.ToDouble(orderProduct1TextBox.Text);
                data[2, 1] = -1;
                data[2, 4] = -1;
                data[3, 0] = -Convert.ToDouble(orderProduct2TextBox.Text);
                data[3, 2] = -1;
                data[3, 5] = -1;
                data[4, 0] = -Convert.ToDouble(orderProduct3TextBox.Text);
                data[4, 3] = -1;
                data[4, 6] = -1;
                data[5, 1] = Convert.ToDouble(costProduct1SemiAutoTextBox.Text);
                data[5, 2] = Convert.ToDouble(costProduct2SemiAutoTextBox.Text);
                data[5, 3] = Convert.ToDouble(costProduct3SemiAutoTextBox.Text);
                data[5, 4] = Convert.ToDouble(costProduct1AutoTextBox.Text);
                data[5, 5] = Convert.ToDouble(costProduct2AutoTextBox.Text);
                data[5, 6] = Convert.ToDouble(costProduct3AutoTextBox.Text);

                CheckNegativeNumbers();
            }
            catch (FormatException)
            {
                output = false;
            }

            return output;
        }

        // Результат при отсутствии решения
        private void NonSolveResult()
        {
            resultProduct1SemiAutoLabel.Text = "Нет решений";
            resultProduct2SemiAutoLabel.Text = "Нет решений";
            resultProduct3SemiAutoLabel.Text = "Нет решений";
            resultProduct1AutoLabel.Text = "Нет решений";
            resultProduct2AutoLabel.Text = "Нет решений";
            resultProduct3AutoLabel.Text = "Нет решений";
            resultMinCostLabel.Text = "Нет решений";
        }

        // Проверка на ввод отрицательных чисел
        private void CheckNegativeNumbers()
        {
            if (Convert.ToDouble(timeLimitSemiAutoTextBox.Text) < 0 || Convert.ToDouble(timeProduct1SemiAutoTextBox.Text) < 0 ||
                Convert.ToDouble(timeProduct2SemiAutoTextBox.Text) < 0 || Convert.ToDouble(timeProduct3SemiAutoTextBox.Text) < 0 ||
                Convert.ToDouble(timeLimitAutoTextBox.Text) < 0 || Convert.ToDouble(timeProduct1AutoTextBox.Text) < 0 ||
                Convert.ToDouble(timeProduct2AutoTextBox.Text) < 0 || Convert.ToDouble(timeProduct3AutoTextBox.Text) < 0 ||
                Convert.ToDouble(costProduct1SemiAutoTextBox.Text) < 0 || Convert.ToDouble(costProduct2SemiAutoTextBox.Text) < 0 ||
                Convert.ToDouble(costProduct3SemiAutoTextBox.Text) < 0 || Convert.ToDouble(costProduct1AutoTextBox.Text) < 0 ||
                Convert.ToDouble(costProduct2AutoTextBox.Text) < 0 || Convert.ToDouble(costProduct3AutoTextBox.Text) < 0 ||
                Convert.ToDouble(orderProduct1TextBox.Text) < 0 || Convert.ToDouble(orderProduct2TextBox.Text) < 0 || Convert.ToDouble(orderProduct3TextBox.Text) < 0)
            {
                throw new FormatException();
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
        }
                
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllDataFields();
            LoadFromTextFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToTextFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // Проверка при закрытии формы
        private void CloseForm()
        {
            DialogResult result = MessageBox.Show("Желаете сохранить данные?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SaveToTextFile();
            }
        }

        // Запись в текстовый файл
        private void SaveToTextFile()
        {
            using (SaveFileDialog saveFile = new SaveFileDialog() { Filter = "|*.txt" })
            {
                saveFile.InitialDirectory = GlobalConfig.pathToFile;
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveFile.FileName, false, Encoding.UTF8))
                        {
                            sw.WriteLine(timeProduct1SemiAutoTextBox.Text + " | " + timeProduct2SemiAutoTextBox.Text + " | " + timeProduct3SemiAutoTextBox.Text + " | ");
                            sw.WriteLine(costProduct1SemiAutoTextBox.Text + " | " + costProduct2SemiAutoTextBox.Text + " | " + costProduct3SemiAutoTextBox.Text + " | ");
                            sw.WriteLine(timeLimitSemiAutoTextBox.Text + " | ");
                            sw.WriteLine(timeProduct1AutoTextBox.Text + " | " + timeProduct2AutoTextBox.Text + " | " + timeProduct3AutoTextBox.Text + " | ");
                            sw.WriteLine(costProduct1AutoTextBox.Text + " | " + costProduct2AutoTextBox.Text + " | " + costProduct3AutoTextBox.Text + " | ");
                            sw.WriteLine(timeLimitAutoTextBox.Text + " | ");
                            sw.WriteLine(orderProduct1TextBox.Text + " | " + orderProduct2TextBox.Text + " | " + orderProduct3TextBox.Text + " | ");

                            MessageBox.Show("Файл сохранен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка сохранения данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Чтение из текстового файла
        private void LoadFromTextFile()
        {
            using (OpenFileDialog openFile = new OpenFileDialog() { Filter = "|*.txt", Multiselect = false })
            {
                try
                {
                    openFile.InitialDirectory = GlobalConfig.pathToFile;
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        string[] str = File.ReadAllText(openFile.FileName).Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        timeProduct1SemiAutoTextBox.Text = str[0];
                        timeProduct2SemiAutoTextBox.Text = str[1];
                        timeProduct3SemiAutoTextBox.Text = str[2];
                        costProduct1SemiAutoTextBox.Text = str[3];
                        costProduct2SemiAutoTextBox.Text = str[4];
                        costProduct3SemiAutoTextBox.Text = str[5];
                        timeLimitSemiAutoTextBox.Text = str[6];

                        timeProduct1AutoTextBox.Text = str[7];
                        timeProduct2AutoTextBox.Text = str[8];
                        timeProduct3AutoTextBox.Text = str[9];
                        costProduct1AutoTextBox.Text = str[10];
                        costProduct2AutoTextBox.Text = str[11];
                        costProduct3AutoTextBox.Text = str[12];
                        timeLimitAutoTextBox.Text = str[13];

                        orderProduct1TextBox.Text = str[14];
                        orderProduct2TextBox.Text = str[15];
                        orderProduct3TextBox.Text = str[16];
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка чтения данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseForm();
            Application.Exit();
        }
    }
}
