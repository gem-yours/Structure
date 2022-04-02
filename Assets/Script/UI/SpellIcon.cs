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
        while ((transform.position - target.transform.position).magnitude > 1)
        {
            var direction = (target.transform.position - transform.position);
            var displacement = direction * 10 * Time.deltaTime;
            Debug.Log(displacement.magnitude);
            transform.position = transform.position + displacement;
            yield return null;
        }
        transform.position = target.transform.position;
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
