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
 