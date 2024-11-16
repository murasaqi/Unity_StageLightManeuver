#if VLB_URP || VLB_HDRP
#define VLB_INSTALLED
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace StageLightManeuver
{
    public class IntegrationSetupWindow : EditorWindow
    {
#if VLB_INSTALLED
        private const bool VLB_INSTALLED = true;
#else
        private const bool VLB_INSTALLED = false;
#endif
        private const string WINDOW_TITLE = "Setup SLM Integration";
        private const string MENU_PATH = "Tools/Stage Light Maneuver/Integration Setup";

        private static class Styles
        {
            private static GUIStyle m_flatBox;

            public static GUIStyle flatBox
            {
                get
                {
                    m_flatBox = m_flatBox ?? new GUIStyle("OL box flat");
                    return m_flatBox;
                }
            }

            private static GUIStyle m_frameBox;

            public static GUIStyle frameBox
            {
                get
                {
                    m_frameBox = m_frameBox ?? new GUIStyle("FrameBox");
                    return m_frameBox;
                }
            }

            private static GUIStyle m_headerLabel;

            public static GUIStyle headerLabel
            {
                get
                {
                    m_headerLabel = m_headerLabel ?? new GUIStyle("SettingsHeader");
                    return m_headerLabel;
                }
            }

            private static GUIStyle m_statusDisabled;

            public static GUIStyle statusDisabled
            {
                get
                {
                    m_statusDisabled = m_statusDisabled ?? new GUIStyle("PR DisabledLabel");
                    return m_statusDisabled;
                }
            }

            private static GUIStyle m_statusInfo;

            public static GUIStyle statusInfo
            {
                get
                {
                    m_statusInfo = m_statusInfo ?? new GUIStyle("CN StatusInfo");
                    return m_statusInfo;
                }
            }

            private static GUIStyle m_statusWarn;

            public static GUIStyle statusWarn
            {
                get
                {
                    m_statusWarn = m_statusWarn ?? new GUIStyle("CN StatusWarn");
                    return m_statusWarn;
                }
            }

            private static GUIStyle m_statusError;

            public static GUIStyle statusError
            {
                get
                {
                    m_statusError = m_statusError ?? new GUIStyle("CN StatusError");
                    return m_statusError;
                }
            }

            private static GUIStyle m_statusSuccess;

            public static GUIStyle statusSuccess
            {
                get
                {
                    if (m_statusSuccess == null)
                    {
                        m_statusSuccess = new GUIStyle("CN StatusInfo");
                        m_statusSuccess.normal.textColor = Color.green;
                    }

                    return m_statusSuccess;
                }
            }

            private static Texture2D m_iconBlank;

            public static Texture2D iconBlank
            {
                get
                {
                    m_iconBlank = m_iconBlank ?? EditorGUIUtility.Load("MenuItemNormal") as Texture2D;
                    return m_iconBlank;
                }
            }

            private static Texture2D m_iconInfo;

            public static Texture2D iconInfo
            {
                get
                {
                    m_iconInfo = m_iconInfo ?? EditorGUIUtility.Load("d_console.infoicon.sml") as Texture2D;
                    return m_iconInfo;
                }
            }

            private static Texture2D m_iconWarn;

            public static Texture2D iconWarn
            {
                get
                {
                    m_iconWarn = m_iconWarn ?? EditorGUIUtility.Load("d_console.warnicon.sml") as Texture2D;
                    return m_iconWarn;
                }
            }

            private static Texture2D m_iconError;

            public static Texture2D iconError
            {
                get
                {
                    m_iconError = m_iconError ?? EditorGUIUtility.Load("d_console.erroricon.sml") as Texture2D;
                    return m_iconError;
                }
            }

            private static Texture2D m_iconSuccess;

            public static Texture2D iconSuccess
            {
                get
                {
                    m_iconSuccess = m_iconSuccess ?? EditorGUIUtility.Load("d_GreenCheckmark") as Texture2D;
                    return m_iconSuccess;
                }
            }
        }


        [MenuItem(MENU_PATH)]
        private static void ShowWindow()
        {
            IntegrationSetupWindow window = EditorWindow.GetWindow<IntegrationSetupWindow>(true, WINDOW_TITLE, true);
            window.Show();
        }

        private void OnGUI()
        {
            // EditorApplication.projectChanged += () => UpdatePackageInfoFromManifest(); // プロジェクトが変更されたらパッケージ情報を更新
            minSize = new(440, 260);
            titleContent = new GUIContent(WINDOW_TITLE);
            // GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            // labelStyle.richText = true;

            EditorGUILayout.LabelField("Setup SLM Integration", Styles.headerLabel,
                GUILayout.Height(Styles.headerLabel.lineHeight));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            using (new EditorGUILayout.VerticalScope(Styles.flatBox))
            {
                EditorGUILayout.LabelField("Integration Status", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(Styles.frameBox))
                {
                    EditorGUILayout.LabelField("VLB Integration", new GUIStyle("PreToolbar2"));
                    EditorGUILayout.Space(4);

                    // 各ステータスを表示 
                    // VLB Integration のインストールステータス
                    // - VLB のインストールステータス
                    // - VLB のシンボル定義ステータス
                    // - VLB の API にアクセスできるか

                    GUIContent vlbPackage = new GUIContent(" Package");
                    GUIContent vlbInstalled = new GUIContent("Installed", Styles.iconSuccess);
                    GUIContent vlbNotInstalled = new GUIContent("Not Installed", Styles.iconWarn);
                    GUIContent vlbSymbol = new GUIContent(" Symbol");
                    GUIContent vlbSymbolDefined = new GUIContent("Defined", Styles.iconSuccess);
                    GUIContent vlbSymbolNotDefined = new GUIContent("Not Defined",
                        CheckVLBAvailable() ? Styles.iconError : Styles.iconWarn,
                        "Integration activation symbol is undefined");
                    GUIContent vlbIntegration = new GUIContent(" API Access");
                    GUIContent vlbIntegrationAvailable =
                        new GUIContent("Available", Styles.iconSuccess, "API access succeeded!");
                    GUIContent vlbIntegrationNotAvailable =
                        new GUIContent("Not Available", Styles.iconError, "Cannot access API");


                    if (VLB_INSTALLED)
                    {
                        EditorGUILayout.LabelField(vlbPackage, vlbInstalled, Styles.statusSuccess);
                        if (CheckSymbolDefine())
                        {
                            EditorGUILayout.LabelField(vlbSymbol, vlbSymbolDefined, Styles.statusSuccess);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(vlbSymbol, vlbSymbolNotDefined,
                                CheckVLBAvailable() ? Styles.statusError : Styles.statusWarn);
                        }

                        if (CheckVLBAvailable())
                        {
                            EditorGUILayout.LabelField(vlbIntegration, vlbIntegrationAvailable, Styles.statusSuccess);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(vlbIntegration, vlbIntegrationNotAvailable, Styles.statusError);
                            // if(GUILayout.Button("Fix")){ InstallIntegration(); }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(vlbPackage, vlbNotInstalled, Styles.statusWarn);
                        EditorGUILayout.LabelField(vlbSymbol,
                            CheckSymbolDefine() ? vlbSymbolDefined.text : vlbSymbolNotDefined.text);
                        EditorGUILayout.LabelField(vlbIntegration,
                            CheckVLBAvailable() ? vlbIntegrationAvailable.text : vlbIntegrationNotAvailable.text);
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(CheckSymbolDefine() && CheckVLBAvailable() || !VLB_INSTALLED);
            if (GUILayout.Button("Fix All"))
            {
                InstallAssembly();
                SetSymbolDefine(true);
            }

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// VLB のAssembly定義をインストールします
        /// </summary>
        private void InstallAssembly()
        {
            // install Editor/Resources/Package/slm_vlb_integration.unitypackage
            // guid: dc6aaec98c31a6144bc94065bb624bb7 
            var PACKAGE_GUID = "dc6aaec98c31a6144bc94065bb624bb7";
            var packagePath = AssetDatabase.GUIDToAssetPath(PACKAGE_GUID);
            AssetDatabase.ImportPackage(packagePath, false);
        }

        /// <summary>
        /// USE_VLB シンボルが定義済みであれば true を返します
        /// </summary>
        /// <returns></returns>
        private bool CheckSymbolDefine()
        {
#if USE_VLB
            return true;
#else
            var symbols =
 PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return symbols.Contains("USE_VLB");
#endif
        }

        /// <summary>
        /// USE_VLB シンボルを定義または未定義にします
        /// </summary>
        private void SetSymbolDefine(bool define)
        {
            var symbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (define)
            {
                if (!symbols.Contains("USE_VLB"))
                {
#if !USE_VLB
                    symbols += ";USE_VLB";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
#endif
                }
            }
            else
            {
                if (symbols.Contains("USE_VLB"))
                {
                    symbols = symbols.Replace("USE_VLB", "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                        symbols);
                }
            }
        }

        /// <summary>
        /// VLB の アセンブリ定義が存在するか確認します
        /// アセンブリ定義が存在しない場合は、VLBのAPI機能へのアクセスはできません
        /// </summary>
        /// <returns></returns>
        private bool CheckVLBAvailable()
        {
            // VLB のアセンブリ定義が存在するか確認
            try
            {
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name == "com.saladgamer.volumetriclightbeam")
                    {
                        // VLB.Version.CurrentAsString にアクセスできるか確認
                        // var type = assembly.GetType("VLB.Version");
                        return true;
                    }
                }

                return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}