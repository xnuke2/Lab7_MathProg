using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7_MathProg
{
    public partial class Form1 : Form
    {
        int range;
        int rowIndexGlobal = 0;

        public Form1()
        {
            InitializeComponent();
            range = Convert.ToInt32(numericUpDown1.Value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            numericUpDown1_ValueChanged(sender, e);
            int[,] init =
            {
                {47,53, 67 },
                {35,7,52 },
                {96,35,54 }
            };
            for (int i = 0; i < range; i++) 
                for (int j = 0; j < range; j++)
                    dataGridView1[i,j].Value = init[j,i];

            //button2_Click(sender, e);

         
            //MessageBox.Show("");
        }

        void PrintMatrix(int[,] matrix, int[,]check)
        {
            if (rowIndexGlobal != 0)
            { 
                dataGridView2.Rows.Add();
                rowIndexGlobal++;
            }

            for (int i = 0; i < range; i++)
            {
                dataGridView2.Rows.Add();
                for (int j = 0; j < range; j++)
                    dataGridView2[j, rowIndexGlobal + i].Value = matrix[i, j];
            }

            for (int i = 0; i < range; i++)
                for (int j = 0; j < range; j++)
                {
                    if (check[i, j] == 1)
                        dataGridView2[j, rowIndexGlobal + i].Style.ForeColor = Color.Blue;
                    if (check[i, j] == -1)
                        dataGridView2[j, rowIndexGlobal + i].Style.ForeColor = Color.Red;
                }

            rowIndexGlobal += range;

        }
        (int,List<int>) hungarian(int[,] matrix)
        {
            int[,] init = new int[range,range];
            for (int i = 0; i < range; i++)
                for (int j = 0; j < range; j++)
                    init[i,j] = matrix[i,j];
           
            do
            {
                //Получение нулей в каждой строке
                for (int i = 0; i < range; i++)
                {
                    int minIndex = 0;
                    int minValue = matrix[i, 0];
                    for (int j = 1; j < range; j++)
                        if (matrix[i, j] < minValue)
                        {
                            minIndex = j;
                            minValue = matrix[i, j];
                        }
                    for (int j = 0; j < range; j++)
                        matrix[i, j] -= minValue;
                }
                //Получение нулей в каждом столбце
                for (int i = 0; i < range; i++)
                {
                    int minIndex = 0;
                    int minValue = matrix[0, i];
                    for (int j = 1; j < range; j++)
                        if (matrix[j, i] < minValue)
                        {
                            minIndex = j;
                            minValue = matrix[j, i];
                        }
                    for (int j = 0; j < range; j++)
                        matrix[j, i] -= minValue;
                }

                //Поиск оптимального решения
                int[,] check = new int[range, range];
                int minZeroIndex;
                int minZeroCount;
                int countZeroPl = 0;
                do
                {
                    minZeroIndex = -1;
                    minZeroCount = range;
                    for (int i = 0; i < range; i++)
                    {
                        int zeroCount = 0;
                        for (int j = 0; j < range; j++)
                            if (matrix[i, j] == 0 && check[i, j] == 0)
                                zeroCount++;

                        if (zeroCount < minZeroCount && zeroCount > 0)
                        {
                            minZeroIndex = i;
                            minZeroCount = zeroCount;
                        }
                            
                    }
                    bool first = true;
                    if (minZeroIndex != -1)
                    {
                        countZeroPl++;
                        for (int i = 0; i < range; i++)
                            if (matrix[minZeroIndex, i] == 0 && check[minZeroIndex, i] == 0)
                            {
                                if (first)
                                {
                                    check[minZeroIndex, i] = 1;
                                    for (int j = 0; j < range; j++)
                                        if (matrix[j, i] == 0 && check[j, i] == 0)
                                            check[j, i] = -1;
                                    first = false;
                                }
                                else check[minZeroIndex, i] = -1;
                            }
                    }



                } while (minZeroIndex != -1);
                PrintMatrix(matrix,check);
                if (countZeroPl == range)
                {
                    
                    // Формирование результата
                    List<int> result = new List<int>();
                    for (int i = 0; i < range; i++)
                        for (int j = 0; j < range; j++)
                            if (check[i, j] == 1)
                            {
                                result.Add(j);
                                break;
                            }
                    int sum = 0;
                    for (int i = 0; i < range; i++)
                        sum += init[i, result[i]];
                    return (sum,result);
                }
                //return (0, null);
                bool[] rows = new bool[range];
                bool[] cols = new bool[range];
                for (int i = 0; i < range; i++)
                {
                    rows[i] = true; cols[i] = true;
                }
                //1.1
                for (int i = 0; i < range; i++)
                {
                    bool hasZero = false;
                    for (int j = 0; j < range; j++)
                        if (check[i, j] == 1)
                        {
                            hasZero = true;
                            break;
                        }
                    rows[i] = hasZero;
                }

                //1.2
                for (int i = 0; i < range; i++)
                {
                    if (!rows[i])
                    {
                        for (int j = 0; j < range; j++)
                            if (check[i, j] == -1)
                            {
                                cols[j] = false;
                                break;
                            }

                    }
                }
                for (int i = 0; i < range; i++)
                    if (!cols[i])
                        for (int j = 0; j < range; j++)
                            if (check[j, i] == 1)
                                rows[j] = false;
                List<int> list = new List<int>();
                for (int i = 0; i < range; i++)
                    for (int j = 0; j < range; j++)
                        if (!(rows[i] || !cols[j])&& matrix[i, j]!=0)
                            list.Add(matrix[i, j]);
                int min = list.Min();
                for (int i = 0; i < range; i++)
                    if (rows[i])
                        for (int j = 0; j < range; j++)
                            matrix[i, j] -= min;
                for (int i = 0; i < range; i++)
                    if (!cols[i])
                        for (int j = 0; j < range; j++)
                            matrix[j, i] += min;

            } while (true);

           
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear(); 
            range = Convert.ToInt32(numericUpDown1.Value);
            for (int i = 0; i < range; i++)
                dataGridView1.Columns.Add(i+"", "");
            for (int i = 0; i < range; i++)
                dataGridView1.Rows.Add("");
            for (int i = 0; i < range; i++)
                for (int j = 0; j < range; j++)
                    dataGridView1[i, j].Value = 0;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            for (int i = 0; i < range; i++)
                for (int j = 0; j < range; j++)
                    dataGridView1[i, j].Value = random.Next(100);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rowIndexGlobal = 0;
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            for (int i = 0; i < range; i++)
                dataGridView2.Columns.Add("" + i, "");
            int[,] matrix = new int[range, range];
            for (int i = 0;i < range; i++)
                for(int j = 0;j < range; j++)
                    matrix[i,j] = Convert.ToInt32(dataGridView1[j,i].Value);
            int sum;
            List<int> res;
            (sum,res)= hungarian(matrix);
            label2.Text = "результат " + sum;
        }
    }
}
