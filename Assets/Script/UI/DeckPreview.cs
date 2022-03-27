using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckPreview : MonoBehaviour
{
    private List<Spell> _currentCandidates = new List<Spell> { null, null, null };

    public List<GameObject> candidates = new List<GameObject>();

    private int numberOfCandidates = 3;
    public Deck deck
    {
        set
        {
            if (value == null)
            {
                return;
            }
            value.onAdd = (Deck deck) =>
            {
                if (_currentCandidates.FindAll(x => x != null).Count >= 3)
                {
                    return;
                }
                var candidates = deck.LatestCandidates(numberOfCandidates);
                foreach (Spell spell in candidates)
                {
                    if (spell != null)
                    {
                        ShowSpell(spell);
                        _currentCandidates.Add(spell);
                    }
                }

            };
            // value.onPick = listener;
            // value.onShuffle = listener;
        }
    }


    private GameObject ShowSpell(Spell spell)
    {
        var go = new GameObject();
        var image = go.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("SpellIcon/" + spell.imageName);
        go.AddComponent<LayoutElement>();
        var fitter = go.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        fitter.aspectRatio = 1;
        go.transform.SetParent(this.gameObject.transform);
        go.transform.position = Vector3.zero;
        go.transform.localScale = Vector3.one;


        return go;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
