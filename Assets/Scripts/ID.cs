// ID.cs
// Jerome Martina

namespace Pantheon
{
    /// <summary>
    /// String constants for content object IDs.
    /// </summary>
    public static class ID
    {
        public static class Item
        {
            public static readonly string _hatchet = "Item_Hatchet";
            public static readonly string _carbine = "Item_Carbine";
            public static readonly string _dagger = "Item_Dagger";
            public static readonly string _flask = "Item_Flask";
            public static readonly string _cartridges = "Item_Cartridges";
            public static readonly string _cuirass = "Item_Cuirass";
            public static readonly string _handGrenade = "Item_HandGrenade";
            public static readonly string _laudanum = "Item_Laudanum";
        }

        public static class NPC
        {
            public static readonly string _ragingGoose = "NPC_RagingGoose";
            public static readonly string _coyote = "NPC_Coyote";
            public static readonly string _giantBumblebee = "NPC_GiantBumblebee";
        }

        public static class Species
        {
            public static readonly string _human = "Species_Human";
            public static readonly string _swine = "Species_Swine";
            public static readonly string _goose = "Species_Goose";
            public static readonly string _pigman = "Species_Pigman";
            public static readonly string _coyote = "Species_Coyote";
            public static readonly string _giantBumblebee = "Species_GiantBumblebee";
        }

        public static class Terrain
        {
            public static readonly string _stoneWall = "Terrain_StoneWall";
            public static readonly string _stoneFloor = "Terrain_StoneFloor";
            public static readonly string _grass = "Terrain_Grass";
            public static readonly string _marbleTile = "Terrain_MarbleTile";
        }

        public static class Feature
        {
            public static readonly string _stoneStairsUp = "Feat_StoneStairsUp";
            public static readonly string _stoneStairsDown = "Feat_StoneStairsDown";
            public static readonly string _tree = "Feat_Tree";
            public static readonly string _trailNorth = "Feat_TrailNorth";
            public static readonly string _trailEast = "Feat_TrailEast";
            public static readonly string _trailSouth = "Feat_TrailSouth";
            public static readonly string _trailWest = "Feat_TrailWest";
            public static readonly string _portal = "Feat_Portal";
            public static readonly string _altarCrystal = "Feat_AltarCrystal";
            public static readonly string _woodFence = "Feat_WoodFence";
        }

        public static class Spell
        {
            public static readonly string _patsons = "Spell_Patsons";
        }

        public static class Landmark
        {
            public static readonly string _keep = "Landmark_Keep";
        }

        public static class Aspect
        {
            public static readonly string _fire = "Aspect_Fire";
            public static readonly string _greed = "Aspect_Greed";
            public static readonly string _swine = "Aspect_Swine";
            public static readonly string _war = "Aspect_War";
        }

        public static class Occupation
        {
            public static readonly string _axeman = "Occ_Axeman";
        }
    }
}
