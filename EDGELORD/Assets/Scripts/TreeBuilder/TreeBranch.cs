﻿using System.Collections;
using System.Collections.Generic;
using Players;
using UnityEngine;

namespace EDGELORD.TreeBuilder
{
    /// <summary>
    /// Branch Component that goes onto each branch. 
    /// Each Branch should have it's root transform at the base of the branch.
    /// 
    /// - Creates Child Branches
    /// - Handles Slicing
    /// - Sends Slice to enemy branches.
    /// </summary>
    public class TreeBranch : MonoBehaviour, IBranchable
    {
        public PlayerID OwningPlayer;
        public List<TreeBranch> DirectChildBranches = new List<TreeBranch>();
        [Space]
        public GameObject SpriteObject; // The visual 
        public TreeBranchData BranchData;

        public TreeRoot MyRoot;
        [Header("Default Propoportions")]
        public float defaultLength = 3.2f;
        public float defaultWidth = 0.5f;

        private void Start()
        {
            var roots = FindObjectsOfType<TreeRoot>();
            foreach (TreeRoot root in roots)
            {
                if (root.OwningPlayer == OwningPlayer) MyRoot = root;
            }
            if (MyRoot == null)
            {
                Debug.LogWarning("[TreeBranch] Matching Root not found!");
                MyRoot = roots[0];
            }
        }
        // Call this when the Branch is created.
        public void Generate(TreeBranchData data, bool doCoroutine = true)
        {
            BranchData = data;
            //ToDo: Actually Generate Self based on input data.

            this.transform.parent = data.ParentBranch.transform;
            this.transform.localPosition = data.LocalBasePoint;
            var rot = Quaternion.LookRotation(Vector3.forward, (Vector3)data.GrowDirection.normalized);
            this.transform.rotation = rot;
            var targetScale = GetNewProportions(data.Length, data.Width);
            if (doCoroutine)
            {
                StartCoroutine(CoLerpGenerate(this.SpriteObject.transform, targetScale));
            }
            else
            {
                this.SpriteObject.transform.localScale = targetScale;
            }
        }

        private Vector3 GetNewProportions(float targetLength, float targetWidth)
        {
            float newLength = targetLength / defaultLength;
            float newWidth = targetWidth / defaultWidth;

            Vector3 newScale = new Vector3(newWidth,  newLength, 1f);
            return newScale;
        }

        public void SliceBranch(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject cutGameObject)
        {
            List<SpriteSlicer2DSliceInfo> sliceInfoList = new List<SpriteSlicer2DSliceInfo>();
            SpriteSlicer2D.SliceSprite(worldStartPoint, worldEndPoint, cutGameObject, false, ref sliceInfoList);
            
            HandleSliceReparenting(sliceInfoList.ToArray());

        }

        public void HandleSliceReparenting(SpriteSlicer2DSliceInfo[] sliceInfo)
        {
            //GameObject[] slicedPieces = 
            //float sliceDistanceFromRoot = 
            //Todo: Rechild children to each appropriate part.

            //Todo: Find sliced piece to remain attached, and replace the old one it with the new one.

            //ToDo: Add Rigidbody to unattached piece, and destroy it after a period of time. 
        }


        private IEnumerator CoLerpGenerate(Transform targetTransform, Vector3 targetScale, float lerpTime = 0.1f)
        {
            Vector3 currentScale = targetScale;
            currentScale.y = 0f;
            targetTransform.localScale = currentScale;
            float timer = 0f;
            while (timer < lerpTime)
            {
                timer += Time.deltaTime;
                currentScale.y = Mathf.Lerp(0f, targetScale.y, timer / lerpTime);
                targetTransform.localScale = currentScale;
                yield return null;
            }
            targetTransform.localScale = targetScale;
        }

    }
}