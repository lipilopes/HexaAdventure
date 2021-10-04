using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MobSkillManager))]
public class MobSkillManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Esse Script Não pode Ser Usado!!!\n\n* Mob Skill Basic Damage\n\n* Mob Skill Pull Damage\n...", MessageType.Warning);
    }
}
