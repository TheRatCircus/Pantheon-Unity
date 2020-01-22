

using UnityEngine;
using System.Collections.Generic;

namespace Pantheon.Core
{
    public interface IAudioManager
    {
        void Buffer(AudioClip clip, Vector3 pos);
    }

    public sealed class AudioManager : MonoBehaviour, IAudioManager
    {
        private HashSet<AudioClip> playedThisFrame = new HashSet<AudioClip>();
        private List<Vector3> bufferedPos = new List<Vector3>();
        private List<AudioClip> bufferedClips = new List<AudioClip>();

        public void Buffer(AudioClip clip, Vector3 pos)
        {
            bufferedClips.Add(clip);
            bufferedPos.Add(pos);
        }

        private void LateUpdate()
        {
            if (bufferedClips.Count < 1)
                return;

            while (bufferedClips.Count > 0)
            {
                // Only play a sound which has not already been played this frame
                if (!playedThisFrame.Contains(bufferedClips[0]))
                {
                    AudioSource.PlayClipAtPoint(bufferedClips[0], bufferedPos[0]);
                    playedThisFrame.Add(bufferedClips[0]);
                }

                bufferedClips.RemoveAt(0);
                bufferedPos.RemoveAt(0);
            }
            playedThisFrame.Clear();
        }
    }
}
