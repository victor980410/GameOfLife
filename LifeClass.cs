using System;
using System.Drawing;
using System.IO;

namespace GameOfLife
{
    // class that control everything 
    class LifeClass
    {
        private int[,] field = new int[50, 50];
        private int[,] oldField = new int[50, 50];

        /* 
         * 0 cell was never alive
         * -1 cell was alived but died
         * 1 cell which became alive
         * 2 cell which is alive more than 1 generation
        */
        private int[,] cellStatus = new int[50, 50];

        private Color deadCell = Color.Black;
        private Color aliveCell = Color.Red;
        private Color wasAliveCell = Color.FromArgb(86, 0, 0);

        private Bitmap image;

        // contructor
        public LifeClass()
        {
            image = Draw();
        }

        // clear field
        public void Clear()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    field[i, j] = 0;
                    cellStatus[i, j] = 0;
                }
            }
        }

        // saving current field to a file
        public void SaveSeed(string fileName)
        {
            using (StreamWriter writetext = new StreamWriter(fileName))
            {
                for(int i = 0; i < 50; i++)
                {
                    for(int j = 0; j < 50; j++)
                    {
                        writetext.Write(field[i, j] + " ");
                    }
                    writetext.Write("\n");
                }
            }
        }


        // loading seed from a txt file
        public void LoadSeed(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {

                for (int i = 0; i < 50; i++)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    for (int j = 0; j < 50; j++)
                    {
                        oldField[i, j] = Convert.ToInt32(line[j]);
                        if (oldField[i, j] != 1 && oldField[i, j] != 0)
                            throw new FileLoadException();  
                    }
                }
            }
            Clear();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    field[i, j] = oldField[i, j];
                }
            }
        }

        // changing specific cell on image and on field
        public Bitmap ChangeCell(int x, int y)
        {
            if (x % 10 == 0 || y % 10 == 0)
            {
                return image;
            }
            field[x/10, y/10] = Convert.ToInt32(!Convert.ToBoolean(field[x/10, y/10]));
            for (int i = RoundOff(x) + 1; i < RoundOff(x) + 10; i++)
            {
                for (int j = RoundOff(y) + 1; j < RoundOff(y) + 10; j++)
                {
                    if(field[x/10,y/10] == 1)
                        image.SetPixel(i, j, aliveCell);
                    else
                        image.SetPixel(i, j, deadCell);
                }
            }
            return image;
        }

        // function which draw field 
        public Bitmap Draw()
        {
            image = new Bitmap(501, 501);
            for (int i = 0; i < 501; i++)
            {
                for (int j = 0; j < 501; j++)
                {
                    if (i % 10 == 0 || j % 10 == 0)
                    {
                        image.SetPixel(i, j, SystemColors.ControlDarkDark);
                    }
                    else
                    if(cellStatus[i/10,j/10] == 0)
                        image.SetPixel(i, j, deadCell);
                    else if (cellStatus[i / 10, j / 10] == -1)
                        image.SetPixel(i, j, wasAliveCell);
                    else if (cellStatus[i / 10, j / 10] == 1)
                        image.SetPixel(i, j, aliveCell);
                    else if (cellStatus[i / 10, j / 10] == 2)
                        image.SetPixel(i, j, aliveCell);

                }
            }
            return image;
        }

        // 11 -> 10, 15 -> 10, 29 -> 20 etc.
        int RoundOff(int i)
        {
            return ((int)(i / 10.0)) * 10;
        }

        // calculates number of alive neighbors
        private int neighborsNumber(int posX, int posY)
        {
            if (posX > 0 && posX < 49 && posY > 0 && posY < 49)
                return oldField[posX - 1, posY - 1] + oldField[posX - 1, posY] + oldField[posX - 1, posY + 1] +
                           oldField[posX, posY - 1] + oldField[posX, posY + 1] +
                           oldField[posX + 1, posY - 1] + oldField[posX + 1, posY] + oldField[posX + 1, posY + 1];

            if (posX == 0 && posY == 0)
                return oldField[1, 0] + oldField[0, 1] + oldField[1, 1];
            if (posX == 49 && posY == 49)
                return oldField[posX - 1, 49] + oldField[posX, posY - 1] + oldField[posX - 1, posY - 1];
            if (posX == 49 && posY == 0)
                return oldField[posX - 1, 0] + oldField[posX, 1] + oldField[posX - 1, 1];
            if (posX == 0 && posY == 49)
                return oldField[0, 49 - 1] + oldField[1, 49] + oldField[1, 49 - 1];

            if (posX == 0)
                return oldField[0, posY - 1] + oldField[0, posY + 1] +
                        oldField[1, posY - 1] + oldField[1, posY] + oldField[1, posY + 1];
            if (posX == 49)
                return oldField[posX, posY - 1] + oldField[posX, posY + 1] +
                        oldField[posX - 1, posY - 1] + oldField[posX - 1, posY] + oldField[posX - 1, posY + 1];

            if (posY == 0)
                return oldField[posX - 1, 0] + oldField[posX + 1, 0] +
                        oldField[posX - 1, 1] + oldField[posX, 1] + oldField[posX + 1, 1];
            if (posY == 49)
                return oldField[posX - 1, 49] + oldField[posX + 1, 49] +
                        oldField[posX - 1, 49 - 1] + oldField[posX, posY - 1] + oldField[posX + 1, posY - 1];
            return 0;
        }


        // function which create next generation base on current field
        public Bitmap NextGeneration()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    oldField[i, j] = field[i, j];
                }
            }
            for (int i = 0; i < 50; i++) 
            {
                for (int j = 0; j < 50; j++) 
                {
                    int neighbors = neighborsNumber(i, j);
                    if (field[i,j] == 0)
                    {
                        if(neighbors == 3)
                        {
                            field[i, j] = 1;
                            cellStatus[i,j] = 1;
                        } 
                    } else
                    {
                        if(neighbors < 2 || neighbors > 3)
                        {
                            field[i, j] = 0;
                            cellStatus[i, j] = -1;
                        } else
                            cellStatus[i, j] = 2;
                    }
                }
            }
            return Draw();
        }
    }
}
