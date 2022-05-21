using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class SpellIcon : MonoBehaviour
{
#pragma warning disable CS8618
    public Image spellImage;
#pragma warning restore CS8618
    private Spell? _spell;
    public Spell? spell
    {
        set
        {
            _spell = value;
            if (spellImage == null) return;
            spellImage.sprite = (value == null) ? null : value.image;
        }
        get
        {
            return _spell;
        }
    }

    private bool _isActive = true;
    public bool isActive
    {
        set
        {
            _isActive = value;
            if (value)
            {
                spellImage.color = new Color(1, 1, 1);
            }
            else
            {
                spellImage.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        get
        {
            return _isActive;
        }
    }
#pragma warning disable CS8618
    private Rigidbody2D rb2D;
#pragma warning restore CS8618


    public void AttachTo(GameObject target)
    {
        if (target.transform.position == null)
        {
            return;
        }
        StartCoroutine(_Attach(target));
    }

    private IEnumerator _Attach(GameObject target)
    {
        DeleteBackground();
        var animationDuration = 0.5f;
        var rectTransform = GetComponent<RectTransform>();
        var targetRectTransform = target.GetComponent<RectTransform>();
        // targetはsizefitterで幅を調節している
        // 知識が漏れてる感じはするが、現状いい解決策が思いつかないのでこれで対応する
        var widthScale = (rectTransform is not null && targetRectTransform is not null) ?
#pragma warning disable CS8602
        targetRectTransform.sizeDelta.x / rectTransform.sizeDelta.x : 1f;
        var initialWidth = rectTransform.sizeDelta.x;
#pragma warning restore CS8602

        yield return AnimationUtil.EaseInOut(
            animationDuration,
            (current) =>
            {
                var displacment = transform.position - target.transform.position;
                transform.position = target.transform.position + displacment * current;
                transform.localScale = new Vector2(
                    1 + (widthScale - 1) * current,
                    1 + (widthScale - 1) * current
                );
            }
        );
        transform.position = target.transform.position;
    }


    private void DeleteBackground()
    {
        var image = GetComponent<Image>();
        if (image is not null)
        {
            Destroy(image);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
}
