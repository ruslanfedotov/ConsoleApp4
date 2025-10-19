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

  