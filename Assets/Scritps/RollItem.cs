using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RollItem : MonoBehaviour
{
    public Image image;
    public Transform graphicElement;
    public float scaleFactor = 5;

    private string type;
    public RollItemImagesSO itemImagesSO;
    public void ResetToDefault()
    {
        image.sprite = null;
        type = string.Empty;
        StopAllCoroutines();
        graphicElement.localScale = Vector3.one;
    }
    public void SetNewData(string type)
    {
        this.type = type;
        Sprite img = itemImagesSO.GetSprite(type);
        if (img == null)
            Debug.Log("selected image not found for " + type);
        else
            image.sprite = img;
    }
    public void AnimateWin()
    {
        StartCoroutine(IEScalingAnimation());
    }
    private IEnumerator IEScalingAnimation()
    {
        float scaleMul = 1;
        for (int i = 0; i < 3; i++)
        {
            while (scaleMul < 1.3f)
            {
                scaleMul += Time.deltaTime * scaleFactor;
                graphicElement.localScale = scaleMul * Vector3.one;
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);
            while (scaleMul > 0.7f)
            {
                scaleMul -= 3 * Time.deltaTime * scaleFactor;
                graphicElement.localScale = scaleMul * Vector3.one;
                yield return null;
            }
        }
        while (scaleMul < 1f)
        {
            scaleMul += Time.deltaTime * scaleFactor;
            graphicElement.localScale = scaleMul * Vector3.one;
            yield return null;
        }
        graphicElement.localScale = Vector3.one;
    }
}
