using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehaviour : MonoBehaviour
{
	public bool isAiming;
	public bool useLongWeapons;
	public Animator animator;
	public float armsRotation = 8f;                                // Rotation of arms to align with aim, according player height.
	private Transform hips, spine, chest;						   // Avatar bone transforms.
	private Vector3 initialRootRotation;                           // Initial root bone local rotation.
	private Vector3 initialHipsRotation;                           // Initial hips rotation related to the root bone.
	private Vector3 initialSpineRotation;                          // Initial spine rotation related to the hips bone.
	private Vector3 initialChestRotation;                          // Initial chest rotation related to the spine bone.

	public Transform target;

    private void Start()
    {
		hips = animator.GetBoneTransform(HumanBodyBones.Hips);
		spine = animator.GetBoneTransform(HumanBodyBones.Spine);
		chest = animator.GetBoneTransform(HumanBodyBones.Chest);
		Transform root = hips.parent;

		// Get initial values.
		initialRootRotation = (root == transform) ? Vector3.zero : root.localEulerAngles;
		initialHipsRotation = hips.localEulerAngles;
		initialSpineRotation = spine.localEulerAngles;
		initialChestRotation = chest.localEulerAngles;
	}

    // Manage inverse kinematic parameters.
    public void OnAnimatorIK(int layerIndex)
	{
		if (isAiming)
		{
			// Orientate upper body where camera  is targeting.
			//Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
			//targetRot *= Quaternion.Euler(initialRootRotation);
			//targetRot *= Quaternion.Euler(initialHipsRotation);
			//targetRot *= Quaternion.Euler(initialSpineRotation);
			// Set upper body horizontal orientation.
			//animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(hips.rotation) * targetRot);

			// Keep upper body orientation regardless strafe direction.
			float xCamRot = Quaternion.LookRotation(target.transform.forward).eulerAngles.x;
			Quaternion targetRot = Quaternion.AngleAxis(xCamRot + armsRotation, this.transform.right);

            if (useLongWeapons)
            {
				// Correction for long weapons.
				targetRot *= Quaternion.AngleAxis(9f, this.transform.right);
				targetRot *= Quaternion.AngleAxis(20f, this.transform.up);
            }

			targetRot *= spine.rotation;
			//targetRot *= Quaternion.Euler(initialChestRotation);
			// Set upper body vertical orientation.
			animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * targetRot);
		}
	}
}
