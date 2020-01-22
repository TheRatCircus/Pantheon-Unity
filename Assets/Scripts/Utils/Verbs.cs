// Verbs.cs
// Jerome Martina

using Pantheon.Components.Entity;

namespace Pantheon.Utils
{
    public static class Verbs
    {
        public static string Miss(Entity attacker, Entity defender)
        {
            // 3rd person: "misses", 1st/2nd person: "miss"
            string verb = Actor.PlayerControlled(attacker) ? "miss" : "misses";
            string ret =
                $"{attacker.ToSubjectString(true)} " +
                $"{verb} {defender.ToSubjectString(false)}.";
            return ret;
        }

        public static string Hit(Entity attacker, Entity defender, Hit hit)
        {
            string verb = Actor.PlayerControlled(attacker) ? "hit" : "hits";
            return
                $"{attacker.ToSubjectString(true)} {verb} " +
                $"{defender.ToSubjectString(false)} " +
                $"for {hit.TotalDamage()} damage.";
        }

        public static string Kill(Entity killer, Entity killed)
        {
            if (killer == null)
                return $"{killed.ToSubjectString(true)} is killed!";

            string verb = Actor.PlayerControlled(killer) ? "kill" : "kills";
            return
                $"{killer.ToSubjectString(true)} {verb} " +
                $"{killed.ToSubjectString(false)}!";
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
    }
}
