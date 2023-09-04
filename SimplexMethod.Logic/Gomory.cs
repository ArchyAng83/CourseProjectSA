using System;

namespace SimplexMethod.Logic
{
    public class Gomory
    {
        private readonly double[,] data;//матрица симплекс-таблицы

        public Gomory(double[,] data)
        {
            this.data = data;
        }

        //Решение
        public double[,] GetResult(double[] results, out bool output)
        {
            int maxIndexFractional;
            double[,] newData = data;
            int[,] tempBasis = new int[2, data.GetLength(0) + 1];

            for (int j = 0; j < data.GetLength(1); j++)
            {
                tempBasis[0, j] = j;
            }

            do
            {
                Simplex simplex = new Simplex(newData, tempBasis);
                newData = simplex.Calculate(results, out output);
                // проверить на целые числа
                if (CheckIntegers(results))
                {
                    break;
                }
                // выбрать наибольшую дробную часть
                maxIndexFractional = GetMaxFractional(newData);
                // добавить строку в массив (проверить симплексом)
                newData = GetNewData(newData, maxIndexFractional);
                // сохранить базис переменных
                tempBasis = GetNewBasis(tempBasis);

            } while (true);

            return newData;
        }

        //Добавление новой переменной в базис
        private int[,] GetNewBasis(int[,] tempBasis)
        {
            int[,] newBasis = new int[tempBasis.GetLength(0), tempBasis.GetLength(1) + 1];

            for (int i = 0; i < tempBasis.GetLength(0); i++)
            {
                for (int j = 0; j < tempBasis.GetLength(1); j++)
                {
                    newBasis[i, j] = tempBasis[i, j];
                }
            }

            return newBasis;
        }

        //Проверки на целочисленное решение
        private bool CheckIntegers(double[] results)
        {
            bool output = true;

            for (int i = 0; i < results.Length; i++)
            {
                if (Math.Abs(results[i] - Math.Round(results[i])) >= 0.0000000000002)
                {
                    output = false;
                    break;
                }
            }

            return output;
        }

        //Добавление новой строки с дробной частью в симплекс-таблицу
        private double[,] GetNewData(double[,] ndata, int index)
        {
            double[,] newData = new double[ndata.GetLength(0) + 1, ndata.GetLength(1)];
            for (int i = 0; i < ndata.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < ndata.GetLength(1); j++)
                {
                    newData[i, j] = ndata[i, j];
                }
            }

            for (int j = 0; j < newData.GetLength(1); j++)
            {
                newData[newData.GetLength(0) - 2, j] = -(ndata[index, j] - Math.Floor(ndata[index, j]));
                newData[newData.GetLength(0) - 1, j] = ndata[ndata.GetLength(0) - 1, j];
            }

            return newData;
        }

        //Поиск максимальной дробной части 
        private int GetMaxFractional(double[,] newData)
        {
            double maxFractional = newData[0, 0] - Math.Floor(newData[0, 0]);
            int maxIndexFractional = 0;
            for (int i = 0; i < newData.GetLength(0) - 1; i++)
            {
                if ((newData[i, 0] - Math.Floor(newData[i, 0])) > maxFractional)
                {
                    maxFractional = newData[i, 0] - Math.Floor(newData[i, 0]);
                    maxIndexFractional = i;
                }
            }

            return maxIndexFractional;
        }
    }
}
