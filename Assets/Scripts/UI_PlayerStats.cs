using UnityEngine;
using TMPro;

public class UI_PlayerStats : MonoBehaviour
{
    public TMP_Text m_CoinText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_CoinText.text = ""+PlayerStats.GetCoins();
    }
}
