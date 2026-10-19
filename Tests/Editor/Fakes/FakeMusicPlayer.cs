using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RPGFramework.Audio;
using RPGFramework.Audio.Music;
using UnityEngine.Audio;

namespace RPGFramework.ModuleRouter.Tests.Fakes
{
    /// <summary>A music player that plays nothing, writing every call it is asked to a log.</summary>
    internal sealed class FakeMusicPlayer : IMusicPlayer
    {
        private readonly List<string> m_Log;

        internal FakeMusicPlayer()
        {
            m_Log = new List<string>();
        }

        internal string Log => string.Join(", ", m_Log);

        internal void ClearLog()
        {
            m_Log.Clear();
        }

        Task IMusicPlayer.PlayAsync(ulong nameHash, ulong initialStemStateHash, float fadeInTime, float volume)
        {
            m_Log.Add($"play {nameHash}");

            return Task.CompletedTask;
        }

        Task IMusicPlayer.CrossfadeAsync(ulong nameHash, ulong initialStemStateHash, float seconds, float volume)
        {
            m_Log.Add($"crossfade {nameHash}");

            return Task.CompletedTask;
        }

        Task IMusicPlayer.SetSongVolumeAsync(float volume, float seconds)
        {
            m_Log.Add("song volume");

            return Task.CompletedTask;
        }

        MusicSnapshot IMusicPlayer.Pause()
        {
            m_Log.Add("pause");

            return default;
        }

        Task IMusicPlayer.ResumeAsync(MusicSnapshot snapshot, float seconds, float volume)
        {
            m_Log.Add("resume");

            return Task.CompletedTask;
        }

        Task IMusicPlayer.StopAsync(float fadeTime)
        {
            m_Log.Add("stop " + fadeTime.ToString(CultureInfo.InvariantCulture));

            return Task.CompletedTask;
        }

        bool IMusicPlayer.IsPlaying()
        {
            m_Log.Add("is playing");

            return false;
        }

        void IMusicPlayer.SetMusicAssetProvider(IMusicAssetProvider provider)
        {
            m_Log.Add("provider");
        }

        void IMusicPlayer.SetStemMixerGroups(AudioMixerGroup[] groups)
        {
            m_Log.Add("mixer groups");
        }

        Task IMusicPlayer.SetStemStateFadeAsync(ulong stemStateHash, float transitionLength)
        {
            m_Log.Add("stem state");

            return Task.CompletedTask;
        }

        void IMusicPlayer.SetStemStateImmediate(ulong stemStateHash)
        {
            m_Log.Add("stem state");
        }

        float IMusicPlayer.GetVolume()
        {
            m_Log.Add("get volume");

            return 1f;
        }

        void IMusicPlayer.SetVolume(float percent)
        {
            m_Log.Add("set volume");
        }

        void IMusicPlayer.SetReverbPreset(ReverbPreset preset)
        {
            m_Log.Add("reverb preset");
        }

        void IMusicPlayer.SetReverbVolume(float volume)
        {
            m_Log.Add("reverb volume");
        }

        Task IMusicPlayer.PreloadAsync(IReadOnlyList<ulong> nameHashes)
        {
            m_Log.Add("preload");

            return Task.CompletedTask;
        }

        void IMusicPlayer.Unload(IReadOnlyList<ulong> nameHashes)
        {
            m_Log.Add("unload");
        }

        void IDisposable.Dispose()
        {
            m_Log.Add("dispose");
        }
    }
}
