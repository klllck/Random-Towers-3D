using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WobbleText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
    IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] private TMP_Text textComponent;

    private bool isHover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetTextAnimation();
        textComponent.color = Color.white;
        isHover = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        textComponent.color = Color.yellow;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        textComponent.color = Color.white;
    }

    private void Update()
    {
        if (isHover)
        {
            AnimateText();
        }
    }

    private void AnimateText()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var currentPos = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] = currentPos
                    + new Vector3(0f, Mathf.Sin(Time.time * 2f + currentPos.x * 0.01f) * 10f, 0f);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
    
    private void ResetTextAnimation()
    {
        var textInfo = textComponent.textInfo;
        var newVertexPositions = textInfo.meshInfo[0].vertices;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                continue;
            }

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            float offsetY = 0;

            newVertexPositions[vertexIndex + 0].y += offsetY;
            newVertexPositions[vertexIndex + 1].y += offsetY;
            newVertexPositions[vertexIndex + 2].y += offsetY;
            newVertexPositions[vertexIndex + 3].y += offsetY;
        }

        textComponent.mesh.vertices = newVertexPositions;
        textComponent.mesh.uv = textComponent.textInfo.meshInfo[0].uvs0;
        textComponent.mesh.uv2 = textComponent.textInfo.meshInfo[0].uvs2;
        textComponent.mesh.colors32 = textComponent.textInfo.meshInfo[0].colors32;
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        textComponent.ForceMeshUpdate();
    }
}
