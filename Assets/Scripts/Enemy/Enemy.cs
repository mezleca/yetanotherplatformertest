using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class Enemy : MonoBehaviour
{
    // enemy attributes
    public float MIN_DISTANCE_TRIGGER = 5.0f;

    public GameEntity ent;
    private GameEntity player;

    void Start()
    {

    }

    GameEntity get_closest_player() 
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        Player result = null;

        float best = 999.0f;

        for (int i = 0; i < players.Length; i++) {
            var pos = players[i].transform.position;
            float distance = Vector3.Distance(transform.position, pos);

            if (distance < best) {
                result = players[i];
                best = distance;
            }
        }

        if (result != null) 
        {
            return result.ent;
        }

        return null;
    }

    void Awake()
    {
        if (ent == null)
        {
            ent = gameObject.AddComponent<GameEntity>();
        }

        if (player == null) 
        {
            player = get_closest_player();
        }

        Debug.Log("created enemy");
    }

    void Update() {
        // get the closest player
        if (player == null) 
        {
            var result = get_closest_player();

            if (!result) 
            {
                return;
            }

            player = result;
        }

        // move towards the player if close enough
        // TODO: check for collision
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 0 && distance < MIN_DISTANCE_TRIGGER)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 2f * Time.deltaTime);
        }
    }

    void OnDestroy()
    {
         
    }
};