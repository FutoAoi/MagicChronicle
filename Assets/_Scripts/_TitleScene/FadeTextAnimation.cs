using DG.Tweening;
using TMPro;
using UnityEngine;

public class FadeTextAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _text.DOFade(0.1f, _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
