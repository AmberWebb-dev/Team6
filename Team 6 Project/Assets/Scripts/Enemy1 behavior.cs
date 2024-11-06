using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript1 : MonoBehaviour
{
    //setting up basic things here, will make the methods soon.
    bool isShooting;
    bool playerInRange;

    [SerializeField] int HP;
    [SerializeField] int faceTagetSpeed;
    [SerializeField] GameObject bullet;
    [SerializeField] Renderer model;

    Color colorOrig;

    float shootRange;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        //gamemanager updating the game goal goes here.
    }

    // Update is called once per frame
    void Update()
    {

    }

    //dmg method
    public void takeDamage(int amount)
    {
        HP -= amount;
        //color change here

        if(HP <=0)
        {
            //this wont work until the gamegoal is finished
            //GameManager.Instance.GameGoal(-1);
            //Destroy(gameObject);
        }
    }

    //shooting
    //IEnumerator shoot()
    //{
        
    //}






    //flashing red method
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}
