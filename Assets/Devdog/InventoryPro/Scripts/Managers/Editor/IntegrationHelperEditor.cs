using UnityEngine;
using System.Collections;
using UnityEditor;
using Devdog.General.Editors;
using EditorStyles = Devdog.General.Editors.EditorStyles;

namespace Devdog.InventoryPro.Editors
{
    public class IntegrationHelperEditor : IntegrationHelperEditorBase
    {
        [MenuItem(InventoryPro.ToolsMenuPath + "Integrations", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<IntegrationHelperEditor>(true, "Integrations", true);
        }

        protected override void DrawIntegrations()
        {
            ShowIntegration("Rewired", "Rewired is an advanced input system that completely redefines how you work with input, giving you an unprecedented level of control over one of the most important components of your game.", GetUrlForProductWithID("21676"), "REWIRED");
            ShowIntegration("Easy Save 2", "Easy save is a fast and easy tool to load and save almost any data type.", GetUrlForProductWithID("768"), "EASY_SAVE_2");
            ShowIntegration("PlayMaker", "Quickly make gameplay prototypes, A.I. behaviors, animation graphs, interactive objects, cut-scenes, walkthroughs", GetUrlForProductWithID("368"), "PLAYMAKER");
            ShowIntegration("UMA 2", "UMA 2 characters", GetUrlForProductWithID("35611"), "UMA");
            ShowIntegration("Node Canvas", "The complete Visual Behaviour Authoring framework for Unity, empowering you to create advanced AI Behaviours and Logic, including three separate, fully featured, yet seamlessly interchangeable modules for you to choose and easily add in your game.", GetUrlForProductWithID("14914"), "NODE_CANVAS");
            //ShowIntegration("iCode", "Quickly make gameplay prototypes, A.I. behaviors, interactions between GameObjects and more. ", GetUrlForProductWithID("13761"), "ICODE");
            ShowIntegration("Behavior Designer", "Behavior trees are used by AAA studios to create a lifelike AI. With Behavior Designer, you can bring the power of behaviour trees to Unity!", GetUrlForProductWithID("15277"), "BEHAVIOR_DESIGNER");
            ShowIntegration("Dialogue System", "Dialogue System for Unity makes it easy to add interactive dialogue to your game. It's a complete, robust solution including a visual node-based editor, dialogue UIs, cutscenes, quests, save/load, and more.", GetUrlForProductWithID("11672"), "DIALOGUE_SYSTEM");
            ShowIntegration("Relations Inspector", "", GetUrlForProductWithID("53147"), "RELATIONS_INSPECTOR");

            EditorGUILayout.BeginVertical(EditorStyles.boxStyle);
            ShowIntegration("UFPS (Ultimate FPS)", "Featuring the SMOOTHEST CONTROLS and the most POWERFUL FPS CAMERA available for Unity, Ultimate FPS is an awesome script pack for achieving that special AAA FPS feeling.", GetUrlForProductWithID("2943"), "UFPS", false);
            ShowIntegration("UFPS Multiplayer", "UFPS Multiplayer", GetUrlForProductWithID("33752"), "UFPS_MULTIPLAYER", false);
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            ShowIntegration("plyGame", "plyGame has been designed with ease of use in mind and provides the components and editors you need to create games of various genres, from RPGs and Visual Novels to Action games.", GetUrlForProductWithID("9694"), "PLY_GAME");
        }
    }
}