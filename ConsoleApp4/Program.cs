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
