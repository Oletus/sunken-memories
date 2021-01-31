using UnityEngine;

namespace LPUnityUtils
{

    public static class AnimatorExtensions {

        public static bool HasParameter(this Animator animator, string name)
        {
            if ( animator == null )
            {
                return false;
            }
            if ( string.IsNullOrEmpty(name) )
            {
                return false;
            }
            foreach ( AnimatorControllerParameter param in animator.parameters )
            {
                if ( param.name == name )
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetTriggerIfExists(this Animator animator, string name)
        {
            if ( animator == null )
            {
                return;
            }
            if ( string.IsNullOrEmpty(name) )
            {
                return;
            }
            if ( animator.HasParameter(name) )
            {
                animator.SetTrigger(name);
            }
        }

        public static void SetIntegerIfExists(this Animator animator, string name, int intValue)
        {
            if ( animator == null )
            {
                return;
            }
            if ( string.IsNullOrEmpty(name) )
            {
                return;
            }
            if ( animator.HasParameter(name) )
            {
                animator.SetInteger(name, intValue);
            }
        }

        public static void SetFloatIfExists(this Animator animator, string name, float floatValue)
        {
            if ( animator == null )
            {
                return;
            }
            if ( string.IsNullOrEmpty(name) )
            {
                return;
            }
            if ( animator.HasParameter(name) )
            {
                animator.SetFloat(name, floatValue);
            }
        }

        public static void SetBoolIfExists(this Animator animator, string name, bool boolValue)
        {
            if ( animator == null )
            {
                return;
            }
            if ( string.IsNullOrEmpty(name) )
            {
                return;
            }
            if ( animator.HasParameter(name) )
            {
                animator.SetBool(name, boolValue);
            }
        }
    }

}
