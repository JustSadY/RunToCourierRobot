using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private static readonly int IsNear = Animator.StringToHash("isNear");

    [SerializeField] private float openDistance = 3f;

    private GameObject _package;
    private Animator _animator;

    private bool _hasFinished = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _package = GameObject.FindWithTag("Package");
    }

    private void Update()
    {
        if (UI.instance.isFinish) return;
        if (!_package) return;

        float distance = Vector2.Distance(transform.position, _package.transform.position);
        bool isNear = distance < openDistance;

        _animator.SetBool(IsNear, isNear);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasFinished) return;
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Inventory>(out Inventory inventory)) return;
        if (!inventory.HasItem("Package")) return;

        _hasFinished = true;
        UI.instance.isFinish = true;
        other.gameObject.SetActive(false);
        _animator.SetBool(IsNear, false);

        StartCoroutine(WaitForDoorToCloseAndNextLevel());
    }

    private IEnumerator WaitForDoorToCloseAndNextLevel()
    {
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

        yield return new WaitForSeconds(0.5f);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        int lastLoadedLevel = PlayerPrefs.GetInt("LastLoadedLevel", 1);
        if (lastLoadedLevel < nextSceneIndex)
        {
            PlayerPrefs.SetInt("LastLoadedLevel", nextSceneIndex);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}