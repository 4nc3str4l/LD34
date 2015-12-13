using UnityEngine;

public class Skull : MonoBehaviour {

    float speed;
    GameObject monster;
    GUIController guiController;

    void Start () {
        this.transform.parent = null;
        this.transform.parent = null;
        monster = GameObject.FindWithTag("Player");
    }

    void Update () {

        transform.position = Vector3.MoveTowards(transform.position, monster.transform.position, .1f);
    }

    void OnTriggerEnter2D(Collider2D colider)
    {
        if(colider.GetComponentInChildren<AbilityController>())
        {
            GameController.instance.addDead();
            Destroy(gameObject);
        }
    }
}
