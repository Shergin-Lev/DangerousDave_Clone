using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance {  get; private set; }

    private Dictionary<string, string> localizedText;
    private string currentLanguage = "en"; // �� ��������� ����������

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizedText(currentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLocalizedText(string languageCode)
    {
        currentLanguage = languageCode;
        localizedText = new Dictionary<string, string>();

        // ��������� JSON ���� � ����������
        string filePath = $"Localization/{languageCode}";
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);

        if (textAsset != null)
        {
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(textAsset.text);

            foreach (var item in data.items)
            {
                localizedText.Add(item.key, item.value);
            }

            Debug.Log($"Loaded {localizedText.Count} localization entries for '{languageCode}'");
        }
        else
        {
            Debug.LogError($"Localization file not found: {filePath}");
        }
    }

    public static string GetText(string key)
    {
        if (Instance == null || Instance.localizedText == null)
        {
            return key; // ���������� ����, ���� ����������� �� ���������
        }

        if (Instance.localizedText.TryGetValue(key, out string value))
        {
            return value;
        }

        Debug.LogWarning($"Localization key not found; {key}");
        return key; // ���� �� ������ �������, ���������� ����
    }

    public static void SetLanguage(string languageCode)
    {
        if (Instance != null)
        {
            Instance.LoadLocalizedText(languageCode);
        }
    }
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}
