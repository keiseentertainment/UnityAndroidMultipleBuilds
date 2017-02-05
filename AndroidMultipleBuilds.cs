#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AndroidMultipleBuilds.ScreenshotTool
{
    [ExecuteInEditMode]
    public class AndroidMultipleBuilds : EditorWindow
    {
        #region variables
        public string keystorePath = "";
        public string keystorePass = "";
        public string keyaliasName = "";
        public string keyaliasPass = "";
        public string savePath = "";

        public int bundleVersionCode = 1;

        public bool buildARM = true;
        public bool buildX86 = true;
        //public bool buildFAT = false;
        public bool[] buildARMTextures = new bool[7] { true, true, true, true, true, false, true };
        public bool[] buildX86Textures = new bool[7] { true, true, true, true, true, false, true };
        //public bool[] buildFATTextures = new bool[7] { true, true, true, true, true, false, true };

        public Vector2 scrollPosition;

        List<AndroidTargetDevice> targetDeviceConfigs = new List<AndroidTargetDevice>();
        List<MobileTextureSubtarget> textureConfigsARM = new List<MobileTextureSubtarget>();
        List<MobileTextureSubtarget> textureConfigsX86 = new List<MobileTextureSubtarget>();
        //List<MobileTextureSubtarget> textureConfigsFAT = new List<MobileTextureSubtarget>();

        Dictionary<int, MobileTextureSubtarget> texturesTarget = new Dictionary<int, MobileTextureSubtarget>()
        {
            { 0, MobileTextureSubtarget.Generic },
            { 1, MobileTextureSubtarget.DXT },
            { 2, MobileTextureSubtarget.PVRTC },
            { 3, MobileTextureSubtarget.ATC },
            { 4, MobileTextureSubtarget.ETC },
            { 5, MobileTextureSubtarget.ETC2 },
            { 6, MobileTextureSubtarget.ASTC }
        };
        #endregion

        #region menuitem
        [MenuItem("Window/Android Multiple Builds")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(AndroidMultipleBuilds));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
            editorWindow.title = "AMB";
        }
        #endregion

        #region ongui
        void OnGUI()
        {
            //string[] versionAux = Application.version.Split('.');
            //string versionAux2 = Application.version.Replace(".", "");

            bundleVersionCode = int.Parse(Application.version.Replace(".", "")) * 100;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(650));

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            buildARM = EditorGUILayout.BeginToggleGroup("ARMv7", buildARM);
            buildARMTextures[0] = EditorGUILayout.Toggle("Don't override", buildARMTextures[0]);
            buildARMTextures[1] = EditorGUILayout.Toggle(MobileTextureSubtarget.DXT.ToString(), buildARMTextures[1]);
            buildARMTextures[2] = EditorGUILayout.Toggle(MobileTextureSubtarget.PVRTC.ToString(), buildARMTextures[2]);
            buildARMTextures[3] = EditorGUILayout.Toggle(MobileTextureSubtarget.ATC.ToString(), buildARMTextures[3]);
            buildARMTextures[4] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC.ToString(), buildARMTextures[4]);
            buildARMTextures[5] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC2.ToString(), buildARMTextures[5]);
            buildARMTextures[6] = EditorGUILayout.Toggle(MobileTextureSubtarget.ASTC.ToString(), buildARMTextures[6]);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            buildX86 = EditorGUILayout.BeginToggleGroup("x86", buildX86);
            buildX86Textures[0] = EditorGUILayout.Toggle("Don't override", buildX86Textures[0]);
            buildX86Textures[1] = EditorGUILayout.Toggle(MobileTextureSubtarget.DXT.ToString(), buildX86Textures[1]);
            buildX86Textures[2] = EditorGUILayout.Toggle(MobileTextureSubtarget.PVRTC.ToString(), buildX86Textures[2]);
            buildX86Textures[3] = EditorGUILayout.Toggle(MobileTextureSubtarget.ATC.ToString(), buildX86Textures[3]);
            buildX86Textures[4] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC.ToString(), buildX86Textures[4]);
            buildX86Textures[5] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC2.ToString(), buildX86Textures[5]);
            buildX86Textures[6] = EditorGUILayout.Toggle(MobileTextureSubtarget.ASTC.ToString(), buildX86Textures[6]);
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.EndVertical();

            //EditorGUILayout.BeginVertical();

            //buildFAT = EditorGUILayout.BeginToggleGroup("FAT", buildFAT);
            //buildFATTextures[0] = EditorGUILayout.Toggle("Don't override", buildFATTextures[0]);
            //buildFATTextures[1] = EditorGUILayout.Toggle(MobileTextureSubtarget.DXT.ToString(), buildFATTextures[1]);
            //buildFATTextures[2] = EditorGUILayout.Toggle(MobileTextureSubtarget.PVRTC.ToString(), buildFATTextures[2]);
            //buildFATTextures[3] = EditorGUILayout.Toggle(MobileTextureSubtarget.ATC.ToString(), buildFATTextures[3]);
            //buildFATTextures[4] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC.ToString(), buildFATTextures[4]);
            //buildFATTextures[5] = EditorGUILayout.Toggle(MobileTextureSubtarget.ETC2.ToString(), buildFATTextures[5]);
            //buildFATTextures[6] = EditorGUILayout.Toggle(MobileTextureSubtarget.ASTC.ToString(), buildFATTextures[6]);
            //EditorGUILayout.EndToggleGroup();

            //EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Keystore", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Browse Keystore", GUILayout.ExpandWidth(false)))
            {
                //keystorePath = EditorUtility.SaveFolderPanel("Selecione a Keystore", keystorePath, Application.dataPath);
                keystorePath = EditorUtility.OpenFilePanel("Open existing keystore...", Application.dataPath, "Keystore;*.keystore");
            }
            EditorGUILayout.LabelField(keystorePath, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (keystorePath != "")
            {
                EditorGUILayout.Space();

                //keystorePass = EditorGUILayout.TextField("Keystore Pass", keystorePass);
                keystorePass = EditorGUILayout.PasswordField("Keystore password", keystorePass);
                keyaliasName = EditorGUILayout.TextField("Keystore Alias Name", keyaliasName);
                //keyaliasPass = EditorGUILayout.TextField("Keystore Alias Pass", keyaliasPass);
                keyaliasPass = EditorGUILayout.PasswordField("Keystore Alias password", keyaliasPass);
            }
            else
            {
                EditorGUILayout.HelpBox("If the project does not have a keystore, it is not necessary to select any file.", MessageType.Info);
            }

            EditorGUILayout.Space();

            GUILayout.Label(string.Format("Version: {0}", Application.version), EditorStyles.boldLabel);
            GUILayout.Label(string.Format("Bundle Version Code: {0}", bundleVersionCode), EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Bundle Version Code is based on the Version of the project for easy identification.", MessageType.Info);

            EditorGUILayout.Space();

            GUILayout.Label("Select the folder to save the Builds", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Browse folder", GUILayout.ExpandWidth(false)))
            {
                savePath = EditorUtility.SaveFolderPanel("Select a folder...", savePath, "Builds");
            }
            EditorGUILayout.LabelField(savePath, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            int numberOfBuilds = 0;
            //bool build = false;

            if (buildARM || buildX86 /*|| buildFAT*/)
            {
                if (buildARM)
                {
                    for (int i = 0; i < buildARMTextures.Length; i++)
                    {
                        if (buildARMTextures[i])
                        {
                            numberOfBuilds++;
                        }
                    }
                }
                if (buildX86)
                {
                    for (int i = 0; i < buildX86Textures.Length; i++)
                    {
                        if (buildX86Textures[i])
                        {
                            numberOfBuilds++;
                        }
                    }
                }
                //if (buildFAT)
                //{
                //    for (int i = 0; i < buildFATTextures.Length; i++)
                //    {
                //        if (buildFATTextures[i])
                //        {
                //            numberOfBuilds++;
                //        }
                //    }
                //}
            }

            if (numberOfBuilds > 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(string.Format("File name: {0} - {1} - Texture.apk", Application.productName, bundleVersionCode), EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox(string.Format("Total number of bulds: {0}", numberOfBuilds), MessageType.Info);

                EditorGUILayout.Space();

                if (savePath != "" || savePath.Length >= 3)
                {
                    if (GUILayout.Button("Build", GUILayout.MinHeight(45)))
                    {
                        if (savePath == "" || savePath.Length < 3)
                        {
                            savePath = EditorUtility.SaveFolderPanel("Select a folder...", savePath, "Builds");
                            ProcessAllBuilds();
                        }
                        else
                        {
                            ProcessAllBuilds();
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select a folder to save the Builds.", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select at least one type of architecture and one type of texture to do the build.", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Github", GUILayout.MinHeight(30)))
            {
                Application.OpenURL("https://github.com/keiseentertainment");
            }
            if (GUILayout.Button("Check our games!", GUILayout.MinHeight(30)))
            {
                Application.OpenURL("http://keiseentertainment.com");
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Special thanks for João Neto and Marcelo Henrique Cenço", EditorStyles.label);

            GUILayout.EndScrollView();
        }
        #endregion

        #region processbuilds
        void ProcessAllBuilds()
        {
            if (keystorePath != "")
            {
                PlayerSettings.Android.keystoreName = keystorePath;
                PlayerSettings.Android.keystorePass = keystorePass;
                PlayerSettings.Android.keyaliasName = keyaliasName;
                PlayerSettings.Android.keyaliasPass = keyaliasPass;
            }

            textureConfigsARM = new List<MobileTextureSubtarget>();
            textureConfigsX86 = new List<MobileTextureSubtarget>();
            //textureConfigsFAT = new List<MobileTextureSubtarget>();

            if (buildARM)
            {
                for (int i = 0; i < buildARMTextures.Length; i++)
                {
                    if (buildARMTextures[i])
                    {
                        textureConfigsARM.Add(texturesTarget[i]);
                    }
                }

                System.IO.Directory.CreateDirectory(savePath + "/" + AndroidTargetDevice.ARMv7.ToString());

                for (int j = 0; j < textureConfigsARM.Count; j++)
                {
                    PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
                    EditorUserBuildSettings.androidBuildSubtarget = textureConfigsARM[j];
                    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                    PlayerSettings.Android.bundleVersionCode = bundleVersionCode + 10 + j;
                    string path = savePath + "/" + AndroidTargetDevice.ARMv7.ToString() + "/" + Application.productName + " - " + PlayerSettings.Android.bundleVersionCode.ToString() + " - " + textureConfigsARM[j].ToString() + ".apk";

                    buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
                    buildPlayerOptions.locationPathName = path;
                    buildPlayerOptions.target = BuildTarget.Android;
                    string buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);
                }
            }
            if (buildX86)
            {
                for (int i = 0; i < buildX86Textures.Length; i++)
                {
                    if (buildX86Textures[i])
                    {
                        textureConfigsX86.Add(texturesTarget[i]);
                    }
                }

                System.IO.Directory.CreateDirectory(savePath + "/" + AndroidTargetDevice.x86.ToString());

                for (int j = 0; j < textureConfigsX86.Count; j++)
                {
                    PlayerSettings.Android.targetDevice = AndroidTargetDevice.x86;
                    EditorUserBuildSettings.androidBuildSubtarget = textureConfigsX86[j];
                    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                    PlayerSettings.Android.bundleVersionCode = bundleVersionCode + 20 + j;
                    string path = savePath + "/" + AndroidTargetDevice.x86.ToString() + "/" + Application.productName + " - " + PlayerSettings.Android.bundleVersionCode.ToString() + " - " + textureConfigsX86[j].ToString() + ".apk";

                    buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
                    buildPlayerOptions.locationPathName = path;
                    buildPlayerOptions.target = BuildTarget.Android;
                    string buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);
                }
            }
            //if (buildFAT)
            //{
            //    for (int i = 0; i < buildFATTextures.Length; i++)
            //    {
            //        if (buildFATTextures[i])
            //        {
            //            textureConfigsFAT.Add(texturesTarget[i]);
            //        }
            //    }

            //    System.IO.Directory.CreateDirectory(savePath + "/" + AndroidTargetDevice.FAT.ToString());

            //    for (int j = 0; j < textureConfigsFAT.Count; j++)
            //    {
            //        PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
            //        EditorUserBuildSettings.androidBuildSubtarget = textureConfigsFAT[j];
            //        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            //        PlayerSettings.Android.bundleVersionCode = bundleVersionCode + 30 + j;
            //        string path = savePath + "/" + AndroidTargetDevice.FAT.ToString() + "/" + Application.productName + " - " + PlayerSettings.Android.bundleVersionCode.ToString() + " - " + textureConfigsFAT[j].ToString() + ".apk";

            //        buildPlayerOptions.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
            //        buildPlayerOptions.locationPathName = path;
            //        buildPlayerOptions.target = BuildTarget.Android;
            //        string buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);
            //    }
            //}
        }
    }
    #endregion
}
#endif