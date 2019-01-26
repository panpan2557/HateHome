using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heightfollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        this.GetComponent<SpriteRenderer>().size = this.transform.parent.GetComponent<SpriteRenderer>().size;
    }

}
