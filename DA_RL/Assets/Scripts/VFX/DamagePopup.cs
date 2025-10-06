using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro dmgText;
    private float disappearTimer;
    private Color textColor;



    public static DamagePopup Create(Vector3 pos, int damage, bool isCritial)
    {
        Transform dmgPopTrans = Instantiate(GameManager.init.DamagePopupObj, pos, Quaternion.identity);
        DamagePopup dmgPopup = dmgPopTrans.GetComponent<DamagePopup>();
        dmgPopup.Setup(damage, isCritial);

        return dmgPopup;
    }

    private void Awake()
    {
        dmgText = transform.GetComponent<TextMeshPro>();
    }

    private void Setup(int damage, bool isCrit)
    {
        dmgText.SetText(damage.ToString());
        /*if (isCrit) { dmgText.fontSize = 45; textColor = Color.red; }
        else { dmgText.fontSize = 36; textColor = new Color(255, 121, 0); }*/

        textColor = dmgText.color;
        disappearTimer = 1f;
    }

    private void Update()
    {
        float moveYSpeed = 10f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if(disappearTimer < 0)
        {
            textColor.a -= 3f * Time.deltaTime;
            dmgText.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
