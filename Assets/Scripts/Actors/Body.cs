// Body.cs
// Jerome Martina

using Pantheon.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// Storage and functions for actor body parts.
    /// </summary>
    [System.Serializable]
    public class Body
    {
        [SerializeField] protected List<BodyPart> parts;

        public List<BodyPart> Parts { get => parts; }

        public void Initialize()
        {
            foreach (BodyPart part in parts)
                part.Initialize();
        }

        /// <summary>
        /// Get the average move speed of all parts which have any.
        /// </summary>
        /// <returns></returns>
        public int GetMoveTime()
        {
            IEnumerable<BodyPart> aQuery = from app in parts
                                           where app.Type == AppendageType.Legs
                                           select app;
            
            // If this actor has no walking appendages, it crawls at 200
            if (aQuery.Count() == 0)
                return 200;

            // Otherwise get average of all speeds
            int sum = 0;

            foreach (BodyPart app in aQuery)
                sum += app.MoveSpeed;

            return sum / aQuery.Count();
        }

        // Check if this actor has any prehensile body parts
        public bool HasPrehensile()
        {
            foreach (BodyPart part in parts)
                if (part.Prehensile) return true;

            return false;
        }

        public List<BodyPart> GetPrehensiles()
        {
            List<BodyPart> prehensiles = new List<BodyPart>();

            foreach (BodyPart part in parts)
                if (part.Prehensile)
                    prehensiles.Add(part);

            return prehensiles;
        }

        /// <summary>
        /// Get all the melee attacks this actor can possibly perform.
        /// </summary>
        /// <returns></returns>
        public List<Melee> GetMelees()
        {
            List<Melee> melees = new List<Melee>();

            foreach (BodyPart part in parts)
            {
                if (part.Item != null)
                    melees.Add(part.Item.Melee);
                else if (part.CanMelee && part.Dexterous)
                    melees.Add(part.Melee);
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

            List<BodyPart> prehensiles = GetPrehensiles();
            foreach (BodyPart prehensile in prehensiles)
                if (prehensile.Item == item)
                    wieldStrength += prehensile.Strength;

            if (wieldStrength >= item.StrengthReq)
                return true;
            else
                return false;
        }
    }
}