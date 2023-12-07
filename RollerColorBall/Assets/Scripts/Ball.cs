using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine.Scripting.APIUpdating;
using System.Linq;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;

    [SerializeField] private float stepDuration = .1f;
    private const float MAX_RAY_DISTANCE = 1000f;

    public UnityAction <List<RoadTile>,float> OnMoveStart;

    private Vector3 _direction;
    private bool canMove = true;
    private void Start()
    {
        //change def ball pos:

        transform.position = levelManager.defaultBallRoadTile.position;
        swipeListener.OnSwipe.AddListener(swipe =>
        {
            switch (swipe)
            {
                case "Right":
                    _direction = Vector3.right;
                    break;
                case "Left":
                    _direction = Vector3.left;
                    break;
                case "Up":
                    _direction = Vector3.forward;
                    break;
                case "Down":
                    _direction = Vector3.back;
                    break;

            }
            Move();
        });
    }

    private void Move()
    {
        if (canMove)
        {
            Debug.Log("Can");
            canMove = false;
            //Raycast:
            RaycastHit[] hits = Physics.RaycastAll(transform.position, _direction, MAX_RAY_DISTANCE)
                                       .OrderBy(hit => hit.distance).ToArray(); // added this line to sort tiles by distance from the ray's origin
            Debug.Log(hits.Length);
            Vector3 targetPosition = transform.position;

            int steps = 0;
            List<RoadTile> pathRoadTiles = new List<RoadTile>();
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger)
                { // Road tile
                  // add road tiles to the list to be painted:
                    pathRoadTiles.Add(hits[i].transform.GetComponent<RoadTile>());


                }
                else
                { // Wall tile
                    if (i == 0)
                    { // means wall is near the ball
                        canMove = true;
                        return;
                    }
                    //else:
                    steps = i;
                    targetPosition = hits[i - 1].transform.position;
                    break;
                }
            }
            StartCoroutine(MoveCoroutine(targetPosition, stepDuration * steps,pathRoadTiles));
        }

        IEnumerator MoveCoroutine(Vector3 targetPosition, float duration,List<RoadTile> pathRoadTile)
        {
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                if (OnMoveStart != null)
                    OnMoveStart.Invoke(pathRoadTile, duration*2);
                transform.position = Vector3.Lerp(startingPosition, targetPosition, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            transform.position = targetPosition; 
            canMove = true;


        }
    }

}

