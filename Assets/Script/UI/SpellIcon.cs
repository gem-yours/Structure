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

    public void MoveTo(Vector2 destination)
    {
        StartCoroutine(_Move(destination));
    }

    private IEnumerator _Move(Vector2 destination)
    {
        if (rb2D == null) yield break;
        while (rb2D.position != destination)
        {
            rb2D.MovePosition(Vector2.MoveTowards(rb2D.position, destination, 1f));
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
