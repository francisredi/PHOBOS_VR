using UnityEngine;

namespace RealisticEyeMovements {

	[System.Serializable]
	public class EyeControlData
	{
		#region fields

				public enum EyeControl
				{
					None,
					MecanimEyeBones,
					SelectedObjects
				}
				public EyeControl eyeControl = EyeControl.MecanimEyeBones;

				public Transform leftEye;
				public Transform rightEye;

		#endregion


		public void CheckConsistency( Animator animator )
		{
			if ( eyeControl == EyeControl.MecanimEyeBones )
			{
				if ( null == animator )
					throw new System.Exception("No Animator found.");
				if ( null == animator.GetBoneTransform(HumanBodyBones.LeftEye) || null == animator.GetBoneTransform(HumanBodyBones.RightEye) )
					throw new System.Exception("Mecanim humanoid eye bones not found.");
			}
			else if ( eyeControl == EyeControl.SelectedObjects )
			{
				if ( null == leftEye )
					throw new System.Exception("The left eye object hasn't been assigned.");
				if ( null == rightEye )
					throw new System.Exception("The right eye object hasn't been assigned.");
			}
		}



	}
}
