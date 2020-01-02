// Strings.cs
// Jerome Martina

namespace Pantheon.Util
{
    public static class Strings
    {
        public static string Miss(Entity attacker, Entity defender)
        {
            // 3rd person: "misses", 1st/2nd person: "miss"
            string verb = attacker.ThirdPerson ? "misses" : "miss";
            string ret = 
                $"{attacker.ToSubjectString(true)} " +
                $"{verb} {defender.ToSubjectString(false)}.";
            return ret;
        }

        public static string Hit(Entity attacker, Entity defender, Hit hit)
        {
            string verb = attacker.ThirdPerson ? "hits" : "hit";
            return
                $"{attacker.ToSubjectString(true)} {verb} " +
                $"{defender.ToSubjectString(false)} " +
                $"for {hit.TotalDamage()} damage.";
        }

        public static string Kill(Entity killer, Entity killed)
        {
            if (killer == null)
                return $"{killed.ToSubjectString(true)} is killed!";

            string verb = killer.ThirdPerson ? "kills" : "kill";
            return
                $"{killer.ToSubjectString(true)} {verb} " +
                $"{killed.ToSubjectString(false)}!";
        }

        public static string Accelerate(Entity entity)
        {
            return entity.PlayerControlled ? "accelerate" : "accelerates";
        }

        public static string Slow(Entity entity)
        {
            return entity.PlayerControlled ? "slow" : "slows";
        }
    }
}
