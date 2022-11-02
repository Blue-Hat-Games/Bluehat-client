using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    [Space]
    public float targetValue;

    [Space]
    public Image fillImage;
    public Image backGroundImage;
    float backGroundSize;
    
    float incrementValue;
    float currentValue;
    float totalvalue;
    float currentPositionX;
    public InputField inputField;
    string input;
    public Button addBtn;
    float calcValue;
    RectTransform fillImageRecTransform;
    

    private void Start()
    {
        backGroundSize = backGroundImage.rectTransform.rect.width;
        
        addBtn.onClick.AddListener(Add);
       
        currentPositionX = fillImage.rectTransform.anchoredPosition.x;
       
        incrementValue = (backGroundSize / targetValue);

    }


    public void Add()
    {
        input = inputField.text;
        
        float inVal = float.Parse(input);
        
        AddProggress(inVal); 
    }

    public void AddProggress(float value)
    {
        if (value > targetValue)
        {
            totalvalue = targetValue;

            currentValue = totalvalue * incrementValue;

            calcValue = currentPositionX + currentValue;

            if (fillImage.rectTransform.anchoredPosition.x != 0 && totalvalue <= targetValue)
            {
                fillImage.rectTransform.anchoredPosition = new Vector2(calcValue, 0);
            }
            else
            {

            }
        } else if (value < targetValue)
        
        {
            totalvalue += value;

            currentValue = totalvalue * incrementValue;

            calcValue = currentPositionX + currentValue;

            if (fillImage.rectTransform.anchoredPosition.x != 0 && totalvalue <= targetValue)
            {
                fillImage.rectTransform.anchoredPosition = new Vector2(calcValue, 0);
            }
            else
            {

            }
        }
      


    }
    
}
