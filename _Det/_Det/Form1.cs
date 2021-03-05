using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace _Det {

    public partial class Form1 : Form {

        List<string> LogList = new List<string>(); // Список шагов для вывода в файл

        public Form1() { // Первичная инициализация программы
            InitializeComponent();
            buttonCompute.Visible = false;
        }

        async void Gauss(int mSize) { // Асинхронный метод Гаусса для вычисления определителя
            double[,] Array = new double[mSize, mSize]; // Создаем локальный двухмерный массив для записи в него нашей матрицы

            for (int i = 0; i < mSize; i++) { // Цикл записи данных в матрицу
                for (int j = 0; j < mSize; j++) {
                    Array[i, j] = Convert.ToDouble(Controls[$"input{i}{j}"].Text); // Записываем поэлементно данные из окошек матрицы
                }
            }

            double buff = 0, times = 1; // Объявление локальный переменных для алгоритма Гаусса

            for (int i = 0; i < mSize - 1; i++) { // Снова обходим матрицу
                for (int j = i + 1; j < mSize; j++) {
                    if (Array[i, i] != 0) { // Элемент главной диагонали == 0
                        buff = Array[j, i] / Array[i, i]; // Закидываем в буффер частное элемента на 1 дальше и текущего элемента главной диагонали
                    }
                    for (int k = i; k < mSize; k++) { // Цикл работы с текущим столбцом
                        labelInfo.Text = $"Вычитаем из элемента {j}го столбца {k}й строки ({Array[j, k]}) произведения э-та {i}го столбца {k}й строки ({Array[i, k] * buff}) на {buff} (частное {j}го столбца {i}й строки и {i}го столбца {i}й строки)";
                        LogList.Add(labelInfo.Text); // Сверху вывод лога на экран + запись каждого шага в список для вывода в файл
                        Array[j, k] -= Math.Floor((Array[i, k] * buff) * 100) / 100; // Вычитание с округлением (Floor - пол -- нижняя граница дробного числа)
                        await Task.Delay(TimeSpan.FromMilliseconds(1000)); // Задержка в 1 секунду
                        for (int i1 = 0; i1 < mSize; i1++) { // Цикл обхода матрицы для пошагового вывода на экран
                            for (int j1 = 0; j1 < mSize; j1++) {
                                Controls[$"input{i1}{j1}"].Text = Array[i1, j1].ToString(); // Запись значений в окошки матрицы
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < mSize; i++) {
                times *= Array[i, i]; // После алгоритма перемножаем элементы главной диагонали
            }

            labelInfo.Text = $"Детерминант матрицы: {times}"; // Выводим детерминант на экран

            int f = 0; string[] ToFile = new string[LogList.Count]; // Запись в файл. Создаем новый строчный массив размера получившегося списка
            foreach (string capture in LogList) { // Проходимся по списку и в каждый элемент строчного массива забиваем данные из списка
                ToFile[f] = $"Шаг {f + 1} :: {capture}"; f++;
            }

            File.WriteAllLines("Log.txt", ToFile); // Записываем в файл (файл спавнится рядом с .exe)

        }

        private void buttonStep_Click(object sender, EventArgs e) { // Обработчик нажатия кнопки "создать"
            textBoxSize.Visible = false; // Делаем невидимым окно ввода размера матрицы
            buttonStep.Visible = false; // Делаем невидимым кнопку "создать"
            buttonCompute.Visible = true; // Кнопку "вычислить" делаем видимой
            int xStart = 12, yStart = 12; // Позиция окошка матрицы 0 строки 0 столбца (верхний левый угол) от которого строим остальные окна
            for (int i = 0; i < Convert.ToInt32(textBoxSize.Text); i++) { // Цикл обхода по матрице
                for (int j = 0; j < Convert.ToInt32(textBoxSize.Text); j++) {
                    TextBox Box = new TextBox(); // Создаем новый экземпляр окошка матрицы
                    Box.Name = $"input{i}{j}"; // Ставим ему имя "input" + цифры его координат в матрице
                    Box.Size = new Size(25, 20); // Ставим размер окна
                    Box.Location = new Point(xStart + (i * 33), yStart + (j * 26)); // Вычисление позиций окошек матрицы. Каждое окно дальше на 33 вправо и ниже на 26 вниз
                    Controls.Add(Box); // Добавляем на окно программы получившийся объект
                }
            }
        }

        private void buttonCompute_Click(object sender, EventArgs e) { // Обработчик нажатия кнопки вычислить
            buttonCompute.Visible = false; // Делаем невидимой кнопку "вычислить"
            Gauss(Convert.ToInt32(textBoxSize.Text)); // Вызываем функцию вычисления, отправляя параметром порядок матрицы
        }

    }
}
