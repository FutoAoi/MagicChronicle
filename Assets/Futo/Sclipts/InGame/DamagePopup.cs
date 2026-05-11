using TMPro;
using UnityEditor.Media;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private int _textSize = 24;
    private TextMeshProUGUI _textMesh;
    private float _disappearTimer;
    private Color _textColor;
    private Vector3 _moveVector;
    private DamagePopUpObjectPool _pool;
    private RectTransform _rect;

    private const float DISAPPEAR_TIME = 1.0f;
    private const float MOVE_SPEED = 100.0f;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Setup(int damage, DamagePopUpObjectPool pool)
    {
        _pool = pool;

        if (_textMesh == null)
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        _textMesh.text = damage.ToString();
        _textMesh.fontSize = _textSize;
        _textMesh.color = Color.white;

        _textColor = _textMesh.color;
        _disappearTimer = DISAPPEAR_TIME;

        _moveVector = new Vector2(0, 1f) * MOVE_SPEED;
    }

    private void Update()
    {
        _moveVector = Vector2.MoveTowards(_moveVector, Vector2.zero, 80f * Time.deltaTime);
        _rect.anchoredPosition += (Vector2)_moveVector * Time.deltaTime;

        _disappearTimer -= Time.deltaTime;

        if (_disappearTimer < DISAPPEAR_TIME * 0.5f)
        {
            _textColor.a -= 2f * Time.deltaTime;
            _textMesh.color = _textColor;
        }

        if (_disappearTimer < 0f)
        {
            _pool.Return(this);
        }
    }
}
