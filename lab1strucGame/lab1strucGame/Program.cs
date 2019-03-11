using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;

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
        public int Prizes { get; set; }
        public object[,] place;
        
        public Field(int width, int height, int p)
        {
            Width = width;
            Height = height;
            place = new object[width, height];
            player = new Player();
            Prizes = p;
            GeneratePrize(p, width, height);
        }
        public void MinusPrizes()
        {
            Prizes--;
        }
        public bool StopGame()
        {
            if(Prizes == 0)
            {
                return true;
            }
            return false;
        }

        public bool FailGame()
        {
            if(player.Lives == 0)
            {
                return true;
            }
            return false;
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
        public void GeneratePlayer(int x, int y,int l)
        {
            player = new Player(x, y, l);
            place[player.X, player.Y] = player;
        }
        public void Report()
        {
            Console.SetCursorPosition(0, Height + 1);
            Console.WriteLine($" У вас осталось {player.Lives} жизней. Осталось собрать  {Prizes} призов");
    
        }
        public bool MovePlayer(int x,int y)
        {
            int X = player.X;
            int Y = player.Y;
            if (place[player.X + x, player.Y + y] == null)
                {
                player = player.Move(x, y);
                place[player.X, player.Y] = player;
                place[X, Y] = null;
                return true;
                }
            else if (place[player.X + x, player.Y + y].GetType() == typeof(Enemy))
            {

                player = player.Move(x, y);
                player = player.MinusLives();
                place[player.X, player.Y] = player;
                place[X, Y] = null;           
            }
            else if (place[X + x, Y + y].GetType() == typeof(Prize))
            {

                player = player.Move(x, y);
                place[player.X, player.Y] = player;
                place[X, Y] = null;
                MinusPrizes();
                return false;
            }
            else if (place[X + x, Y + y].GetType() == typeof(MedHelp))
            {

                player = player.Move(x, y);
                place[player.X, player.Y] = player;
                player = player.PlusLives();
                place[X, Y] = null;
                return false;
            }
            else if (place[X + x, Y + y].GetType() == typeof(Teleport))
            {
                
                if(player.X + x == 2 && player.Y + y == 2)
                {
                    player = player.Teleport(Width - 3, Height-2);
                    place[player.X, player.Y] = player;
                }
                else if (player.X + x == Width - 2 && player.Y + y == Height - 2)
                {
                    player = player.Teleport(3, 2);
                    place[player.X, player.Y] = player;
                }
                place[X, Y] = null;
                return false;
            }
            else if (place[X + x, Y + y].GetType() == typeof(BreakPoint))
            {
                return false;
            }
            return false;
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
        private void GeneratePrize(int count,int w, int h)
        {
            int x, y;
            while (count > 0)
            {
                Random rnd = new Random();
                x = rnd.Next(1, w - 1);
                y = rnd.Next(1, h - 1);
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
        public void GenerateMedHelp(int count)
        {
            int x, y;
            while (count > 0)
            {
                Random rnd = new Random();
                x = rnd.Next(1, Width);
                y = rnd.Next(1, Height);
                if (place[x, y] == null)
                {
                    place[x, y] = new MedHelp (x, y);
                    count--;
                }
                else
                {
                    continue;
                }
            }
        }
        public void GenerateTeleport()
        {
            int x = 2, y = 2;
            int x1 = Width -2,  y1 = Height - 2;
            place[x, y] = new Teleport(x,y);
            place[x1, y1] = new Teleport(x1,y1);
        }
    }
    //структура игрока
    struct Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Lives { get; set; }
        public string Symbol { get; }
        public Player(int x, int y, int lives)
        {
            X = x;
            Y = y;
            Symbol = "I";
            Lives = lives;
        }
        public Player MinusLives()
        {
            return new Player(X, Y, --Lives);
        }
        public Player PlusLives()
        {
            return new Player(X, Y, ++Lives);
        }
        public Player Move(int x, int y) 
        {
            X += x;
            Y += y;
            return new Player(X, Y, Lives);
        }
        public Player Teleport(int x, int y)
        {
            return new Player(x, y, Lives);
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
            Symbol = "#";
            point = new Point(x, y);
        }

    }
    //стуруктура помощь здоровье
    struct MedHelp
    {
        public Point point;
        public string Symbol { get; }
        public MedHelp(int x,int y)
        {
           Symbol = "M";
            point = new Point(x, y);
        }
       
    }

    struct Teleport
    {
        public Point point;
        public string Symbol { get; }
        public Teleport(int x,int y)
        {
            Symbol = "T";
            point = new Point(x, y);
        }
    }

    //структура помощь время
    //struct TimeHelp
    //{
    //    public Point point;
    //    public string TimeSymbol { get; }
    //    public TimeHelp(int x, int y)
    //    {
    //        TimeSymbol = "C";
    //        point = new Point(x, y);
    //    }
    //}
    class Program
    {
        static void Report1(int y)
        {
            Console.SetCursorPosition(0, y);
            Console.WriteLine($"Призы - @.\nЛовушки  - %,\nТочки остановки - #.\n");
        }
        static void DrawField(Field field, int x, int y)
        {
                var v = field.place[x, y];
                if(v == null)
                {
                    Console.SetCursorPosition(x, y);
                    Console.WriteLine(" ");
                }
                else if (v != null)
                {
                    if (v.GetType() == typeof(Wall))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Wall w = (Wall)v;
                        Console.SetCursorPosition(w.point.X, w.point.Y);
                        Console.WriteLine(w.Symbol);
                    }
                    else if (v.GetType() == typeof(MedHelp))
                    {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    MedHelp w = (MedHelp)v;
                    Console.SetCursorPosition(w.point.X, w.point.Y);
                    Console.WriteLine(w.Symbol);
                    }
                    else if (v.GetType() == typeof(Teleport))
                    {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Teleport w = (Teleport)v;
                    Console.SetCursorPosition(w.point.X, w.point.Y);
                    Console.WriteLine(w.Symbol);
                    }
                    else if (v.GetType() == typeof(Player))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Player p = (Player)v;
                        Console.SetCursorPosition(p.X, p.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    else if (v.GetType() == typeof(Enemy))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Enemy p = (Enemy)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    else if (v.GetType() == typeof(Prize))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Prize p = (Prize)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                    else if (v.GetType() == typeof(BreakPoint))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        BreakPoint p = (BreakPoint)v;
                        Console.SetCursorPosition(p.point.X, p.point.Y);
                        Console.WriteLine(p.Symbol);
                    }
                }
            Console.ResetColor();
        }
        static void Timer()
        {
            for (int a = 80; a >= 0; a--)
            {
                Console.SetCursorPosition(0, 20);
                Console.Write("\rУ вас осталось {0:00}", a);
               
                System.Threading.Thread.Sleep(1000);
                if (a == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Вркмя вышло, вы проиграли");
                   
                }
            }
           
        }
        static void DrawAllField(Field field)
        {
            int rows = field.place.GetUpperBound(0) + 1;
            for (int i = 0; i < rows; i++)
            {
                for(int j = 0; j < field.place.Length / rows; j++)
                {
                    DrawField(field, i,j);
                }
            }               
        }
        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Привет {name}\nДобро пожаловать в игру Inertia.\nСоберите все призы и переходите на новый уровень\nЧтобы начать нажмите Enter");

            Console.ReadKey();
            //Console.BackgroundColor = ConsoleColor.White;
            int steps = 0;
            int level = 1;
            bool lives = true;
            string connectionString = @"Data Source = DESKTOP-OQ106UV\SQLEXPRESS;Initial Catalog=Lab1;Integrated Security=True";
            while (level <= 3 && lives)
            {
                Console.Clear();
                Field field = new Field(25, 12, 5);
                if (level == 1)
                {
                     field = new Field(25, 12, 5);
                    field.GenerateWall();
                    field.GenerateTeleport();
                    field.GeneratePlayer(5, 6, 3);
                    field.GenerateEnemies(3);                  
                    field.GenerateMedHelp(1);
                    field.GeneratePoints(13);
                    DrawAllField(field);

                }
                else if (level == 2)
                {
                    field = new Field(35, 17, 5);
                    field.GenerateWall();
                    field.GeneratePlayer(5, 6, 3);
                    field.GenerateEnemies(15);
                    field.GenerateTeleport();
                    field.GenerateMedHelp(1);
                    field.GeneratePoints(27);
                    DrawAllField(field);

                }
                else if (level == 3)
                {
                    field = new Field(40, 19, 5);
                    field.GenerateWall();
                    field.GeneratePlayer(5, 6, 3);
                    field.GenerateTeleport();
                    field.GenerateEnemies(20);
                    field.GenerateMedHelp(1);
                    field.GeneratePoints(30);
                    DrawAllField(field);

                }
                ConsoleKeyInfo keyinfo;
                Thread myThread = new Thread(new ThreadStart(Timer));
                myThread.Start();
                int count = 0;
                while (!field.StopGame())
                {
                    Report1(field.Height + 3);
                    field.Report();
                    keyinfo = Console.ReadKey();
                    if (keyinfo.Key == ConsoleKey.A)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(-1, 0);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.D)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(1, 0);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.W)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(0, -1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.S)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(0, 1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.Q)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(-1, -1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.E)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(1, -1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if(keyinfo.Key == ConsoleKey.Z)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(-1, 1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    else if (keyinfo.Key == ConsoleKey.X)
                    {
                        count++;
                        bool go = true;
                        while (go)
                        {
                            int x = field.player.X;
                            int y = field.player.Y;
                            go = field.MovePlayer(1, 1);
                            DrawField(field, field.player.X, field.player.Y);
                            DrawField(field, x, y);
                            System.Threading.Thread.Sleep(60);
                        }

                    }
                    Console.SetCursorPosition(44, 10);
                    Console.Write($"Вы совершили шагов:{ count}");
                    if (field.FailGame())
                    {
                        lives = false;
                        break;
                    }
                    
                }
                             
                Console.ResetColor();
                myThread.Abort();
                level++;
                steps += count;

            }
            if(lives == false)
            {
                Console.Clear();
                Console.WriteLine("Вы проиграли - закончились жизни!");
                Console.WriteLine("");
                Console.WriteLine("Статистика игр: ");
                string insert = String.Format("INSERT INTO Games (Name, Steps, State) VALUES ('{0}', {1}, '{2}')", name, steps, "Fail");
                string select = "Select * from Games";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(insert, connection);//создаем объект комманда
                    //выполняем
                    command.ExecuteNonQuery();

                    SqlCommand command1 = new SqlCommand(select, connection);
                    SqlDataReader reader = command1.ExecuteReader();//читаем данные

                    while (reader.Read())
                    {
                        object login = reader["Name"];
                        object stp = reader["Steps"];
                        object state = reader["State"];
                        Console.WriteLine("Name: {0} \t Steps: {1} \t State: {2}", login, stp, state);
                    }
                    connection.Close();
                }

                Console.ReadLine();
            }
            else
            {               
                
                Console.Clear();
                Console.WriteLine("Ура, Вы победили!");
                Console.WriteLine("");
                Console.WriteLine("Статистика игр: ");
                string insert = String.Format("INSERT INTO Games (Name, Steps, State) VALUES ('{0}', {1}, '{2}')", name, steps, "Success");
                string select = "Select * from Games";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(insert, connection);//создаем объект комманда
                    //выполняем
                    command.ExecuteNonQuery();

                    SqlCommand command1 = new SqlCommand(select, connection);
                    SqlDataReader reader = command1.ExecuteReader();//читаем данные

                    while (reader.Read())
                    {
                        object login = reader["Name"];
                        object stp = reader["Steps"];
                        object state = reader["State"];
                        Console.WriteLine("Name: {0} \t Steps: {1} \t State: {2}", login, stp, state);
                    }
                    connection.Close();
                }
                

                Console.ReadLine();
            }

        }

       
    }
    
}
