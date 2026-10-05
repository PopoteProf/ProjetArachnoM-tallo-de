using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour {
    [SerializeField] private string PlayerTag = "Player";


    private void Start() {
        StaticData.EndLevelPosition = transform.position;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == PlayerTag) {
            StaticData.DoWin();
            gameObject.SetActive(false);
        }
    }
}