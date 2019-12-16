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
            string ret = 
                $"{attacker.ToSubjectString(true)} {verb} " +
                $"{defender.ToSubjectString(false)} " +
                $"for {hit.TotalDamage()} damage.";
            return ret;
        }
    }
}
