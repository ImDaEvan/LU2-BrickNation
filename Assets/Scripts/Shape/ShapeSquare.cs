using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;
    void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }
    public void DeactivateCell()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }
    public void ActivateCell()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(true);
    }
    public void SetOccupied()
    {
        occupiedImage.gameObject.SetActive(true);
    }
    public void UnsetOccupied()
    {
         occupiedImage.gameObject.SetActive(false);
    }
}
  