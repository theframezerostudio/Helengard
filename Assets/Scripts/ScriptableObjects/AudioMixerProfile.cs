using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New Audio Mixer Profile", menuName = "Audio/Mixer Profile")]
public class AudioMixerProfile : ScriptableObject
{
    [field: SerializeField] public string ID {  get; private set; }
    [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
    [field: SerializeField] public int MaxLimit { get; private set; }
}
