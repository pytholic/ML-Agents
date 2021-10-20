using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private Bounds bndFloor; // floor bounds
    private Vector3 orig;
    private GameObject platform;

    public override void Initialize()
    {
        orig = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z); // original position
        platform = this.transform.parent.transform.Find("platform").gameObject;
        bndFloor = platform.transform.Find("floor").gameObject.GetComponent<Renderer>().bounds; // getting floor bounds
    }

    private void RandomPlaceTarget()
    {
        float rx = Random.Range(bndFloor.min.x, bndFloor.max.x);
        float rz = Random.Range(bndFloor.min.z, bndFloor.max.z);
        targetTransform.position = new Vector3(rx, orig.y, rz); //targetTransform.localPosition does not work
    }
    public override void OnEpisodeBegin()
    {
        Globals.Episode += 1;
        transform.localPosition = new Vector3(orig.x, orig.y, orig.z);
        RandomPlaceTarget();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // x, y, z => 3 values
        sensor.AddObservation(targetTransform.localPosition);
    }

    public float moveSpeed = 10;
    public override void OnActionReceived(float[] vectorAction)
    {
        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        Globals.ScreenText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            Globals.Success += 1;
            SetReward(+1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }

        if (other.CompareTag("wall"))
        {
            Globals.Fail += 1;
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

    // Use heuristic to test by manually sending actions
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }
}
