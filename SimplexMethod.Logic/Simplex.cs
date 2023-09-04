using System;

namespace SimplexMethod.Logic
{
    public class Simplex
    {
        private double[,] data; //матрица симплекс-таблицы
        private readonly int cols;//кол-во столбцов симплекс таблицы
        private readonly int rows;// кол-во строк симплекс таблицы
        private readonly int[] basis;// массив базиса
        private readonly int[,] tempBasis;// массив временного базиса

        public Simplex(double[,] data)
        {
            this.data = data;
            rows = data.GetLength(0);
            cols = data.GetLength(1);
            basis = new int[rows + 1];
            tempBasis = new int[2, rows + 1];

            for (int j = 0; j < cols; j++)
            {
                tempBasis[0, j] = j;
            }
        }

        public Simplex(double[,] data, int[,] tempBasis)
        {
            this.data = data;
            rows = data.GetLength(0);
            cols = data.GetLength(1);
            this.tempBasis = tempBasis;
            basis = new int[tempBasis.GetLength(1)];
        }

        //Расчет 
        public double[,] Calculate(double[] results, out bool output)
        {
            while (!IsReferencePlan(out int freeRow))
            {
                int mainRow = FindReferencePlan(out int mainCol);

                if (IsEndForReference(freeRow))
                {
                    output = false;
                    return data;
                }

                //заполнить таблицу
                FillTable(mainRow, mainCol);
            }

            while (!IsOptimalPlan())
            {
                int mainCol = FindOptimalPlan(out int mainRow);

                if (IsEndForOptimal(mainCol))
                {
                    output = false;
                    return data;
                }

                //залонить таблицу
                FillTable(mainRow, mainCol);
            }

            for (int j = 0; j < basis.Length; j++)
            {
                basis[j] = tempBasis[1, j];
            }

            for (int i = 0; i < results.Length; i++)
            {
                int k = Array.IndexOf(basis, i + 1);
                if (k != -1)
                    results[i] = data[k, 0];
                else
                    results[i] = 0;
            }

            output = true;

            return data;
        }        

        //Проверка на опорность
        private bool IsReferencePlan(out int freeRow)
        {
            bool output = true;
            freeRow = 0;

            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, 0] < 0)
                {
                    output = false;
                    freeRow = i;
                    break;
                }
            }

            return output;
        }

        //Проверка на оптимальность
        private bool IsOptimalPlan()
        {
            bool output = true;

            for (int j = 1; j < cols; j++)
            {
                if (data[data.GetLength(0) - 1, j] == 0)
                {
                    return output;
                }
            }

            for (int j = 1; j < cols; j++)
            {
                if (data[rows - 1, j] < 0)
                {
                    output = false;
                    break;
                }
            }

            return output;
        }

        //Поиск опорного решения
        private int FindReferencePlan(out int mainCol)
        {
            int mainRow = 0;
            mainCol = 0;

            if (!SearchZeroInReference(out mainCol))
            {
                //выбор с.ч. < 0
                for (int i = 0; i < rows - 1; i++)
                {
                    if (data[i, 0] < 0)
                    {
                        mainRow = i;
                        break;
                    }
                }

                //выбор эл-та < 0 в этой строке
                for (int j = 1; j < cols; j++)
                {
                    if (data[mainRow, j] < 0)
                    {
                        mainCol = j;
                        break;
                    }
                }

                //выбор мин эл-то, у которого положит. СО минимум
                mainRow = FindMainRow(mainCol);

                Swap(mainRow, mainCol);

                return mainRow;
            }

            //выбор мин эл-то, у которого положит. СО минимум
            mainRow = FindMainRowZero(mainCol);

            Swap(mainRow, mainCol);

            return mainRow;
        }  

        //Поиск оптимального решения
        private int FindOptimalPlan(out int mainRow)
        {
            double minNegative = data[rows - 1, 1];
            int mainCol;

            if (!SearchZeroInOptimal(out mainCol))
            {
                mainCol = 1;
                //выбор макс эл-та в f-строке
                for (int j = 1; j < cols; j++)
                {
                    if (data[rows - 1, j] < minNegative)
                    {
                        minNegative = data[rows - 1, j];
                        mainCol = j;
                    }
                }

                //выбор мин эл-то, у которого положит. СО минимум
                mainRow = FindMainRow(mainCol);

                Swap(mainRow, mainCol);

                return mainCol;
            }

            //выбор мин эл-то, у которого положит. СО минимум
            mainRow = FindMainRowZero(mainCol);

            Swap(mainRow, mainCol);

            return mainCol;
        }

        //Поиск условия при ноле в с.ч. при опорном
        private bool SearchZeroInReference(out int mainCol)
        {
            bool isZero = false;
            int mainRow = 0;
            mainCol = 0;
            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, 0] == 0)
                {
                    isZero = true;
                    mainRow = i;
                    break;
                }
            }

            if (isZero)
            {
                for (int j = 1; j < cols; j++)
                {
                    if (data[mainRow, j] < 0)
                    {
                        isZero = false;
                        mainCol = j;
                        for (int i = 0; i < rows - 1; i++)
                        {
                            if (data[i, mainCol] < 0)
                            {
                                isZero = true;
                                break;
                            }
                        }

                        if (isZero)
                        {
                            break;
                        }
                    }
                }
            }

            if (mainCol == 0)
            {
                isZero = false;
            }

            return isZero;
        }

        //Поиск условия при ноле в с.ч. при оптимуме
        private bool SearchZeroInOptimal(out int mainCol)
        {
            bool isZero = false;
            int mainRow = 0;
            mainCol = 0;
            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, 0] == 0)
                {
                    isZero = true;
                    mainRow = i;
                    break;
                }
            }

            if (isZero)
            {
                for (int j = 1; j < cols; j++)
                {
                    if (data[mainRow, j] < 0)
                    {
                        isZero = false;
                        mainCol = j;
                        if (data[rows - 1, mainCol] < 0)
                        {
                            isZero = true;
                            break;
                        }
                    }
                }
            }

            if (mainCol == 0)
            {
                isZero = false;
            }

            return isZero;
        }

        //Поиск разрешающей строки для данного разрешающего столбца
        private int FindMainRow(int mainCol)
        {
            int mainRow = 0;
            double min = double.MaxValue;
            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, mainCol] != 0 && (data[i, 0] / data[i, mainCol]) >= 0 && (data[i, 0] / data[i, mainCol]) < min)
                {
                    min = data[i, 0] / data[i, mainCol];
                    mainRow = i;
                }
            }

            return mainRow;
        }

        //Поиск разрешающей строки для данного разрешающего столбца, если ноль в с.ч. удовлетворяет условию
        private int FindMainRowZero(int mainCol)
        {
            int mainRow = 0;
            double min = double.MaxValue;
            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, mainCol] != 0 && (data[i, 0] / data[i, mainCol]) > 0 && (data[i, 0] / data[i, mainCol]) < min)
                {
                    min = data[i, 0] / data[i, mainCol];
                    mainRow = i;
                }
            }

            return mainRow;
        }

        //Заполнение новой симплекс-таблицы
        private void FillTable(int mainRow, int mainCol)
        {
            double[,] newData = new double[rows, cols];

            //заполнить разрешающую клетку на обратн. разр. эл-та
            newData[mainRow, mainCol] = 1.0 / data[mainRow, mainCol];

            //делим эл-ты строки на разр. эл-т
            for (int j = 0; j < cols; j++)
            {
                if (j != mainCol)
                {
                    newData[mainRow, j] = data[mainRow, j] / data[mainRow, mainCol];
                }
            }

            //делим эл-ты столбца на отриц. разр. эл-т
            for (int i = 0; i < rows; i++)
            {
                if (i != mainRow)
                {
                    newData[i, mainCol] = -(data[i, mainCol] / data[mainRow, mainCol]);
                }
            }

            //заполнить остальные эл-ты Жордановским методом
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i != mainRow && j != mainCol)
                    {
                        newData[i, j] = (data[i, j] * data[mainRow, mainCol] - data[mainRow, j] * data[i, mainCol]) / data[mainRow, mainCol];
                    }
                }
            }

            data = newData;
        }

        //Проверка на завершение, если все эл-ты в столбцы отрицательные
        private bool IsEndForOptimal(int mainCol)
        {
            bool output = true;

            for (int i = 0; i < rows - 1; i++)
            {
                if (data[i, mainCol] > 0)
                {
                    output = false;
                    break;
                }
            }

            return output;
        }

        //Проверка на завершение, если все эл-ты в строке отрицательные
        private bool IsEndForReference(int mainRow)
        {
            bool output = true;

            for (int j = 1; j < cols; j++)
            {
                if (data[mainRow, j] < 0)
                {
                    output = false;
                    break;
                }
            }

            return output;
        }

        //Менять местами Xn на Yn
        private void Swap(int mainRow, int mainCol)
        {
            int temp = tempBasis[1, mainRow];
            tempBasis[1, mainRow] = tempBasis[0, mainCol];
            tempBasis[0, mainCol] = temp;
        }
    }
}
