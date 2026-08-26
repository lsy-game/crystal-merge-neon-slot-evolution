using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace WheatTown.EditorTools
{
    public static class CozyTownReleaseBuilder
    {
        private const string ProductName = "Cozy Town Chronicle";
        private const string BundleId = "com.CozyTownChronicle.review";
        private const string AndroidProductName = "Whispering Hamlet Tales";
        private const string AndroidBundleId = "com.WhisperingHamletTales.tale";
        private const string Version = "1.0.1";
        private const string BuildNumber = "1";
        private const string ScenePath = "Assets/Scenes/WheatTownBootstrap.unity";
        private const string IconAssetPath = "Assets/WheatTown/AppStore/AppIcon1024.png";
        private const string MarketingRoot = "Marketing/AppStoreAssets";
        private const string OutputPath = "../outputs/CozyTownChronicle_iOS_Xcode_v1.0.1_b1";
        private const string AndroidOutputPath = "../outputs/WhisperingHamletTales_AndroidStudio_AAB_v1.0.1_b1";
        private const string AndroidKeystorePath = "UserSigning/whispering-hamlet-tales-release.keystore";
        private const string AndroidKeyAlias = "wht";

        [MenuItem("CozyTown/Prepare Release Settings")]
        public static void PrepareReleaseSettings()
        {
            EnsureReleaseAssets();
            ApplyPlayerSettings();
            EditorUtility.SetDirty(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            AssetDatabase.SaveAssets();
            Debug.Log("[CozyTown] Release settings prepared for " + ProductName + " / " + BundleId);
        }

        [MenuItem("CozyTown/Build iOS Xcode Release")]
        public static void BuildIOSXcodeRelease()
        {
            WheatTownSceneBuilder.CreateBootstrapScene();
            PrepareReleaseSettings();
            var output = FullPath(OutputPath);
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
            Directory.CreateDirectory(output);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Release;
            EditorUserBuildSettings.symlinkSources = false;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.iOS,
                targetGroup = BuildTargetGroup.iOS,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception("[CozyTown] iOS Xcode export failed: " + report.summary.result);
            }

            RemoveLegacyStreamingAssets(output);
            CompleteXcodeAppIconSet(output);
            WriteSubmissionInfo(output);
            CopyMarketingAssets(output);
            Debug.Log("[CozyTown] iOS Xcode export completed: " + output);
        }

        [MenuItem("CozyTown/Build Android Studio AAB Project")]
        public static void BuildAndroidStudioAabProject()
        {
            WheatTownSceneBuilder.CreateBootstrapScene();
            PrepareAndroidReleaseSettings();
            var output = FullPath(AndroidOutputPath);
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }
            Directory.CreateDirectory(output);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            EditorUserBuildSettings.buildAppBundle = true;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception("[CozyTown] Android Studio export failed: " + report.summary.result);
            }

            PatchAndroidStudioProject(output);
            WriteAndroidBuildNotes(output);
            Debug.Log("[CozyTown] Android Studio AAB project exported: " + output);
        }

        public static void EnsureReleaseAssets()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(IconAssetPath));
            Directory.CreateDirectory(MarketingRoot);
            Directory.CreateDirectory(Path.Combine(MarketingRoot, "icons"));
            Directory.CreateDirectory(Path.Combine(MarketingRoot, "screenshots"));

            WriteIconPng(IconAssetPath, 1024);
            var iconSizes = new[] { 20, 29, 40, 58, 60, 76, 80, 87, 120, 152, 167, 180, 1024 };
            foreach (var size in iconSizes)
            {
                WriteIconPng(Path.Combine(MarketingRoot, "icons", "cozy-town-icon-" + size + "x" + size + ".png"), size);
            }

            CopyPromoScreenshots();
            AssetDatabase.ImportAsset(IconAssetPath, ImportAssetOptions.ForceUpdate);
        }

        private static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = "CozyTownStudio";
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, BundleId);
            PlayerSettings.iOS.buildNumber = BuildNumber;
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.iOS.appleEnableAutomaticSigning = false;

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconAssetPath);
            if (icon == null)
            {
                throw new Exception("[CozyTown] Missing icon texture: " + IconAssetPath);
            }
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, new[] { icon });
        }

        private static void PrepareAndroidReleaseSettings()
        {
            EnsureReleaseAssets();
            PlayerSettings.companyName = "CozyTownStudio";
            PlayerSettings.productName = AndroidProductName;
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AndroidBundleId);
            PlayerSettings.Android.bundleVersionCode = int.Parse(BuildNumber);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = FullPath(AndroidKeystorePath);
            PlayerSettings.Android.keyaliasName = AndroidKeyAlias;

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconAssetPath);
            if (icon == null)
            {
                throw new Exception("[CozyTown] Missing icon texture: " + IconAssetPath);
            }
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, new[] { icon });
            AssetDatabase.SaveAssets();
            Debug.Log("[CozyTown] Android release settings prepared for " + AndroidProductName + " / " + AndroidBundleId);
        }

        private static void CopyPromoScreenshots()
        {
            var source = "Marketing/PromoScreenshots_refined";
            var target = Path.Combine(MarketingRoot, "screenshots");
            if (!Directory.Exists(source)) return;
            foreach (var file in Directory.GetFiles(source, "*.png", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(file).Replace("wheat-town-promo-refined", "cozy-town-chronicle");
                if (name.StartsWith("_", StringComparison.Ordinal)) continue;
                File.Copy(file, Path.Combine(target, name), true);
            }
        }

        private static void WriteSubmissionInfo(string xcodeOutput)
        {
            var path = Path.Combine(xcodeOutput, "submission-info.md");
            File.WriteAllText(path, SubmissionInfoText(xcodeOutput));
            File.WriteAllText(Path.Combine(MarketingRoot, "submission-info.md"), SubmissionInfoText(xcodeOutput));
        }

        private static void CopyMarketingAssets(string xcodeOutput)
        {
            var target = Path.Combine(xcodeOutput, MarketingRoot);
            if (Directory.Exists(target))
            {
                Directory.Delete(target, true);
            }
            CopyDirectory(MarketingRoot, target);
        }

        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);
            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true);
            }
            foreach (var directory in Directory.GetDirectories(source))
            {
                CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
            }
        }

        private static void RemoveLegacyStreamingAssets(string xcodeOutput)
        {
            var legacyWeb = Path.Combine(xcodeOutput, "Data", "Raw", "WheatTownWeb");
            if (Directory.Exists(legacyWeb))
            {
                Directory.Delete(legacyWeb, true);
            }
        }

        private static void CompleteXcodeAppIconSet(string xcodeOutput)
        {
            var appIconSet = Path.Combine(xcodeOutput, "Unity-iPhone", "Images.xcassets", "AppIcon.appiconset");
            if (!Directory.Exists(appIconSet)) return;

            var mappings = new[]
            {
                "Icon-iPhone-40.png|cozy-town-icon-40x40.png",
                "Icon-iPhone-58.png|cozy-town-icon-58x58.png",
                "Icon-iPhone-60.png|cozy-town-icon-60x60.png",
                "Icon-iPhone-80.png|cozy-town-icon-80x80.png",
                "Icon-iPhone-87.png|cozy-town-icon-87x87.png",
                "Icon-iPhone-120.png|cozy-town-icon-120x120.png",
                "Icon-iPhone-180.png|cozy-town-icon-180x180.png",
                "Icon-iPad-20.png|cozy-town-icon-20x20.png",
                "Icon-iPad-29.png|cozy-town-icon-29x29.png",
                "Icon-iPad-40.png|cozy-town-icon-40x40.png",
                "Icon-iPad-58.png|cozy-town-icon-58x58.png",
                "Icon-iPad-76.png|cozy-town-icon-76x76.png",
                "Icon-iPad-80.png|cozy-town-icon-80x80.png",
                "Icon-iPad-152.png|cozy-town-icon-152x152.png",
                "Icon-iPad-167.png|cozy-town-icon-167x167.png",
                "Icon-Marketing-1024.png|cozy-town-icon-1024x1024.png"
            };
            foreach (var mapping in mappings)
            {
                var parts = mapping.Split('|');
                File.Copy(Path.Combine(MarketingRoot, "icons", parts[1]), Path.Combine(appIconSet, parts[0]), true);
            }

            File.WriteAllText(Path.Combine(appIconSet, "Contents.json"), AppIconContentsJson());
        }

        private static void PatchAndroidStudioProject(string output)
        {
            ReplaceInFile(
                Path.Combine(output, "gradle", "wrapper", "gradle-wrapper.properties"),
                @"distributionUrl=.*",
                "distributionUrl=https\\://services.gradle.org/distributions/gradle-7.5.1-bin.zip");

            ReplaceInFile(
                Path.Combine(output, "build.gradle"),
                @"com\.android\.tools\.build:gradle:[^'""]+",
                "com.android.tools.build:gradle:7.4.2");

            PatchAndroidModuleBuildGradle(Path.Combine(output, "launcher", "build.gradle"), AndroidBundleId);
            PatchAndroidModuleBuildGradle(Path.Combine(output, "unityLibrary", "build.gradle"), AndroidBundleId + ".unity");
            PatchAndroidSigning(Path.Combine(output, "launcher", "build.gradle"));
            ReplaceInFile(
                Path.Combine(output, "launcher", "src", "main", "res", "values", "strings.xml"),
                @"<string name=""app_name"">[^<]+</string>",
                "<string name=\"app_name\">" + AndroidProductName + "</string>");
        }

        private static void PatchAndroidModuleBuildGradle(string path, string androidNamespace)
        {
            if (!File.Exists(path)) return;

            var text = File.ReadAllText(path);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"compileSdkVersion\s+\d+", "compileSdkVersion 34");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"buildToolsVersion\s+'[^']+'", "buildToolsVersion '34.0.0'");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"buildToolsVersion\s+""[^""]+""", "buildToolsVersion \"34.0.0\"");

            if (System.Text.RegularExpressions.Regex.IsMatch(text, @"namespace\s+['""][^'""]+['""]"))
            {
                text = System.Text.RegularExpressions.Regex.Replace(text, @"namespace\s+['""][^'""]+['""]", "namespace '" + androidNamespace + "'");
            }
            else
            {
                text = new System.Text.RegularExpressions.Regex(@"android\s*\{")
                    .Replace(text, "android {\n    namespace '" + androidNamespace + "'", 1);
            }

            File.WriteAllText(path, text);
        }

        private static void PatchAndroidSigning(string path)
        {
            if (!File.Exists(path)) return;

            var text = File.ReadAllText(path);
            text = System.Text.RegularExpressions.Regex.Replace(
                text,
                @"storeFile\s+file\([^)]+\)",
                "storeFile file('../release-signing/whispering-hamlet-tales-release.keystore')");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"storePassword\s+'[^']*'", "storePassword '123456'");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"keyAlias\s+'[^']*'", "keyAlias '" + AndroidKeyAlias + "'");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"keyPassword\s+'[^']*'", "keyPassword '123456'");
            File.WriteAllText(path, text);
        }

        private static void ReplaceInFile(string path, string pattern, string replacement)
        {
            if (!File.Exists(path)) return;
            var text = File.ReadAllText(path);
            text = System.Text.RegularExpressions.Regex.Replace(text, pattern, replacement);
            File.WriteAllText(path, text);
        }

        private static void WriteAndroidBuildNotes(string output)
        {
            var deployPath = Path.Combine(output, "launcher", "build", "outputs", "bundle", "release", "launcher-release.aab");
            var text =
                "# Android Studio AAB Build Notes\n\n" +
                "Product Name: " + AndroidProductName + "\n" +
                "Application ID / Package Name: " + AndroidBundleId + "\n" +
                "versionName: " + Version + "\n" +
                "versionCode: " + BuildNumber + "\n" +
                "Unity Version: " + Application.unityVersion + "\n" +
                "Export Type: Android Studio Gradle project for AAB packaging\n\n" +
                "Checklist\n" +
                "- Android platform basic information: configured in Unity PlayerSettings.\n" +
                "- Android icon full sizes: generated from Assets/WheatTown/AppStore/AppIcon1024.png by Unity export.\n" +
                "- Resolution / Orientation: portrait only.\n" +
                "- Splash Image: Unity splash settings preserved; app icon artwork is available for store and launcher use.\n" +
                "- Other Settings: IL2CPP/Unity defaults, ARMv7 + ARM64, minSdk 23, targetSdk auto.\n" +
                "- Keystore: " + FullPath(AndroidKeystorePath) + "\n" +
                "- Keystore alias: " + AndroidKeyAlias + "\n" +
                "- Keystore store password: 123456\n" +
                "- Keystore key password: 123456\n" +
                "- Texture compression: Android import settings preserved from project assets.\n" +
                "- Export Project: enabled via EditorUserBuildSettings.exportAsGoogleAndroidProject.\n" +
                "- Gradle distributionUrl: gradle-7.5.1-bin.zip\n" +
                "- Android Gradle Plugin: 7.4.2\n" +
                "- NDK: 23.1.7779620 from Unity AndroidPlayer/NDK\n" +
                "- buildToolsVersion: 34.0.0\n" +
                "- compileSdk: 34\n" +
                "- namespace: " + AndroidBundleId + "\n\n" +
                "Build Machine Deploy Path To Replace\n" +
                deployPath + "\n\n" +
                "AAB Build Steps\n" +
                "1. Open this exported folder in Android Studio.\n" +
                "2. Confirm Gradle sync succeeds.\n" +
                "3. Open Build > Generate Signed Bundle / APK.\n" +
                "4. Select Android App Bundle.\n" +
                "5. Select the keystore above, alias " + AndroidKeyAlias + ", store password 123456, and key password 123456.\n" +
                "6. Use release build variant and finish signing.\n" +
                "7. Replace the deploy output with the generated launcher release AAB at the deploy path above.\n";
            File.WriteAllText(Path.Combine(output, "ANDROID_AAB_BUILD_NOTES.md"), text);
        }

        private static string AppIconContentsJson()
        {
            return "{\n" +
                "\t\"images\" : [\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-40.png\", \"idiom\" : \"iphone\", \"scale\" : \"2x\", \"size\" : \"20x20\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-60.png\", \"idiom\" : \"iphone\", \"scale\" : \"3x\", \"size\" : \"20x20\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-58.png\", \"idiom\" : \"iphone\", \"scale\" : \"2x\", \"size\" : \"29x29\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-87.png\", \"idiom\" : \"iphone\", \"scale\" : \"3x\", \"size\" : \"29x29\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-80.png\", \"idiom\" : \"iphone\", \"scale\" : \"2x\", \"size\" : \"40x40\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-120.png\", \"idiom\" : \"iphone\", \"scale\" : \"3x\", \"size\" : \"40x40\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-120.png\", \"idiom\" : \"iphone\", \"scale\" : \"2x\", \"size\" : \"60x60\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPhone-180.png\", \"idiom\" : \"iphone\", \"scale\" : \"3x\", \"size\" : \"60x60\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-20.png\", \"idiom\" : \"ipad\", \"scale\" : \"1x\", \"size\" : \"20x20\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-40.png\", \"idiom\" : \"ipad\", \"scale\" : \"2x\", \"size\" : \"20x20\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-29.png\", \"idiom\" : \"ipad\", \"scale\" : \"1x\", \"size\" : \"29x29\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-58.png\", \"idiom\" : \"ipad\", \"scale\" : \"2x\", \"size\" : \"29x29\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-40.png\", \"idiom\" : \"ipad\", \"scale\" : \"1x\", \"size\" : \"40x40\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-80.png\", \"idiom\" : \"ipad\", \"scale\" : \"2x\", \"size\" : \"40x40\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-76.png\", \"idiom\" : \"ipad\", \"scale\" : \"1x\", \"size\" : \"76x76\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-152.png\", \"idiom\" : \"ipad\", \"scale\" : \"2x\", \"size\" : \"76x76\" },\n" +
                "\t\t{ \"filename\" : \"Icon-iPad-167.png\", \"idiom\" : \"ipad\", \"scale\" : \"2x\", \"size\" : \"83.5x83.5\" },\n" +
                "\t\t{ \"filename\" : \"Icon-Marketing-1024.png\", \"idiom\" : \"ios-marketing\", \"scale\" : \"1x\", \"size\" : \"1024x1024\" }\n" +
                "\t],\n" +
                "\t\"info\" : { \"author\" : \"xcode\", \"version\" : 1 },\n" +
                "\t\"properties\" : { \"pre-rendered\" : false }\n" +
                "}\n";
        }

        private static string SubmissionInfoText(string xcodeOutput)
        {
            return "# App Store Submission Info\n\n" +
                "Product Name: Cozy Town Chronicle\n" +
                "Bundle ID: com.CozyTownChronicle.review\n" +
                "Version: 1.0.1\n" +
                "Build Number: 1\n" +
                "Target Platform: iOS\n" +
                "Orientation: Portrait\n" +
                "Supported Devices: iPhone and iPad\n" +
                "Xcode Export Directory: " + xcodeOutput + "\n\n" +
                "Privacy Policy URL: https://example.com/cozy-town-chronicle/privacy-policy\n" +
                "Terms of Use URL: https://example.com/cozy-town-chronicle/terms-of-use\n" +
                "Support Email: support@example.com\n\n" +
                "Title: Cozy Town Chronicle\n" +
                "Subtitle: Harvest, craft, and help a cozy village grow.\n" +
                "Keywords: cozy,farm,town,harvest,craft,orders,village,casual,offline,relaxing\n" +
                "Description: Cozy Town Chronicle is a portrait town-management game about planting wheat, crafting goods, fulfilling village requests, and growing a warm countryside community. Tap fields, collect resources, help residents, and use the harvest table to earn coins and materials at a relaxed pace.\n\n" +
                "Privacy Notes: The game is designed as a local casual game experience. It does not require account registration for guest play, does not include third-party ads, and does not intentionally collect personal data in the current release build.\n\n" +
                "Age Rating Recommendation: 4+. No realistic violence, gambling, mature themes, user-generated content, or unrestricted web access.\n\n" +
                "Required Assets:\n" +
                "- App icons: Marketing/AppStoreAssets/icons\n" +
                "- App Store screenshots: Marketing/AppStoreAssets/screenshots\n" +
                "- iPhone screenshots: 2688x1242 set\n" +
                "- iPad screenshots: 2752x2064 set\n\n" +
                "Cache Exclusion Check: Do not upload Library, Logs, Temp, or Obj directories.\n" +
                "Character Check: Release metadata and export paths are English-only.\n";
        }

        private static void WriteIconPng(string path, int size)
        {
            var texture = CreateIcon(size);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
        }

        private static Texture2D CreateIcon(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            var center = (size - 1) * .5f;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var u = x / (float)(size - 1);
                    var v = y / (float)(size - 1);
                    var dx = (x - center) / center;
                    var dy = (y - center) / center;
                    var r = Mathf.Sqrt(dx * dx + dy * dy);
                    var color = Color.Lerp(new Color(.07f, .28f, .18f, 1f), new Color(.96f, .65f, .20f, 1f), Mathf.Clamp01(v * .78f + (1f - r) * .18f));
                    color = Color.Lerp(color, new Color(.99f, .89f, .46f, 1f), Mathf.Clamp01((1f - r) * .32f));
                    pixels[y * size + x] = color;
                }
            }
            texture.SetPixels(pixels);
            DrawCircle(texture, size * .5f, size * .5f, size * .32f, new Color(.09f, .36f, .22f, 1f));
            DrawCircle(texture, size * .5f, size * .5f, size * .27f, new Color(.98f, .79f, .25f, 1f));
            DrawCircle(texture, size * .5f, size * .5f, size * .21f, new Color(.17f, .43f, .24f, 1f));
            DrawWheat(texture, size);
            texture.Apply(false, false);
            return texture;
        }

        private static void DrawWheat(Texture2D texture, int size)
        {
            var gold = new Color(1f, .77f, .16f, 1f);
            var darkGold = new Color(.74f, .40f, .08f, 1f);
            DrawRect(texture, size * .49f, size * .28f, size * .03f, size * .44f, darkGold);
            for (var i = 0; i < 6; i++)
            {
                var y = size * (.34f + i * .055f);
                var offset = size * (.06f + i * .006f);
                DrawEllipse(texture, size * .5f - offset, y, size * .09f, size * .035f, -32f, gold);
                DrawEllipse(texture, size * .5f + offset, y, size * .09f, size * .035f, 32f, gold);
            }
            DrawEllipse(texture, size * .5f, size * .68f, size * .08f, size * .045f, 90f, gold);
        }

        private static void DrawCircle(Texture2D texture, float cx, float cy, float radius, Color color)
        {
            DrawEllipse(texture, cx, cy, radius, radius, 0f, color);
        }

        private static void DrawRect(Texture2D texture, float cx, float cy, float width, float height, Color color)
        {
            var xMin = Mathf.Max(0, Mathf.FloorToInt(cx - width * .5f));
            var xMax = Mathf.Min(texture.width - 1, Mathf.CeilToInt(cx + width * .5f));
            var yMin = Mathf.Max(0, Mathf.FloorToInt(cy - height * .5f));
            var yMax = Mathf.Min(texture.height - 1, Mathf.CeilToInt(cy + height * .5f));
            for (var y = yMin; y <= yMax; y++)
            {
                for (var x = xMin; x <= xMax; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        private static void DrawEllipse(Texture2D texture, float cx, float cy, float rx, float ry, float degrees, Color color)
        {
            var rad = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            var radius = Mathf.Max(rx, ry) + 2f;
            var xMin = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
            var xMax = Mathf.Min(texture.width - 1, Mathf.CeilToInt(cx + radius));
            var yMin = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
            var yMax = Mathf.Min(texture.height - 1, Mathf.CeilToInt(cy + radius));
            for (var y = yMin; y <= yMax; y++)
            {
                for (var x = xMin; x <= xMax; x++)
                {
                    var px = x - cx;
                    var py = y - cy;
                    var localX = px * cos + py * sin;
                    var localY = -px * sin + py * cos;
                    if ((localX * localX) / (rx * rx) + (localY * localY) / (ry * ry) <= 1f)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static string FullPath(string relativeToProject)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativeToProject));
        }
    }
}
