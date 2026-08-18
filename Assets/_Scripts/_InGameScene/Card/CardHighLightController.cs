using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHighLightController : MonoBehaviour
{
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private Color _start;
    [SerializeField] private Color _end;

    private static readonly List<Image> _targets = new();
    public static void Register(Image img) => _targets.Add(img);
    public static void Unregister(Image img) => _targets.Remove(img);

    private void Update()
    {
        float t = Mathf.PingPong(Time.time, _duration) / _duration;
        Color c = Color.Lerp(_start, _end, t);
        foreach (var img in _targets)
            img.color = c;
    }
}
