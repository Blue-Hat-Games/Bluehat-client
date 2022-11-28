using UnityEngine;

public class HatManager : MonoBehaviour
{
    public GameObject animalObject;

    public GameObject[] hatPrefabs;

    private void Start()
    {
        Transform hatPoint = null;

        var allChildren = animalObject.GetComponentsInChildren<Transform>();
        foreach (var childTr in allChildren)
        {
            if (childTr.name == "HatPoint") hatPoint = childTr;

            Debug.Log(childTr.name);
        }

        SetHat(animalObject);
    }

    private void SetHat(GameObject animalObj)
    {
        animalObj.SetActive(false);

        var initX = 0;
        for (var i = 0; i < hatPrefabs.Length; i++)
        {
            var animal = Instantiate(animalObj, new Vector3(initX, 0, 0), Quaternion.identity);
            animal.SetActive(true);
            Transform hatPoint = null;
            var allChildren = animal.GetComponentsInChildren<Transform>();

            foreach (var childTr in allChildren)
                if (childTr.name == "HatPoint")
                    hatPoint = childTr;

            var hatObj = Instantiate(hatPrefabs[i]);
            hatObj.transform.SetParent(hatPoint);
            hatObj.transform.localPosition = Vector3.zero;
            hatObj.transform.rotation = Quaternion.identity;

            initX += 3;
        }
    }
}