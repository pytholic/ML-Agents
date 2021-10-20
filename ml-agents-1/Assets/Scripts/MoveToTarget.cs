using UnityEngine;
using Unity.MLAgents;

public class MoveToTarget : Agent
{
    public float Movespeed = 5;
    private Vector3 orig; // original position of the cube when game starts
    private Bounds bndFloor; // floor bounds
    private GameObject Target = null;
    public override void Initialize()
    {
        orig = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        bndFloor = this.transform.parent.transform.Find("Floor").gameObject.GetComponent<Renderer>().bounds;
        Target = this.transform.parent.transform.Find("Target").gameObject;
    }

    public override void OnEpisodeBegin()
    {
        Globals.Episode += 1;
        this.transform.position = new Vector3(orig.x, orig.y, orig.z);
        RandomPlaceTarget();

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        this.transform.Translate(Vector3.right * vectorAction[0] * Movespeed * Time.deltaTime); // translation on x-axis
        this.transform.Translate(Vector3.forward * vectorAction[1] * Movespeed * Time.deltaTime); // translation on z-axis
        BoundCheck();
        Globals.ScreenText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target") == true)
        {
            Globals.Success += 1;
            AddReward(1.0f);
            EndEpisode();
        }
    }

    private void RandomPlaceTarget()
    {
        float rx = Random.Range(bndFloor.min.x, bndFloor.max.x);
        float rz = Random.Range(bndFloor.min.z, bndFloor.max.z);
        Target.transform.position = new Vector3(rx, 0, rz);
    }

    private void BoundCheck()
    {
        if (this.transform.position.x < bndFloor.min.x)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
        else if (this.transform.position.x > bndFloor.max.x)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }

        if (this.transform.position.z < bndFloor.min.z)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
        else if (this.transform.position.z > bndFloor.max.z)
        {
            Globals.Fail += 1;
            AddReward(-0.1f);
            EndEpisode();
        }
    }
}