using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public Image normalImage;
    public Image hoverImage;
    public Image activeImage;
    public bool Selected {get;set;}
    public int CellIndex {get;set;}
    public bool CellOccupied {get;set;}
    public List<Sprite> normalImages;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CellOccupied == false)
        {
            Selected = true;
            hoverImage.gameObject.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        Selected = true;
        
        if(CellOccupied == false)
        {
            hoverImage.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(CellOccupied == false)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false);
        }    
    }
    void Start()
    {
        Selected = false;
        CellOccupied = false;
    }
    public bool CanUseCell()
    {
        return hoverImage.gameObject.activeSelf;
    }
    public void ActivateCell()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        CellOccupied = true;
    }
    public void SetImage(bool setFirstImage)
    {
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }
    public void PlaceShapeOnBoard()
    {
        ActivateCell();
    }
}
