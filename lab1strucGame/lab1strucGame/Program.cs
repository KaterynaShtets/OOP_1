using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace lab1strucGame
{

   


    struct Point
    {
        private int x, y;
        
        // свойство X
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        // свойство Y
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    //структура поля 
    struct Field
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Player player { get; set; }
        public object[,] place;
        
        public Field(int width, int height)
        {
            Width = width;
            Height = height;
            place = new object[width, height];
            player = new Player();
        }

        public void GenerateWall()
        {
            for(int i = 0; i < Width; i++)
            {
                Wall wallUp = new Wall(i, 0);
                Wall wallDown = new Wall(i, Height - 1);
                place[wallUp.point.X, wallUp.point.Y] = wallUp;
                place[wallDown.point.X, wallDown.point.Y] = wallDown;
            }
            for (int i = 0; i < Height; i++)
            {
                Wall wallLeft = new Wall(0, i);
                Wall wallRight = new Wall(Width - 1,i);
                place[wallLeft.point.X, wallLeft.point.Y] = wallLeft;
                place[wallRight.point.X, wallRight.point.Y] = wallRight;
            }
        }
        public void GeneratePlayer(int x, int y,int l,int p)
        {
            player = new Player(x, y, l,p);
            place[player.point.X, player.point.Y] = player;
        }
        public void  Report()
        {
            Console.WriteLine($" У вас осталось {player.Lives} жизней. Осталось собрать  {player.Prizes} призов");
    
        }
        public bool MovePlayer(int x,int y)
        {
                int X = player.point.X;
                int Y = player.point.Y;
                int l = player.Lives;
                int p = player.Prizes;
          
           
            if (place[X + x, Y + y] == null)
                {
                    player = new Player(X + x, Y + y, l,p);
                    place[player.point.X, player.point.Y] = player;
                    place[X, Y] = null;
                return true;
                }
            else if (place[X + x, Y + y].GetType() == typeof(Enemy))
            {
               
                player = new Player(player.point.X + x, player.point.Y + y, l - 1,p);
                place[player.point.X, player.point.Y] = player;
                place[X, Y] = null;
              
                if (player.Lives == 0) { Console.Clear();Console.WriteLine($"У вас {player.Lives}жизней, вы проиграли"); Console.Read(); }
                
                return false;
            }
            else if (place[X + x, Y + y].GetType() == typeof(Prize))
            {
                
                player = new Player(player.point.X + x, player.point.Y + y,l,p-1);
                place[player.point.X, player.point.Y] = player;
                place[X, Y] = null;
                if (player.Prizes == 0) { Console.Clear();Console.WriteLine("Вы собрали все призы"); Console.Read(); }
               
                return false;
            }
            else if (place[X + x, Y + y].GetType() == typeof(BreakPoint))
            {

                player = new Player(player.point.X, player.point.Y, player.Lives, p--);
               place[player.point.X-x, player.point.Y-y] = player;
                place[player.point.X - x, player.point.Y - y] = null;
                return false;
            }
            else
                {
                    return false;
                }
            
        }
        
        public void GenerateEnemies(int count)
        {
            int x, y;
            while(count > 0)
            {
               Random rnd = new Random();
               x = rnd.Next(1, Width);
               y = rnd.Next(1, Height);
                if (place[x, y] == null)
                {
                    place[x, y] = new Enemy(x, y);
                    count--;
                }
                else
                {
                    continue;
                }

            }
      
        }
        public void GeneratePrize(int count)
        {
            int x, y;
            while (count > 0)
            {
                Random rnd = new Random();
                x = rnd.Next(1, Width);
                y = rnd.Next(1, Height);
                if (place[x, y] == null)
                {
                    place[x, y] = new Prize(x, y);
                    count--;
                }
                else
                {
                    continue;
                }

            }

        }
        public void GeneratePoints(int count)
        {
            int x, y;
            while (count > 0)
            {
                Random rnd = new Random();
                x = rnd.Next(1, Width);
                y = rnd.Next(1, Height);
                if (place[x, y] == null)
                {
                    place[x, y] = new BreakPoint(x, y);
                    count--;
                }
                else
                {
                    continue;
                }

            }

        }
    }
    //структура игрока
    struct Player
    {
        public Point point;
        public int Lives { get; set; }
        public int Prizes { get; set; }
        public string Symbol { get; }
        public Player(int x, int y, int lives,int prizes)
        {
            Symbol = "I";
            Prizes = prizes;
            point = new Point(x, y);
            Lives = lives;
        }

    }
    //структура стены
    struct Wall
    {
        public Point point;
      

        public string Symbol { get; }
        public Wall(int x, int y)
        {
            Symbol = "#";
            point = new Point(x, y);
        }
    }
    //структура врага(ЛОВУШКИ)
    struct Enemy
    {
        public Point point;
        public string Symbol { get; }
        public Enemy(int x,int y)
        {
            Symbol = "%";
            point = new Point(x, y);
        }
    }
    //структура приза
    struct Prize
    {
        public Point point;
        public string Symbol { get; }
        public Prize(int x, int y)
        {
            Symbol = "@" +
                "";
            point = new Point(x, y);
        }

    }
    //структура стены
    struct BreakPoint
    {
        public Point point;
        public string Symbol { get; }
        public BreakPoint(int x, int y)
        {
            Symbol = ".";
            point = new Point(x, y);
        }

    }


    class Program
    {
        static void Report1()
        {
            Console.WriteLine($"В этой игре вам нужно собрать все призы которые обозначены как- @.\n " +
                $"У вас есть ловушки обозначеннные - %, \n при попадании на " +
                $"них у вас бцдет отниматься 1 из 3х жизней(если жизней больше нет - проигрыш) \n" +
                $"Так же в игре есть точки остановки - . (при попадании на них игрок останавливается).\n" +
                $"Чтобы выграть вам нужно собрать все призы.");
        }

        static void DrawField(Field field)
        {
            foreach (object v in field.place)
            {
                if (v != null)
                {
                    if (v.GetType() == typeof(Wall))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Wall w = (Wall)v;
                        Console.SetCursorPosition(w.point.X, w.point.Y);
                        Console.WriteLine(w.Symbol);
                    }
                    if (v.GetType() == typeof(Player))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Player p = (Player)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    if (v.GetType() == typeof(Enemy))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Enemy p = (Enemy)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    if (v.GetType() == typeof(Prize))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Prize p = (Prize)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    if (v.GetType() == typeof(BreakPoint))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        BreakPoint p = (BreakPoint)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                }


            }
            
        }

      
        static void Main(string[] args)
        {
            //Console.BackgroundColor = ConsoleColor.White;
         
            //Console.Clear();
            Field field = new Field(20, 10);
            field.GenerateWall();
            field.GeneratePlayer(5, 6, 3, 5);
            field.GeneratePrize(5);
            field.GenerateEnemies(9);
            field.GeneratePoints(9);
            DrawField(field);
            Report1();
            ConsoleKeyInfo keyinfo;
            do
            {
                field.Report();
                keyinfo = Console.ReadKey();
               if(keyinfo.Key == ConsoleKey.A)
               {
                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(-1, 0);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

               }
                if (keyinfo.Key == ConsoleKey.D)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(1, 0);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.W)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(0, -1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.S)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(0, 1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.Q)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(-1, -1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.E)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(1, -1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.Z)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(-1,1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
                if (keyinfo.Key == ConsoleKey.X)
                {

                    bool go = true;
                    while (go)
                    {
                        go = field.MovePlayer(1, 1);
                        Console.Clear();
                        DrawField(field);
                        System.Threading.Thread.Sleep(100);
                    }

                }
            }
            while (keyinfo.Key != ConsoleKey.Enter);
          
            Console.ResetColor();
            Console.ReadLine();

        }

       
    }
    
}
