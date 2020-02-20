// Projectile.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Utils;
using System.Collections;
using UnityEngine;

namespace Pantheon
{
    public sealed class Projectile : MonoBehaviour
    {
        public string ProjName { get; set; } = "DEFAULT_PROJ_NAME";
        public Damage[] Damages { get; set; }
        public bool Spins { get; set; }
        public bool Pierces { get; set; }

        public ICellTalentEffect[] OnLandEffects { get; set; }

        public Entity Sender { get; set; }
        public Line Line { get; set; }
        public Vector2Int Target { get; set; }
        public Entity[] Debris { get; set; }

        public void InitializeToss(Entity tosser, Entity tossed, Line line)
        {
            GetComponent<SpriteRenderer>().sprite = tossed.Flyweight.Sprite;
            Line = line;
            // TODO: Some entities may not remain after being tossed
            // e.g. potions and other such fragile things
            Debris = new Entity[] { tossed };

            if (tossed.TryGetComponent(out Melee melee))
            {
                Damages = melee.Attacks[0].Damages;
            }
            else
            {
                Damages = new Damage[1];
                Damages[0] = new Damage()
                {
                    // TODO: Base on weight
                    Type = DamageType.Bludgeoning,
                    Min = 1,
                    Max = 3
                };
            }

            Pierces = false;
            Spins = true;

            Target = line[line.Count - 1];
        }

        public void Fire()
        {
            Physics2D.IgnoreCollision(
                Sender.GameObjects[0].GetComponent<Collider2D>(),
                GetComponent<Collider2D>());
            Locator.Scheduler.Lock();
            StartCoroutine(Fly());
        }

        private IEnumerator Fly()
        {
            Vector3 targetPos = Target.ToVector3();

            // Move to target
            while (transform.position != targetPos)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPos, .3f);

                if (Spins)
                    transform.Rotate(0, 0, 8, Space.Self);

                yield return new WaitForSeconds(.01f);
            }

            Die();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2Int collisionCell;
            EntityWrapper wrapper = collision.gameObject.GetComponent<EntityWrapper>();

            if (!wrapper)
                return;

            Entity entity = wrapper.Entity;
            collisionCell = entity.Cell;

            // Proj can't collide with entity not in the line
            if (!Line.Contains(collisionCell))
                return;

            if (entity == Sender)
                return;

            Hit hit = new Hit(Damages);
            Locator.Log.Send(
                $"{ProjName.FirstCharToUpper()} hits " +
                $"{Strings.Subject(entity, false)}!",
                Color.white);
            entity.TakeHit(Sender, hit);

            if (!Pierces)
                Die();
        }

        private void Die()
        {
            if (Debris != null)
                foreach (Entity e in Debris)
                    e.Move(Sender.Level, Target);

            if (OnLandEffects != null)
                foreach (ICellTalentEffect cte in OnLandEffects)
                    cte?.Affect(Sender, Sender.Level, Target);

            Locator.Scheduler.Unlock();
            Destroy(gameObject);
        }
    }
}
