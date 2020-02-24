// Verbs.cs
// Jerome Martina

using Pantheon.Components.Entity;

namespace Pantheon.Utils
{
    public static class Verbs
    {
        // TODO: Some functions return a whole phrase, others only a word.
        // This class needs to be made consistent; all should only return a word.

        public static string Be(Entity entity)
        {
            // TODO: 1st person
            // 2nd person "are", 3rd person "is"
            return 
                $"{Strings.Subject(entity, true)} " +
                $"{(Actor.PlayerControlled(entity) ? "are" : " is ")}";
        }

        public static string Miss(Entity attacker, Entity defender)
        {
            // 3rd person: "misses", 1st/2nd person: "miss"
            string verb = Actor.PlayerControlled(attacker) ? "miss" : "misses";
            string ret =
                $"{Strings.Subject(attacker, true)} " +
                $"{verb} {Strings.Subject(defender, false)}.";
            return ret;
        }

        public static string Hit(Entity attacker, Entity defender, Hit hit)
        {
            string verb = Actor.PlayerControlled(attacker) ? "hit" : "hits";
            return
                $"{Strings.Subject(attacker, true)} {verb} " +
                $"{Strings.Subject(defender, false)} " +
                $"for {hit.TotalDamage()} damage.";
        }

        public static string Kill(Entity killer, Entity killed)
        {
            // TODO: Support for self
            if (killer == null)
                return $"{Strings.Subject(killed, true)} is killed!";

            string verb = Actor.PlayerControlled(killer) ? "kill" : "kills";
            return
                $"{Strings.Subject(killer, true)} {verb} " +
                $"{Strings.Subject(killed, false)}!";
        }

        public static string Accelerate(Entity entity)
        {
            return Actor.PlayerControlled(entity) ? "accelerate" : "accelerates";
        }

        public static string Slow(Entity entity)
        {
            return Actor.PlayerControlled(entity) ? "slow" : "slows";
        }

        public static string Swing(Entity entity)
        {
            return Actor.PlayerControlled(entity) ? "swing" : "swings";
        }

        public static string Blink(Entity entity)
        {
            return 
                $"{Strings.Subject(entity, true)} " +
                $"{(Actor.PlayerControlled(entity) ? "blink" : "blinks")}.";
        }
    }
}
