﻿using System;

namespace Napilnik
{
    class Program
    {
        private const int PlayerHealth = 100;
        private const int WeaponDamage = 20;
        private const int MaxWeaponAmmo = 10;

        static void Main(string[] args)
        {
            var player = new Player(PlayerHealth);
            var weapon = new Weapon(WeaponDamage, MaxWeaponAmmo);
            var bot = new Bot(weapon);
            bot.OnSeePlayer(player);
        }
    }

    public class Weapon
    {
        public int Damage { get; private set; }
        public int Ammo { get; private set; }
        public int MaxAmmo { get; private set; }

        public event Action OnFire;

        public Weapon(int damage, int maxAmmo)
        {
            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }

            if (maxAmmo <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAmmo));
            }

            Damage = damage;
            Ammo = maxAmmo;
        }

        public void Fire(IDamageable damageable)
        {
            if (!CanFire())
            {
                return;
            }

            damageable.TakeDamage(Damage);
            Ammo -= 1;

            OnFire?.Invoke();
        }

        public void Reload()
        {
            Ammo = MaxAmmo;
        }
        public bool CanFire()
        {
            return Ammo > 0;
        }
    }

    public class Player : IDamageable
    {
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

        public event Action OnDie;
        public event Action OnTakeDamage;

        public Player(float maxHealth)
        {
            if (maxHealth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHealth));
            }

            MaxHealth = maxHealth;
            Health = MaxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage));
            }

            Health = Math.Clamp(Health - damage, 0, MaxHealth);

            OnTakeDamage?.Invoke();

            if (Health == 0)
            {
                OnDie?.Invoke();
            }
        }
    }

    public class Bot
    {
        public Weapon Weapon { get; private set; }

        public Bot(Weapon weapon)
        {
            if (Weapon == null)
            {
                throw new ArgumentNullException(nameof(weapon));
            }

            Weapon = weapon;
        }

        public void OnSeePlayer(IDamageable damageable)
        {
            if (damageable == null)
            {
                throw new ArgumentNullException(nameof(damageable));
            }

            if (Weapon.CanFire())
            {
                Weapon.Fire(damageable);
            }
            else
            {
                Weapon.Reload();
            }
        }
    }

    public interface IDamageable
    {
        public void TakeDamage(float damage);
    }
}