using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DarkNaku.Foundation
{
    public class GoogleSheetsEditorWindow : EditorWindow
    {
        private int _selectedIndex;

        [MenuItem("Tools/Google Sheets Setting")]
        private static void OpenWindow()
        {
            var window = GetWindow<GoogleSheetsEditorWindow>("Google Sheets Setting");

            if (GoogleSheetsConfig.IsAbleToRefreshAccessToken) 
            { 
                GoogleSheetsConfig.RefreshAccessToken();
            }
        }

        private void OnGUI()
        {
            GoogleSheetsConfig.ClientID = EditorGUILayout.TextField("Client ID", GoogleSheetsConfig.ClientID);
            GoogleSheetsConfig.ClientSecret = EditorGUILayout.TextField("Client Secret", GoogleSheetsConfig.ClientSecret);
            DrawReadOnlyTextField("Access Token", GoogleSheetsConfig.AccessToken);
            DrawReadOnlyTextField("Refresh Token", GoogleSheetsConfig.RefreshToken);
            DrawReadOnlyTextField("Expire Time", (new DateTime(GoogleSheetsConfig.ExpireTime)).ToString());

            EditorGUI.BeginDisabledGroup(GoogleSheetsConfig.IsAbleToRequestAccessToken == false);
            if (GUILayout.Button("Get Access Token")) GoogleSheetsConfig.RequestAccessToken();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(GoogleSheetsConfig.HasAccessToken == false);
            if (GUILayout.Button("Get Google Sheet List")) GoogleSheetsConfig.RequestSpreadSheets();
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(20);

            EditorGUI.BeginDisabledGroup(GoogleSheetsConfig.SpreadSheetNames.Count == 0);
            _selectedIndex = DrawPopup("Spread Sheets", _selectedIndex, GoogleSheetsConfig.SpreadSheetNames.ToArray());
            GoogleSheetsConfig.SelectSpreadSheet(_selectedIndex);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(GoogleSheetsConfig.IsSelectedSpreadSheetID == false);
            if (GUILayout.Button("Download Google Sheet")) GoogleSheetsConfig.DownloadSpreadSheet();
            EditorGUI.EndDisabledGroup();

            EditorUtility.SetDirty(GoogleSheetsConfig.Instance);
        }

        private void DrawReadOnlyTextField(string label, string text)
        { 
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
                EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
	    }

        private int DrawPopup(string label, int index, string[] list)
        {
            int selectedIndex = 0;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
                selectedIndex = EditorGUILayout.Popup(index, GoogleSheetsConfig.SpreadSheetNames.ToArray());
            }
            EditorGUILayout.EndHorizontal();

            return selectedIndex;
        }
    }
}
