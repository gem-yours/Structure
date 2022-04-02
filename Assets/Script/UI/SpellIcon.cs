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
        if (rb2D == null) yield break;
        while ((Vector3)rb2D.position != target.transform.position)
        {
            rb2D.MovePosition(Vector2.MoveTowards(rb2D.position, target.transform.position, 0.5f));
            yield return null;
        }
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
