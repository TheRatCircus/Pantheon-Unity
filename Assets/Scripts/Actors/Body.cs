// Body.cs
// Jerome Martina

using Pantheon.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// Storage and functions for actor appendages.
    /// </summary>
    [System.Serializable]
    public sealed class Body
    {
        [SerializeField] private List<Appendage> parts;

        public List<Appendage> Parts { get => parts; }

        public void Initialize()
        {
            foreach (Appendage app in parts)
                app.Initialize();
        }

        /// <summary>
        /// Get the average move speed of all appendages which have any.
        /// </summary>
        /// <returns></returns>
        public int GetMoveTime()
        {
            IEnumerable<Appendage> aQuery = from app in parts
                                           where app.Type == AppendageType.Legs
                                           select app;
            
            // If this actor has no walking appendages, it crawls at 200
            if (aQuery.Count() == 0)
                return 200;

            // Otherwise get average of all speeds
            int sum = 0;

            foreach (Appendage app in aQuery)
                sum += app.MoveSpeed;

            return sum / aQuery.Count();
        }

        // Check if this actor has any prehensile appendages
        public bool HasPrehensile()
        {
            foreach (Appendage app in parts)
                if (app.Prehensile) return true;

            return false;
        }

        public List<Appendage> GetPrehensiles()
        {
            List<Appendage> prehensiles = new List<Appendage>();

            foreach (Appendage app in parts)
                if (app.Prehensile)
                    prehensiles.Add(app);

            return prehensiles;
        }

        /// <summary>
        /// Get all the melee attacks this actor can possibly perform.
        /// </summary>
        /// <returns></returns>
        public List<Melee> GetMelees()
        {
            List<Melee> melees = new List<Melee>();

            foreach (Appendage app in parts)
            {
                if (app.Item != null)
                    melees.Add(app.Item.Melee);
                else if (app.CanMelee && app.Dexterous)
                    melees.Add(app.Melee);
                else
                    continue;
            }

            if (melees.Count == 0)
                return null;
            else
                return melees;
        }

        /// <summary>
        ///  Check if the cumulative strength of the actor's prehensiles used to
        ///  wield an item meet that item's strength requirement.
        /// </summary>
        /// <param name="req">The strength requirement checked against.</param>
        /// <returns>True if the actor has enough strength over all its prehensiles.</returns>
        public bool MeetsStrengthReq(Item item)
        {
            if (item.StrengthReq == 0)
                return true;

            int wieldStrength = 0;

            List<Appendage> prehensiles = GetPrehensiles();
            foreach (Appendage prehensile in prehensiles)
                if (prehensile.Item == item)
                    wieldStrength += prehensile.Strength;

            if (wieldStrength >= item.StrengthReq)
                return true;
            else
                return false;
        }
    }
}
