using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private void Start() {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }


    Vector3 targetAngle;
    private void LateUpdate() {

        targetAngle.y = transform.eulerAngles.y;
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();

        if (Vector3.Distance(targetPosition,  transform.position) <= 0.2f)
        {
            targetAngle.z = 0;
            targetAngle.x = 0;
        }

        else
        {
            if (targetPosition.x < transform.position.x)
            {
                targetAngle.z = 20;
            }

            if (targetPosition.x > transform.position.x)
            {
                targetAngle.z = -20;
            }

            if (targetPosition.z < transform.position.z)
            {
                targetAngle.x = -20;
            }

            if (targetPosition.z > transform.position.z)
            {
                targetAngle.x = 20;
            }
        }


        if(transform.childCount > 0)
            transform.GetChild(0).transform.DOLocalRotate(new Vector3(targetAngle.x, 0 ,targetAngle.z), 0.25f);

        targetPosition.y = .7f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }
    
    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}

