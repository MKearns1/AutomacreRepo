using UnityEngine;

public class SelectionHighlight : MonoBehaviour
{

    private Renderer[] renderers;
    private MaterialPropertyBlock block;

    [SerializeField]
    private Color highlightColor = Color.yellow;
    [SerializeField] private Color SelectColor = Color.yellow;
    [SerializeField] private Color HoverColor = Color.yellow;

    private bool highlighted;
    bool isHovered;
    bool isSelected;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
    }

    public void SetHighlight(bool state, Color color = default)
    {
        //if (highlighted == state)
           // return;

        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();

        highlighted = state;

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);

            if (state)
            {
                block.SetColor("_EmissionColor", color);
            }
            else
            {
                block.Clear();
            }

            r.SetPropertyBlock(block);
        }
    }

    public void SetSelected(bool state)
    {
        isSelected = state;
        UpdateVisual();
    }
    public void SetHover(bool state)
    {
        isHovered = state;
        UpdateVisual();
    }
    
    public void UpdateVisual()
    {
        Color? color = null;
        if (isSelected) color = SelectColor;
        else if(isHovered) color = HoverColor;
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);

            if (color != null)
            {
                block.SetColor("_EmissionColor", color.Value);
            }
            else
            {
                block.Clear();
            }

            r.SetPropertyBlock(block);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
