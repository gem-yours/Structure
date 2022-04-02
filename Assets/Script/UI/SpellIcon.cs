using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
public class SpellIcon : MonoBehaviour
{
    public Image? spellImage;
    private Rigidbody2D? rb2D;

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
        var animationDuration = 0.5f;
        for (float current = 0; current < animationDuration; current += Time.deltaTime)
        {
            var displacment = transform.position - target.transform.position;
            var easeInOut = AnimationCurve.EaseInOut(0, displacment.magnitude, animationDuration, 0);
            transform.position = target.transform.position + displacment.normalized * easeInOut.Evaluate(current);
            yield return null;
        }
        // transform.position = target.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
