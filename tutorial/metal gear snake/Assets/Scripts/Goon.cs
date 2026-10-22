using UnityEngine;

// A goon standing in the vents. Snake knocks him out by crawling into him.
public class Goon : MonoBehaviour
{
    public Sprite knockedOutSprite; // drag the GoonKO sprite here in the Inspector

    // Called by Snake when he crawls into the goon
    public void KnockOut()
    {
        GetComponent<SpriteRenderer>().sprite = knockedOutSprite;

        // Turn off the collider so Snake can crawl over him
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
