using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("Datas/DeckData"))]
public class DeckData : ScriptableObject
{
    [Header("デッキの名前")]
    public string DeckName;
    [Header("デッキ内のカードリスト")]
    public List<int> Cards;
}
