using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlphaButtonClickMask : MonoBehaviour, ICanvasRaycastFilter 
{
    protected Image _image;

    public void Start()
    {
        this._image = GetComponent<Image>();

        Texture2D tex = this._image.sprite.texture as Texture2D;

        bool isInvalid = false;
        if (tex != null)
        {
            try
            {
                tex.GetPixels32();
            }
            catch (UnityException e)
            {
                Debug.LogError(e.Message);
                isInvalid = true;
            }
        }
        else
        {
            isInvalid = true;
        }

        if (isInvalid)
        {
            Debug.LogError("This script need an Image with a readbale Texture2D to work.");
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this._image.rectTransform, sp, eventCamera, out localPoint);

        Vector2 normalizedLocal = new Vector2(1.0f + localPoint.x / this._image.rectTransform.rect.width, 1.0f + localPoint.y / this._image.rectTransform.rect.height);
        Vector2 uv = new Vector2(
            this._image.sprite.rect.x + normalizedLocal.x * this._image.sprite.rect.width,
            this._image.sprite.rect.y + normalizedLocal.y * this._image.sprite.rect.height );

        uv.x /= this._image.sprite.texture.width;
        uv.y /= this._image.sprite.texture.height;

        //uv are inversed, as 0,0 or the rect transform seem to be upper right, then going negativ toward lower left...
        Color c = this._image.sprite.texture.GetPixelBilinear(uv.x, uv.y);

        return c.a> 0.1f;
    }
}
