using UnityEngine;

[CreateAssetMenu(
    fileName = "FirebaseConfig",
    menuName = "Starlight Food Truck/Firebase Config")]
public class FirebaseConfig : ScriptableObject
{
    [Header("Realtime Database")]
    public string databaseUrl;

    public bool IsValid => !string.IsNullOrWhiteSpace(databaseUrl);
}
