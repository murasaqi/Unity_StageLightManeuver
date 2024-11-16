using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;


namespace StageLightManeuver
{
    [InitializeOnLoad]
    public class IntegrationSetupWindow : EditorWindow
    {
        private const string ASSEMBLY_NAME = "com.saladgamer.volumetriclightbeam";
        private const string VLB_INTEGRATION_SYMBOL = "USE_VLB";
        private const string VLB_MENU_CATEGORY_NAME = "\U0001F4A1 Volumetric Light Beam";
        private const string VLB_CALL_INIT_MENU_NAME = "\u2699 Open Config";
        private const string VLB_CALL_INIT_MENU_PATH = "Edit/" + VLB_MENU_CATEGORY_NAME + "/" + VLB_CALL_INIT_MENU_NAME;
        private const string WINDOW_TITLE = "Setup SLM Integration";
        private const string MENU_PATH = "Tools/Stage Light Maneuver/Integration Setup";

        private static class Styles
        {
            private static GUIStyle m_boxHeader;
            
            public static GUIStyle boxHeader
            {
                get
                {
                    m_boxHeader = m_boxHeader ?? new GUIStyle("PreToolbar2");
                    m_boxHeader.richText = true;
                    return m_boxHeader;
                }
            }
            
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
                    m_statusDisabled.richText = true;
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


        static IntegrationSetupWindow()
        {
            CompilationPipeline.assemblyCompilationFinished += (assembly, messages) => UpdateIntegrationStatus();
            // CompilationPipeline.compilationFinished += UpdateIntegrationStatus;
        }

        [MenuItem(MENU_PATH)]
        private static void ShowWindow()
        {
            IntegrationSetupWindow window = EditorWindow.GetWindow<IntegrationSetupWindow>(true, WINDOW_TITLE, true);
            window.Show();
        }

        private void OnGUI()
        {
            minSize = new(440, 260);
            titleContent = new GUIContent(WINDOW_TITLE);
            // GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            // labelStyle.richText = true;

            EditorGUILayout.LabelField("Setup SLM Integration", Styles.headerLabel,
                GUILayout.Height(Styles.headerLabel.lineHeight));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            EditorGUILayout.HelpBox(
                "After setup or uninstallation of the integration or update associated packages, some errors may occur.\nIn this case, restart Unity and reopen this window.",
                MessageType.Info);
            EditorGUILayout.Separator();

            using (new EditorGUILayout.VerticalScope(Styles.flatBox))
            {
                EditorGUILayout.LabelField("Integration Status", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(Styles.frameBox))
                {
                    EditorGUILayout.LabelField("VLB Integration", Styles.boxHeader);
                    EditorGUILayout.Space(4);

                    EditorGUILayout.LabelField(" Used in SLM", "Optional", Styles.statusSuccess);

                    // 各ステータスを表示 
                    // VLB Integration のインストールステータス
                    // - VLB のインストールステータス
                    // - VLB のシンボル定義ステータス
                    // - VLB の API にアクセスできるか

                    GUIContent vlbPackage = new GUIContent(" Package");
                    GUIContent vlbInstalled = new GUIContent("Installed", Styles.iconSuccess);
                    GUIContent vlbNotInstalled = new GUIContent("Not Installed", Styles.iconWarn);
                    GUIContent vlbSymbol = new GUIContent(" Integration Symbols");
                    GUIContent vlbSymbolDefined = new GUIContent("Defined", Styles.iconSuccess);
                    GUIContent vlbSymbolNotDefined = new GUIContent("Not Defined",
                        CheckVlbApi() ? Styles.iconError : Styles.iconWarn,
                        "Integration activation symbol is undefined");
                    GUIContent vlbIntegration = new GUIContent(" API Access");
                    GUIContent vlbIntegrationAvailable =
                        new GUIContent("Available", Styles.iconSuccess, "API access succeeded!");
                    GUIContent vlbIntegrationNotAvailable =
                        new GUIContent("Not Available", Styles.iconError, "Cannot access API");


                    if (CheckVlbInstalled())
                    {
                        EditorGUILayout.LabelField(vlbPackage, vlbInstalled, Styles.statusSuccess);
                        if (CheckSymbolDefine())
                        {
                            EditorGUILayout.LabelField(vlbSymbol, vlbSymbolDefined, Styles.statusSuccess);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(vlbSymbol, vlbSymbolNotDefined,
                                CheckVlbApi() ? Styles.statusError : Styles.statusWarn);
                        }

                        if (CheckVlbApi())
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
                        EditorGUILayout.LabelField(vlbSymbol.text,
                            CheckSymbolDefine() ? vlbSymbolDefined.text : vlbSymbolNotDefined.text,
                            Styles.statusDisabled);
                        EditorGUILayout.LabelField(vlbIntegration.text,
                            CheckVlbApi() ? vlbIntegrationAvailable.text : vlbIntegrationNotAvailable.text,
                            Styles.statusDisabled);
                    }
                }
            }
            EditorGUILayout.Separator();
            
            EditorGUI.BeginDisabledGroup(CheckSymbolDefine() && CheckVlbApi() || !CheckVlbInstalled());
            if (GUILayout.Button("Fix All"))
            {
                InstallAssembly();
                CompilationPipeline.RequestScriptCompilation();
                SetSymbolDefine(VLB_INTEGRATION_SYMBOL, true);
                CompilationPipeline.RequestScriptCompilation();
                Repaint();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();
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
        private static bool CheckSymbolDefine()
        {
#if USE_VLB
            return true;
#else
            var symbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return symbols.Contains(VLB_INTEGRATION_SYMBOL);
#endif
        }

        /// <summary>
        /// USE_VLB シンボルを定義または未定義にします
        /// </summary>
        private static void SetSymbolDefine(string symbolName, bool define)
        {
            var symbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (define)
            {
                if (!symbols.Contains(symbolName))
                {
#if !USE_VLB
                    symbols += ";" + symbolName;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                        symbols);
                    CompilationPipeline.RequestScriptCompilation();
#endif
                }
            }
            else
            {
                if (symbols.Contains(symbolName))
                {
                    symbols = symbols.Replace(symbolName, "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                        symbols);
                    CompilationPipeline.RequestScriptCompilation();
                }
            }
        }

        /// <summary>
        /// VLB がインストール済みか確認します
        /// </summary>
        /// <returns> VLB がインストール済みか否か </returns>
        private static bool CheckVlbInstalled()
        {
            var menuItemExistsMethod = typeof(Menu).GetMethod("MenuItemExists",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);
            return (bool)menuItemExistsMethod.Invoke(null, new object[] { VLB_CALL_INIT_MENU_PATH });
        }

        /// <summary>
        /// VLB の アセンブリ定義が存在するか確認します
        /// アセンブリ定義が存在しない場合は、VLBのAPI機能へのアクセスはできません
        /// </summary>
        /// <returns>SLM から VLB の API にアクセスできるか否か</returns>
        private static bool CheckVlbApi()
        {
            // VLB のアセンブリ定義が存在するか確認
            try
            {
                // var assembly = CompilationPipeline.GetAssemblies().FirstOrDefault(a => a.name == ASSEMBLY_NAME);
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                return assemblies.Any(assembly => assembly.GetName().Name == ASSEMBLY_NAME);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static void UpdateIntegrationStatus(object obj = null) => DisableUnsupportedFeatures();

        /// <summary>
        /// 依存関係が確認できない機能を無効化します
        /// </summary>
        internal static void DisableUnsupportedFeatures()
        {
            // TODO: 遅延評価
            
            // Debug.Log("RUN DisableUnsupportedFeatures");
            // if (CheckVlbInstalled() == false)
            // {
            //     SetSymbolDefine(VLB_INTEGRATION_SYMBOL, false);
            // }
            

            if (CheckVlbInstalled() && !CheckSymbolDefine() && CheckVlbApi())
            {
                SetSymbolDefine(VLB_INTEGRATION_SYMBOL, true);
            }
        }

        private static void RestartUnity()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            AssetDatabase.Refresh();
            EditorApplication.OpenProject(System.IO.Directory.GetCurrentDirectory());
        }
    }
}