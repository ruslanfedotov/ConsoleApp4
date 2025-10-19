using System;
using System.Collections.Generic;

namespace TextRPG
{
    // Перечисление типов врагов
    public enum EnemyType
    {
        Goblin,
        Skeleton,
        Mage,
        Boss
    }

    // Базовый класс для предметов
    public abstract class Item
    {
        public string Name { get; protected set; }
        public int Value { get; protected set; }

        protected Item(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public abstract void Use(Player player);
    }

    // Класс оружия
    public class Weapon : Item
    {
        public int Attack { get; private set; }

        public Weapon(string name, int value, int attack) : base(name, value)
        {
            Attack = attack;
        }

        public override void Use(Player player)
        {
            player.EquipWeapon(this);
        }

        public override string ToString()
        {
            return $"{Name} (Атака: {Attack})";
        }
    }

    // Класс доспехов
    public class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(string name, int value, int defense) : base(name, value)
        {
            Defense = defense;
        }

        public override void Use(Player player)
        {
            player.EquipArmor(this);
        }

        public override string ToString()
        {
            return $"{Name} (Защита: {Defense})";
        }
    }

    // Класс зелья здоровья
    public class HealthPotion : Item
    {
        public HealthPotion() : base("Зелье здоровья", 15) { }

        public override void Use(Player player)
        {
            player.Heal();
            Console.WriteLine("Вы выпили зелье здоровья и полностью восстановили HP!");
        }

        public override string ToString()
        {
            return $"{Name} (Полное восстановление здоровья)";
        }
    }

    // Класс врага
    public class Enemy
    {
        public string Name { get; protected set; }
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public EnemyType Type { get; protected set; }

        public double CritChance { get; set; }
        public double FreezeChance { get; set; }
        public bool IgnoreDefense { get; set; }

        protected Random random;

        public Enemy(string name, int hp, int attack, int defense, EnemyType type)
        {
            Name = name;
            MaxHP = hp;
            HP = hp;
            Attack = attack;
            Defense = defense;
            Type = type;
            random = new Random();

            // Установка особенностей по типу
            switch (type)
            {
                case EnemyType.Goblin:
                    CritChance = 0.2;
                    break;
                case EnemyType.Skeleton:
                    IgnoreDefense = true;
                    break;
                case EnemyType.Mage:
                    FreezeChance = 0.25;
                    break;
            }
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }

        public int CalculateDamage(Player player)
        {
            int damage = Attack;

            // Критический удар
            if (random.NextDouble() < CritChance)
            {
                damage = (int)(damage * 1.5);
                Console.WriteLine($"{Name} наносит критический удар!");
            }

            return damage;
        }

        public bool TryFreezePlayer()
        {
            return random.NextDouble() < FreezeChance;
        }

        public bool IsAlive => HP > 0;

        public string GetStatus()
        {
            return $"{Name} - HP: {HP}/{MaxHP}, Атака: {Attack}, Защита: {Defense}";
        }
    }

    // Фабрика для создания врагов
    public static class EnemyFactory
    {
        private static Random random = new Random();

        public static Enemy CreateRegularEnemy()
        {
            var type = (EnemyType)random.Next(0, 3);

            switch (type)
            {
                case EnemyType.Goblin:
                    return new Enemy("Гоблин", 30, 8, 3, EnemyType.Goblin);
                case EnemyType.Skeleton:
                    return new Enemy("Скелет", 25, 10, 2, EnemyType.Skeleton);
                case EnemyType.Mage:
                    return new Enemy("Маг", 20, 12, 1, EnemyType.Mage);
                default:
                    return new Enemy("Гоблин", 30, 8, 3, EnemyType.Goblin);
            }
        }

        public static Enemy CreateBoss()
        {
            var bossType = random.Next(0, 4);

            switch (bossType)
            {
                case 0:
                    var maximoleg = new Enemy("Максимолегович (Босс Гоблин)", 60, 12, 4, EnemyType.Goblin);
                    maximoleg.CritChance = 0.3;
                    return maximoleg;
                case 1: 
                    return new Enemy("Овал (Босс Скелет)", 63, 13, 3, EnemyType.Skeleton);
                case 2: 
                    var hello = new Enemy("Привет (Босс Маг)", 36, 19, 1, EnemyType.Mage);
                    hello.FreezeChance = 0.35;
                    return hello;
                case 3:
                    var sss = new Enemy("ССС (Босс Скелет-Маг)", 33, 18, 1, EnemyType.Skeleton);
                    sss.FreezeChance = 0.4;
                    return sss;
                default:
                    return new Enemy("Максимолегович (Босс Гоблин)", 60, 12, 4, EnemyType.Goblin);
            }
        }
    }

    // Класс игрока
    public class Player
    {
        public int HP { get; private set; }
        public int MaxHP { get; private set; }
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }
        public bool IsFrozen { get; set; }
        public int Attack => Weapon?.Attack ?? 0;
        public int Defense => Armor?.Defense ?? 0;

        private Random random;

        public Player(int maxHP)
        {
            MaxHP = maxHP;
            HP = maxHP;
            random = new Random();

            // Стартовое снаряжение
            Weapon = new Weapon("говно", 100, 100);
            Armor = new Armor("нет брони", 100, 100);
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }

        public void Heal()
        {
            HP = MaxHP;
        }

        public void EquipWeapon(Weapon weapon)
        {
            Weapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            Armor = armor;
        }

        public bool TryDodge()
        {
            return random.NextDouble() < 0.4; // 40% шанс уклонения
        }

        public int CalculateBlockedDamage(int incomingDamage)
        {
            double blockPercentage = 0.7 + (random.NextDouble() * 0.3); // 70-100% защиты
            int blockedDamage = (int)(Defense * blockPercentage);
            return Math.Max(0, incomingDamage - blockedDamage);
        }

        public string GetStatus()
        {
            return $"Игрок - HP: {HP}/{MaxHP}, Атака: {Attack}, Защита: {Defense}";
        }

        public string GetEquipment()
        {
            return $"Оружие: {Weapon}\nДоспехи: {Armor}";
        }
    }

    // Главный класс игры
    public class Game
    {
        private Player player;
        private Random random;
        private int turnCount;

        // Списки предметов
        private List<Weapon> weapons = new List<Weapon>
        {
            new Weapon("меч просто", 10, 5),
            new Weapon("круче меч", 20, 8),
            new Weapon("лук", 25, 10),
            new Weapon("Магический посох", 30, 12),
            new Weapon("суперпупер мэч", 50, 15)
        };

        private List<Armor> armors = new List<Armor>
        {
            new Armor("броня", 10, 3),
            new Armor("Кольчуга", 20, 5),
            new Armor("оспехи", 30, 8),
            new Armor("штаны крутые", 25, 6),
            new Armor("мегапупер броня", 50, 12)
        };

        public Game()
        {
            player = new Player(100);
            random = new Random();
            turnCount = 0;
        }

        public void Start()
        {
            Console.WriteLine("Добро пожаловать в текстовую RPG игру!");
            Console.WriteLine("Каждый ход вы будете встречать либо сундук, либо врага.");
            Console.WriteLine("Каждые 10 ходов вас ждет встреча с боссом!\n");

            while (player.HP > 0)
            {
                turnCount++;
                Console.WriteLine($"\n=== Ход {turnCount} ===");
                Console.WriteLine(player.GetStatus());

                if (player.IsFrozen)
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход!");
                    player.IsFrozen = false;
                    ContinueGame();
                    continue;
                }

                // Случайное событие: 50% сундук, 50% враг
                if (random.Next(2) == 0)
                {
                    OpenChest();
                }
                else
                {
                    FightEnemy();
                }

                // Каждые 10 ходов - босс
                if (turnCount % 10 == 0)
                {
                    Console.WriteLine("\n⚠️ Приближается босс! ⚠️");
                    FightBoss();
                }

                if (player.HP > 0)
                {
                    ContinueGame();
                }
            }

            Console.WriteLine("\n=== Игра окончена ===");
            Console.WriteLine($"Вы продержались {turnCount} ходов!");
        }

        private void OpenChest()
        {
            Console.WriteLine("Вы нашли сундук!");

            // Случайный предмет
            Item item = random.Next(3) switch
            {
                0 => new HealthPotion(),
                1 => weapons[random.Next(weapons.Count)],
                2 => armors[random.Next(armors.Count)],
                _ => new HealthPotion()
            };

            Console.WriteLine($"В сундуке: {item}");

            if (item is HealthPotion)
            {
                item.Use(player);
            }
            else
            {
                Console.WriteLine("\nВаша текущая экипировка:");
                Console.WriteLine(player.GetEquipment());
                Console.WriteLine("\nХотите взять этот предмет? (д/н)");

                if (Console.ReadLine().ToLower() == "д")
                {
                    item.Use(player);
                    Console.WriteLine("Предмет экипирован!");
                }
                else
                {
                    Console.WriteLine("Вы выбросили предмет.");
                }
            }
        }

        private void FightEnemy()
        {
            var enemy = EnemyFactory.CreateRegularEnemy();
            Console.WriteLine($"Вы встретили {enemy.Name}!");
            StartCombat(enemy);
        }

        private void FightBoss()
        {
            var boss = EnemyFactory.CreateBoss();
            Console.WriteLine($"Перед вами {boss.Name}!");
            StartCombat(boss);
        }

        private void StartCombat(Enemy enemy)
        {
            Console.WriteLine($"\n=== Бой с {enemy.Name} ===");

            while (enemy.IsAlive && player.HP > 0)
            {
                // Ход игрока
                PlayerTurn(enemy);
                if (!enemy.IsAlive) break;

                // Ход врага
                EnemyTurn(enemy);
            }

            if (!enemy.IsAlive)
            {
                Console.WriteLine($"\nПобеда! Вы победили {enemy.Name}!");
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            Console.WriteLine("\nВаш ход:");
            Console.WriteLine("1 - Атаковать");
            Console.WriteLine("2 - Защищаться");
            Console.Write("Выберите действие: ");

            switch (Console.ReadLine())
            {
                case "1":
                    int damage = player.Attack;
                    enemy.TakeDamage(damage);
                    Console.WriteLine($"Вы нанесли {damage} урона {enemy.Name}!");
                    Console.WriteLine($"{enemy.GetStatus()}");
                    break;
                case "2":
                    Console.WriteLine("Вы готовитесь к защите...");
                    break;
                default:
                    Console.WriteLine("Неверный выбор, вы пропускаете ход!");
                    break;
            }
        }

        private void EnemyTurn(Enemy enemy)
        {
            Console.WriteLine($"\nХод {enemy.Name}:");

            int damage = enemy.CalculateDamage(player);
            int finalDamage = damage;

            // Проверка защиты игрока
            if (player.TryDodge())
            {
                Console.WriteLine("Вы полностью уклонились от атаки!");
            }
            else if (!enemy.IgnoreDefense)
            {
                finalDamage = player.CalculateBlockedDamage(damage);
                Console.WriteLine($"{enemy.Name} наносит {finalDamage} урона! (Исходный урон: {damage})");
            }
            else
            {
                Console.WriteLine($"{enemy.Name} игнорирует вашу защиту и наносит {finalDamage} урона!");
            }

            player.TakeDamage(finalDamage);

            // Проверка заморозки
            if (enemy.TryFreezePlayer())
            {
                player.IsFrozen = true;
                Console.WriteLine($"{enemy.Name} замораживает вас! Вы пропустите следующий ход.");
            }

            Console.WriteLine($"{player.GetStatus()}");
        }

        private void ContinueGame()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    // Главная программа
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}