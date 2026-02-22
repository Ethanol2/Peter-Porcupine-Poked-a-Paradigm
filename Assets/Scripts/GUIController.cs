using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _pointsText;

    [Header("Insanity")]
    [SerializeField] private Image _insanityBar;
    [SerializeField] private Color _goodColor = Color.cornflowerBlue, _badColor = Color.softRed;

    [Header("Health")]
    [SerializeField] private Transform _healthPointsParent;
    [SerializeField] private GameObject _healthPointsObject;
    private List<GameObject> _healthPoints = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInsanityBar();
        for (int i = 0; i < GameManager.Instance.MaxHitPoints; i++)
        {
            GameObject obj = Instantiate(_healthPointsObject, _healthPointsParent);
            obj.SetActive(true);
            _healthPoints.Add(obj);
        }
        _healthPointsObject.gameObject.SetActive(false);

        GameManager.Instance.OnHealthChange.AddListener(UpdateHealthPoints);
        GameManager.Instance.OnPointsChange.AddListener(UpdateScore);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInsanityBar();
    }

    private void UpdateInsanityBar()
    {
        _insanityBar.fillAmount = GameManager.Instance.InsanityNormalized;
        _insanityBar.color = Color.Lerp(_badColor, _goodColor, _insanityBar.fillAmount);
    }

    private void UpdateHealthPoints(int count)
    {
        foreach (GameObject obj in _healthPoints)
        {
            obj.SetActive(count > 0);
            count--;
        }
    }

    private void UpdateScore(int score)
    {
        _pointsText.text = $"{score} Points";
    }
}
