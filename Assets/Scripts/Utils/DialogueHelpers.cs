// Actor.cs
// Jerome Martina

#define DEBUG_ACTOR
#undef DEBUG_ACTOR

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class DialogueHelpers
    {
        public static string GetIdleDialogue(string id)
        {
            string ret = null;
            TextAsset ta = Assets.Dialogue[id];
            using (StringReader reader = new StringReader(ta.text))
            {
                while (ret != "%%% Idle")
                    ret = reader.ReadLine();

                // Read past empty line
                reader.ReadLine();

                List<string> options = new List<string>();

                // Read until next empty line
                string s;
                while ((s = reader.ReadLine()) != "")
                    options.Add(s);

                ret = options.Random();
            }
            return ret;
        }
    }
}
