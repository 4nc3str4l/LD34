using UnityEngine;

public class Skull : MonoBehaviour {

    float speed;
    GameObject monster;
    GUIController guiController;
    bool _addedForce = false;

    void Start () {
        this.transform.parent = null;
        this.transform.parent = null;
        monster = GameObject.FindWithTag("Player");
    }

    void Update ()
    {
        if(!_addedForce)GetComponent<Rigidbody2D>().AddForce(Vector2.up * 300);
        transform.position = Vector3.MoveTowards(transform.position, monster.transform.position, 0.2f);
        _addedForce = true;
    }

    void OnTriggerEnter2D(Collider2D colider)
    {
        if(colider.GetComponentInChildren<AbilityController>())
        {
            GameController.Instance.addDead();
            Destroy(gameObject);
        }
    }
}
