using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pumpkins
{
    internal class Program
    {
        class PumpkinInfo
        {
            public int row;
            public int column;
            public bool isAlive;
            public int dayDied;
        }

        enum Derection
        {
            None,
            Up,
            Right,
            Down,
            Left,
            Center
        }

        class Root
        {
            public Derection derection;
            public bool isFinishedDay;
            public bool isAlive;
        }

        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] inputs = input.Split(' ');
            int p = int.Parse(inputs[0]);
            int d = int.Parse(inputs[1]);
            int n = int.Parse(inputs[2]);

            if (d < 0 || d > 10 || n < 1 || n > 30 || p < 1 || p > n * n)
                return;

            Root[,] grid = new Root[n, n];
            List<PumpkinInfo> pumpkins = new List<PumpkinInfo>();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    grid[i, j] = new Root() { derection = Derection.None, isFinishedDay = true };

            for (int i = 0; i < p; i++)
            {
                input = Console.ReadLine();
                inputs = input.Split(' ');
                pumpkins.Add(new PumpkinInfo() { row = int.Parse(inputs[0]), column = int.Parse(inputs[1]), isAlive = true });
                grid[pumpkins[i].row, pumpkins[i].column] = new Root() { derection = Derection.Center, isAlive = true };
            }

            int day = 1;

            while (pumpkins.Where(pumpkin => pumpkin.isAlive).Count() > 0) //days
            {
                RootsStartDay(grid, n);

                for (int i = 0; i < p; i++)
                {
                    if (pumpkins[i].isAlive)
                    {
                        // root direction 1 - up, 2 - right, 3 - down, 4 - left
                        if (pumpkins[i].row - day >= 0 && //up
                                                          //is there empty place to grow                                
                            grid[pumpkins[i].row - day, pumpkins[i].column].derection == Derection.None &&
                            //border from left or (no right direction from left, no center from left)
                            (pumpkins[i].column - 1 < 0 || (grid[pumpkins[i].row - day, pumpkins[i].column - 1].isFinishedDay || (
                                !grid[pumpkins[i].row - day, pumpkins[i].column - 1].isAlive ||
                                (grid[pumpkins[i].row - day, pumpkins[i].column - 1].isAlive &&
                                !grid[pumpkins[i].row - day, pumpkins[i].column - 1].isFinishedDay &&
                                grid[pumpkins[i].row - day, pumpkins[i].column - 1].derection != Derection.Right &&
                                grid[pumpkins[i].row - day, pumpkins[i].column - 1].derection != Derection.Center)))) &&
                            //border from up or (no down direction from up, no center from up)
                            (pumpkins[i].row - day - 1 < 0 || (grid[pumpkins[i].row - day - 1, pumpkins[i].column].isFinishedDay || (
                                !grid[pumpkins[i].row - day - 1, pumpkins[i].column].isAlive ||
                                (grid[pumpkins[i].row - day - 1, pumpkins[i].column].isAlive &&
                                !grid[pumpkins[i].row - day - 1, pumpkins[i].column].isFinishedDay &&
                                grid[pumpkins[i].row - day - 1, pumpkins[i].column].derection != Derection.Down &&
                                grid[pumpkins[i].row - day - 1, pumpkins[i].column].derection != Derection.Center)))) &&
                            //border from right or (no left direction from right, no center from right)
                            (pumpkins[i].column + 1 >= n || (grid[pumpkins[i].row - day, pumpkins[i].column + 1].isFinishedDay || (
                                !grid[pumpkins[i].row - day, pumpkins[i].column + 1].isAlive ||
                                (grid[pumpkins[i].row - day, pumpkins[i].column + 1].isAlive &&
                                !grid[pumpkins[i].row - day, pumpkins[i].column + 1].isFinishedDay &&
                                grid[pumpkins[i].row - day, pumpkins[i].column + 1].derection != Derection.Left &&
                                grid[pumpkins[i].row - day, pumpkins[i].column + 1].derection != Derection.Center)))))
                        {
                            grid[pumpkins[i].row - day, pumpkins[i].column].derection = Derection.Up;
                            grid[pumpkins[i].row - day, pumpkins[i].column].isFinishedDay = true;
                        }
                        else
                        {
                            Death(pumpkins, i, day, grid, n);
                            continue;
                        }

                        if (pumpkins[i].column + day < n && // right
                                                            //is there empty place to grow
                            grid[pumpkins[i].row, pumpkins[i].column + day].derection == Derection.None &&
                            //border from up or (no down direction from up, no center from up)
                            (pumpkins[i].row - 1 < 0 || (grid[pumpkins[i].row - 1, pumpkins[i].column + day].isFinishedDay || (
                                !grid[pumpkins[i].row - 1, pumpkins[i].column + day].isAlive ||
                                (grid[pumpkins[i].row - 1, pumpkins[i].column + day].isAlive &&
                                !grid[pumpkins[i].row - 1, pumpkins[i].column + day].isFinishedDay &&
                                grid[pumpkins[i].row - 1, pumpkins[i].column + day].derection != Derection.Down &&
                                grid[pumpkins[i].row - 1, pumpkins[i].column + day].derection != Derection.Center)))) &&
                            //border from right or (no left direction from left, no center from left)
                            (pumpkins[i].column + day + 1 >= n || (grid[pumpkins[i].row, pumpkins[i].column + day + 1].isFinishedDay || (
                                !grid[pumpkins[i].row, pumpkins[i].column + day + 1].isAlive ||
                                (grid[pumpkins[i].row, pumpkins[i].column + day + 1].isAlive &&
                                !grid[pumpkins[i].row, pumpkins[i].column + day + 1].isFinishedDay &&
                                grid[pumpkins[i].row, pumpkins[i].column + day + 1].derection != Derection.Left &&
                                grid[pumpkins[i].row, pumpkins[i].column + day + 1].derection != Derection.Center)))) &&
                            //border from down or (no up direction from down, no center from down)
                            (pumpkins[i].row + 1 >= n || (grid[pumpkins[i].row + 1, pumpkins[i].column + day].isFinishedDay || (
                                !grid[pumpkins[i].row + 1, pumpkins[i].column + day].isAlive ||
                                (grid[pumpkins[i].row + 1, pumpkins[i].column + day].isAlive &&
                                !grid[pumpkins[i].row + 1, pumpkins[i].column + day].isFinishedDay &&
                                grid[pumpkins[i].row + 1, pumpkins[i].column + day].derection != Derection.Up &&
                                grid[pumpkins[i].row + 1, pumpkins[i].column + day].derection != Derection.Center)))))
                        {
                            grid[pumpkins[i].row, pumpkins[i].column + day].derection = Derection.Right;
                            grid[pumpkins[i].row, pumpkins[i].column + day].isFinishedDay = true;
                        }
                        else
                        {
                            Death(pumpkins, i, day, grid, n);
                            continue;
                        }

                        if (pumpkins[i].row + day < n &&// down
                                                        //is there empty place to grow
                            grid[pumpkins[i].row + day, pumpkins[i].column].derection == Derection.None &&
                            //border from right or (no left direction from right, no center from right)
                            (pumpkins[i].column + 1 >= n || (grid[pumpkins[i].row + day, pumpkins[i].column + 1].isFinishedDay || (
                                !grid[pumpkins[i].row + day, pumpkins[i].column + 1].isAlive ||
                                (grid[pumpkins[i].row + day, pumpkins[i].column + 1].isAlive &&
                                !grid[pumpkins[i].row + day, pumpkins[i].column + 1].isFinishedDay &&
                                grid[pumpkins[i].row + day, pumpkins[i].column + 1].derection != Derection.Left &&
                                grid[pumpkins[i].row + day, pumpkins[i].column + 1].derection != Derection.Center)))) &&
                            //border from down or (no up direction from down, no center from down)
                            (pumpkins[i].row + day + 1 >= n || (grid[pumpkins[i].row + day + 1, pumpkins[i].column].isFinishedDay || (
                                !grid[pumpkins[i].row + day + 1, pumpkins[i].column].isAlive ||
                                (grid[pumpkins[i].row + day + 1, pumpkins[i].column].isAlive &&
                                !grid[pumpkins[i].row + day + 1, pumpkins[i].column].isFinishedDay &&
                                grid[pumpkins[i].row + day + 1, pumpkins[i].column].derection != Derection.Up &&
                                grid[pumpkins[i].row + day + 1, pumpkins[i].column].derection != Derection.Center)))) &&
                            //border from left or (no right direction from left, no center from left)
                            (pumpkins[i].column - 1 < 0 || (grid[pumpkins[i].row + day, pumpkins[i].column - 1].isFinishedDay || (
                                !grid[pumpkins[i].row + day, pumpkins[i].column - 1].isAlive ||
                                (grid[pumpkins[i].row + day, pumpkins[i].column - 1].isAlive &&
                                !grid[pumpkins[i].row + day, pumpkins[i].column - 1].isFinishedDay &&
                                grid[pumpkins[i].row + day, pumpkins[i].column - 1].derection != Derection.Right &&
                                grid[pumpkins[i].row + day, pumpkins[i].column - 1].derection != Derection.Center)))))
                        {
                            grid[pumpkins[i].row + day, pumpkins[i].column].derection = Derection.Down;
                            grid[pumpkins[i].row + day, pumpkins[i].column].isFinishedDay = true;
                        }
                        else
                        {
                            Death(pumpkins, i, day, grid, n);
                            continue;
                        }

                        if (pumpkins[i].column - day >= 0 &&// left
                                                            //is there empty place to grow
                            grid[pumpkins[i].row, pumpkins[i].column - day].derection == Derection.None &&
                            //border from down or (no up direction from down, no center from down)
                            (pumpkins[i].row + 1 >= n || (grid[pumpkins[i].row + 1, pumpkins[i].column - day].isFinishedDay || (
                                !grid[pumpkins[i].row + 1, pumpkins[i].column - day].isAlive ||
                                (grid[pumpkins[i].row + 1, pumpkins[i].column - day].isAlive &&
                                !grid[pumpkins[i].row + 1, pumpkins[i].column - day].isFinishedDay &&
                                grid[pumpkins[i].row + 1, pumpkins[i].column - day].derection != Derection.Up &&
                                grid[pumpkins[i].row + 1, pumpkins[i].column - day].derection != Derection.Center)))) &&
                            //border from left or (no right direction from left, no center from left)
                            (pumpkins[i].column - day - 1 < 0 || (grid[pumpkins[i].row, pumpkins[i].column - day - 1].isFinishedDay || (
                                !grid[pumpkins[i].row, pumpkins[i].column - day - 1].isAlive ||
                                (grid[pumpkins[i].row, pumpkins[i].column - day - 1].isAlive &&
                                !grid[pumpkins[i].row, pumpkins[i].column - day - 1].isFinishedDay &&
                                grid[pumpkins[i].row, pumpkins[i].column - day - 1].derection != Derection.Right &&
                                grid[pumpkins[i].row, pumpkins[i].column - day - 1].derection != Derection.Center)))) &&
                            //border from up or (no down direction from up, no center from up)
                            (pumpkins[i].row - 1 < 0 || (grid[pumpkins[i].row - 1, pumpkins[i].column - day].isFinishedDay || (
                                !grid[pumpkins[i].row - 1, pumpkins[i].column - day].isAlive ||
                                (grid[pumpkins[i].row - 1, pumpkins[i].column - day].isAlive &&
                                !grid[pumpkins[i].row - 1, pumpkins[i].column - day].isFinishedDay &&
                                grid[pumpkins[i].row - 1, pumpkins[i].column - day].derection != Derection.Down &&
                                grid[pumpkins[i].row - 1, pumpkins[i].column - day].derection != Derection.Center)))))
                        {
                            grid[pumpkins[i].row, pumpkins[i].column - day].derection = Derection.Left;
                            grid[pumpkins[i].row, pumpkins[i].column - day].isFinishedDay = true;
                        }
                        else
                        {
                            Death(pumpkins, i, day, grid, n);
                            continue;
                        }

                    }
                }

                //Print(grid, n);
                
                day++;
            }

            foreach (PumpkinInfo pumpkinInfo in pumpkins)
                if (pumpkinInfo.dayDied <= d)
                    Console.WriteLine(pumpkinInfo.dayDied);
                else
                    Console.WriteLine("ALIVE");

            Console.ReadLine();
        }

        static void RootsStartDay(Root[,] grid, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    if (grid[i, j].derection != Derection.None || grid[i, j].isAlive)
                        grid[i, j].isFinishedDay = false;
            }
        }

        static void Death(List<PumpkinInfo> pumpkins, int i, int day, Root[,] grid, int n)
        {
            pumpkins[i].isAlive = false;
            pumpkins[i].dayDied = day;

            for (int j = 1; j <= day; j++)
            {
                if (pumpkins[i].column + j < n && grid[pumpkins[i].row, pumpkins[i].column + j].derection == Derection.Right)
                    grid[pumpkins[i].row, pumpkins[i].column + j].isAlive = false;
                if (pumpkins[i].column - j >= 0 && grid[pumpkins[i].row, pumpkins[i].column - j].derection == Derection.Left)
                    grid[pumpkins[i].row, pumpkins[i].column - j].isAlive = false;
                if (pumpkins[i].row + j < n && grid[pumpkins[i].row + j, pumpkins[i].column].derection == Derection.Down)
                    grid[pumpkins[i].row + j, pumpkins[i].column].isAlive = false;
                if (pumpkins[i].row - j >= 0 && grid[pumpkins[i].row - j, pumpkins[i].column].derection == Derection.Up)
                    grid[pumpkins[i].row - j, pumpkins[i].column].isAlive = false;
            }
        }

        static void Print(Root[,] grid, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write((int)grid[i, j].derection);

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
