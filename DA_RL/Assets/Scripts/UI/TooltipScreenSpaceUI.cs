using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipScreenSpaceUI : MonoBehaviour
{

    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform rectTransform;

    private void SetTooltipText(string tooltipString)
    {
        tooltipText.SetText(tooltipString);
        tooltipText.ForceMeshUpdate();

        Vector2 textSize = tooltipText.GetRenderedValues(false);
        Vector2 padding = new Vector2(8f, 8f);

        backgroundRectTransform.sizeDelta = textSize + padding;
    }

    private void Update()
    {
        
    }
}
