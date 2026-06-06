using UnityEngine;

public static class SoundDataUtility
{
    public static class KeyConfig
    {
        public static class Se
        {
            public static readonly string Shoot = "Shoot";
            public static readonly string Buff = "Buff";
            public static readonly string Debuff = "Debuff";
            public static readonly string MagicMove = "MagicMove";
        }

        public static class Bgm
        {
            public static readonly string InGame = "InGame";
        }
    }

    public enum SoundType
    {
        Bgm = 0,
        Se = 1
    }

    public static void PrepareAudioSource(this AudioSource source, SoundData soundData)
    {
        source.playOnAwake = soundData.PlayOnAwake;
        source.volume = soundData.Value;
        source.loop = soundData.IsLoop;
        source.clip = soundData.Clip;
    }
}
