using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardHighLightAnimation : MonoBehaviour
{
    [SerializeField] private Image _img;
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private Color _start;
    [SerializeField] private Color _end;
    private Tween _tween;
    private void OnEnable()
    {
        CardHighLightController.Register(_img);
    }

    private void OnDisable()
    {
        CardHighLightController.Unregister(_img);
    }
}
