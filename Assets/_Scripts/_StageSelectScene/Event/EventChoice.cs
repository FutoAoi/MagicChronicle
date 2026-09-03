using UnityEngine;

[CreateAssetMenu(menuName = ("Datas/Choice"))]
public class EventChoice : ScriptableObject
{
    [SerializeField, Tooltip("‘I‘ðŽˆ‚Ì•¶")] private string _choiceText;
    [SerializeField, Tooltip("Œ‹‰Ê•¶")] private string _resultText;
    [SerializeReference, SubclassSelector] private IEventEffect[] _eventEffects;

    public string ChoiceText => _choiceText;
    public string ResultText => _resultText;
    public IEventEffect[] EventEffects => _eventEffects;
}
