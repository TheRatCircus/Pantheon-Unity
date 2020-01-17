// LinePojectile.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public sealed class LineProjectile : MonoBehaviour
    {
        [SerializeField] private string projName = "DEFAULT_LINE_PROJ_NAME";
        [SerializeField] private Damage[] damages = default;
        [SerializeField] private bool spins;
        [SerializeField] private bool pierces;

        private Entity sender;
        private Line line;
        private Vector2Int target;
        private Entity[] debris;

        public void InitializeToss(Entity tosser, Entity entity, Line line)
        {
            GetComponent<SpriteRenderer>().sprite = entity.Flyweight.Sprite;
            projName = entity.ToSubjectString(true);
            sender = tosser;
            this.line = line;
            debris = new Entity[] { entity };

            if (entity.TryGetComponent(out Melee melee))
            {
                damages = melee.Attacks[0].Damages;
            }
            else
            {
                damages = new Damage[1];
                damages[0] = new Damage()
                {
                    // TODO: Base on weight
                    Type = DamageType.Bludgeoning,
                    Min = 1,
                    Max = 3
                };
            }

            pierces = false;
            spins = true;

            target = line[line.Count - 1];
        }

        public void Fire()
        {
            Physics2D.IgnoreCollision(
                sender.GameObjects[0].GetComponent<Collider2D>(),
                GetComponent<Collider2D>());
            Locator.Scheduler.Lock();
            StartCoroutine(Fly());
        } 

        private IEnumerator Fly()
        {
            Vector3 targetPos = target.ToVector3();
            
            // Move to target
            while (transform.position != targetPos)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPos, .3f);

                if (spins)
                    transform.Rotate(0, 0, 8, Space.Self);

                yield return new WaitForSeconds(.01f);
            }

            if (debris != null)
                foreach (Entity e in debris)
                    e.Move(sender.Level, target);

            Locator.Scheduler.Unlock();
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2Int collisionCell;
            EntityWrapper wrapper = collision.gameObject.GetComponent<EntityWrapper>();
            if (wrapper)
            {
                Entity entity = wrapper.Entity;
                collisionCell = entity.Position;

                // Proj can't collide with entity not in the line
                if (!line.Contains(collisionCell))
                    return;

                if (entity == sender)
                    return;

                Hit hit = new Hit(damages);
                Locator.Log.Send(
                    $"{projName} hits {entity.ToSubjectString(false)}!",
                    Color.white);
                entity.TakeHit(sender, hit);
            }
            else return;

            if (!pierces)
            {
                if (debris != null)
                    foreach (Entity e in debris)
                        e.Move(sender.Level, collisionCell);

                Locator.Scheduler.Unlock();
                Destroy(gameObject);
            }
        }
    }
}
