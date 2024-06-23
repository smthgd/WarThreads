using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarThreads
{
    public partial class MainForm : Form
    {
        private int missCount;
        private int hitCount;
        private int speed = 10;
        private const int MaxBullets = 3;

        private bool isGameStarted = false;

        private static Random random = new Random();

        private readonly Semaphore BulletSemaphore = new Semaphore(MaxBullets, MaxBullets); // семафор для ограничения количества пуль
        private readonly AutoResetEvent startEvent = new AutoResetEvent(false);

        private Mutex screenlock = new Mutex(); // объект блокировки экрана

        public MainForm()
        {
            InitializeComponent();

            Gun.Location = new Point(panelGameSreen.Width / 2, panelGameSreen.Height - 100);

            StartGame();
        }

        private void StartGame()
        {
            missCount = 0;
            hitCount = 0;
            speed = 10;
            isGameStarted = false;

            startEvent.Reset();

            var generateEnemiesThread = new Thread(GenerateEnemies);
            generateEnemiesThread.Start();

            var speedThread = new Thread(IncreaseSpeed);
            speedThread.Start();

            panelGameSreen.Controls.Clear();
            panelGameSreen.Controls.Add(Gun);

        }

        private void GenerateEnemies()
        {
            if (startEvent.WaitOne(15000))
            {
                isGameStarted = true;
            }

            // Создаем случайного врага
            while (isGameStarted)
            {
                if (random.Next(50) < (hitCount + missCount) / 25 + 20)
                {
                    Thread enemyThread = new Thread(CreateEnemy);
                    enemyThread.Start();
                }

                Thread.Sleep(400);
            }
        }

        private void CreateEnemy()
        {
            string enemyPictureName = "Enemy" + random.Next(1, 4).ToString() + ".png";

            // Выбор случайного изображения врага
            Image image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", enemyPictureName));

            var size = new Size(20, 20);
            int numberLives = 1;

            // Случайная координата y
            int y = random.Next(panelGameSreen.Height - 50);

            // Большой корабль
            if (random.Next(100) < 5)
            {
                size = new Size(panelGameSreen.Width / 2, panelGameSreen.Height / 2);
                y = 10;
                numberLives = 1000;
            }

            // Нечетные y появляются слева, четные y появляются справа
            int x = y % 2 != 0 ? 0 : panelGameSreen.Width;

            // Установить направление в зависимости от начальной позиции
            int direction = x == 0 ? 1 : -1;

            PictureBox enemy = new PictureBox
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = size,
                Location = new Point(x, y),
                Tag = "enemy"
            };

            panelGameSreen.Invoke(new Action(() =>
            {
                panelGameSreen.Controls.Add(enemy);
            }));

            // Пока противник находится в пределах экрана
            while ((direction == 1 && x <= panelGameSreen.Width) || (direction == -1 && x >= 0))
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(random.Next(1, 10));

                    foreach (Control control in panelGameSreen.Controls)
                    {
                        // Проверяем столкновение врага с пулей
                        if (control is PictureBox bullet && bullet.Tag.ToString() == "bullet")
                        {
                            if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                            {
                                numberLives--;

                                if (numberLives <= 0)
                                {
                                    HandleHit(enemy);
                                    DeleteBullet(bullet);
                                    return;
                                }
                            }
                        }
                    }
                }

                x += direction * speed;

                // Обновляем положение врага
                enemy.Location = new Point(x, enemy.Location.Y);
            }

            HandleMiss(enemy);
        }

        private void IncreaseSpeed()
        {
            while (isGameStarted)
            {
                Interlocked.Increment(ref speed);
                Thread.Sleep(100);
            }
        }

        private void HandleHit(PictureBox enemy)
        {
            Interlocked.Increment(ref hitCount);

            panelGameSreen.Invoke(new Action(() =>
            {
                panelGameSreen.Controls.Remove(enemy);
            }));

            UpdateScore();
        }

        private void HandleMiss(PictureBox enemy)
        {
            Interlocked.Increment(ref missCount);

            // Удаляем врага с панели
            panelGameSreen.Invoke(new Action(() =>
            {
                panelGameSreen.Controls.Remove(enemy);
            }));

            UpdateScore();

            // Проверка условия завершения игры
            if (missCount >= 30)
            {
                GameOver();
            }
        }

        private void UpdateScore()
        {
            string title = string.Format($"Война потоков - Попаданий: {hitCount}, Промахов: {missCount}");

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    Text = title;
                }));
            }
            else
            {
                Text = title;
            }
        }

        private void DeleteBullet(PictureBox bullet)
        {
            if (bullet.Tag.ToString() == "deleted")
            {
                return;
            }

            // Удаляем пулю с панели
            panelGameSreen.Invoke(new Action(() =>
            {
                panelGameSreen.Controls.Remove(bullet);
            }));

            // Выстрел сделан - добавить 1 к семафору
            BulletSemaphore.Release();

            bullet.Tag = "deleted";
        }

        private void GameOver()
        {
            if (screenlock.WaitOne(0))
            {
                MessageBox.Show($"Вы проиграли! Попаданий: {hitCount}, Промахов: {missCount}", "Война потоков",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
        }

        private void FireBullet()
        {
            // Если семафор равен 0, выстрела не происходит
            if (!BulletSemaphore.WaitOne(0))
            {
                return;
            }
            
            // Отобразить пулю
            PictureBox bullet = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "Images"), "Bullet.png")),
                Size = new Size(15, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(Gun.Location.X + 14, Gun.Location.Y - 10),
                Tag = "bullet"
            };

            // Добавляем пулю на панель
            panelGameSreen.Invoke(new Action(() =>
            {
                panelGameSreen.Controls.Add(bullet);
            }));

            while (bullet.Location.Y >= 0)
            {
                panelGameSreen.Invoke(new Action(() =>
                {
                    bullet.Top -= 10;
                }));
                Thread.Sleep(10);

                if (bullet.Tag.ToString() == "deleted")
                {
                    return;
                }
            }

            DeleteBullet(bullet);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            int step = 20; // Шаг перемещения пушки

            // Проверяем нажатую клавишу и перемещаем пушку соответственно
            if (e.KeyCode == Keys.Left)
            {
                startEvent.Set();

                // Проверяем, чтобы пушка не вышла за пределы левой границы панели
                if (Gun.Left - step >= 0)
                {
                    Gun.Left -= step;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                startEvent.Set();

                // Проверяем, чтобы пушка не вышла за пределы правой границы панели
                if (Gun.Right + step <= panelGameSreen.Width)
                {
                    Gun.Left += step;
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                Thread bulletThread = new Thread(FireBullet);
                bulletThread.Start();
            }
        }
    }
}
